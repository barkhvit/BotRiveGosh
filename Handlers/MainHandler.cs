using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Commands;
using BotRiveGosh.Helpers;
using BotRiveGosh.Scenarios;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers
{
    //регистрируется новый пользователь
    //обработка отмены сценириев Cancel
    //проверка пользователя на незаконченный сценарий
    //Запуск сценария - проверка текста на запуск сценария
    //в зависимости от типа Update - переключаем в нужный обработчик
    public class MainHandler : IUpdateHandler
    {
        private readonly MessageUpdateHandler _messageUpdateHandler;
        private readonly CallBackUpdateHandler _callBackUpdateHandler;
        private readonly DocumentUpdateHandler _documentUpdateHandler;

        private readonly IUserService _userService;
        private readonly CommandsForMainMenu _commandsForMainMenu;

        //сценарии
        private readonly IScenarioContextRepository _scenarioContextRepository;
        private readonly IEnumerable<IScenario> _scenarios;

        public MainHandler(MessageUpdateHandler messageUpdateHandler, IUserService userService, 
            IScenarioContextRepository scenarioContextRepository, IEnumerable<IScenario> scenarios, 
            CommandsForMainMenu commandsForMainMenu, CallBackUpdateHandler callBackUpdateHandler,
            DocumentUpdateHandler documentUpdateHandler)
        {
            _messageUpdateHandler = messageUpdateHandler;
            _callBackUpdateHandler = callBackUpdateHandler;
            _documentUpdateHandler = documentUpdateHandler;
            _userService = userService;
            _scenarioContextRepository = scenarioContextRepository;
            _scenarios = scenarios;
            _commandsForMainMenu = commandsForMainMenu;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

                //проверка доступа
                var accessPossible =  await CheckAccessAllowed(botClient, update, ct);
                if (!accessPossible) return;

                //логирование
                //Logger.Log("Начало");
                Console.WriteLine($"{user.Id}:{update.Type}:{Text}");

                //обработка Cancel - отмена сценария
                if (Text == "/cancel" || Text == "cancel")
                {
                    await _scenarioContextRepository.ResetContext(userId, ct);
                    await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                    await botClient.EditMessageText(chatId, messageId, "Действие отменено.", cancellationToken: ct);
                    //ГЛАВНОЕ МЕНЮ
                    return;
                }

                //регистрация пользователя
                if (Text == "/start")
                {
                    var newUser = await _userService.GetOrCreateUserAsync(update, ct);
                    Console.WriteLine($"Новый пользователь: {newUser.Username}");
                    await _commandsForMainMenu.ShowMainMenu(update, ct);
                    return;
                }

                //проверка, есть ли у пользователя сценарий
                var context = await _scenarioContextRepository.GetContext(userId, ct);
                if (context != null)
                {
                    await ProcessScenario(context, update, botClient, ct);
                    return;
                }

                //проверка текста на команды запуска сценариев
                if (Text != null)
                {
                    CallBackDto callBackDto = CallBackDto.FromString(Text);
                    //Сценарий: Показать результат KPI
                    if (callBackDto.Object == Dto_Objects.Kpi && callBackDto.Action == Dto_Action.ShowResult)
                    {
                        await SetNewContext(update, ScenarioType.ShowKpiResult, botClient, ct);
                        return;
                    }
                }

                //проверяем на наличие документа
                if (update.Message?.Document != null)
                {
                    await _documentUpdateHandler.HandleUpdateAsync(botClient, update, ct);
                    return;
                }

                //в зависимости от типа Update переключаем в нужный обработчик
                switch (update.Type)
                {
                    case Telegram.Bot.Types.Enums.UpdateType.Message:
                        await _messageUpdateHandler.HandleUpdateAsync(botClient, update, ct); return;
                    case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                        await _callBackUpdateHandler.HandleUpdateAsync(botClient, update, ct); return;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Logger.Log("Окончание");
            }
            

        }

        private async Task SetNewContext(Update update, ScenarioType scenarioType, ITelegramBotClient botClient, CancellationToken ct)
        {
            // Получаем данные из update с помощью pattern matching
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            //создаем новый контекст
            var newContext = new ScenarioContext(userId, scenarioType);
            await _scenarioContextRepository.SetContext(userId, newContext, ct);
            //запускаем сценарий
            await ProcessScenario(newContext, update, botClient, ct);
        }

        private async Task ProcessScenario(ScenarioContext context, Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            //получаем IScenario
            var scenario = GetScenario(context.CurrentScenario);
            var scenarioResult = await scenario.HandleScenarioAsync(botClient, context, update, ct);
            if (scenarioResult == ScenarioResult.Completed)
            {
                await _scenarioContextRepository.ResetContext(context.UserId, ct);
            }
            else
            {
                await _scenarioContextRepository.SetContext(context.UserId, context, ct);
            }
        }

        private IScenario GetScenario(ScenarioType scenario)
        {
            var handler = _scenarios.FirstOrDefault(s => s.CanHandle(scenario));
            return handler ?? throw new ArgumentException($"Нет сценария типа: {scenario}");
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> CheckAccessAllowed(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            var inputUser = await _userService.GetOrCreateUserAsync(update, ct);

            //если нет доступа
            if (!inputUser.AccessAllowed) 
            {
                
                if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                {
                    await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

                    //если это запрос доступа возвращаем true и пропускаем дальше
                    var dto = CallBackDto.FromString(Text);
                    if (dto.Object == Dto_Objects.Reg && dto.Action == Dto_Action.RegRequest)
                        return true;
                }
                    
                await botClient.SendMessage(chatId, "У вас нет доступа. Можете запросить.",
                    cancellationToken: ct,
                    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Запросить доступ.",
                        new CallBackDto(Dto_Objects.Reg, Dto_Action.RegRequest, inputUser.Id).ToString())));
                return false;
            }
            return true;
        }
        
    }
}

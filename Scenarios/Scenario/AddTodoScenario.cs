using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace BotRiveGosh.Scenarios.Scenario
{
    public class AddTodoScenario : IScenario
    {
        private readonly ITodoService _todoService;
        private readonly IUserService _userService;
        public AddTodoScenario(ITodoService todoService, IUserService userService)
        {
            _todoService = todoService;
            _userService = userService;
        }
        public bool CanHandle(ScenarioType scenarioType)
        {
            return scenarioType == ScenarioType.AddTodo;
        }

        public async Task<ScenarioResult> HandleScenarioAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            Message? message = null;
            string messText = "";

            switch (context.CurrentStep)
            {
                case null:
                    if (update.CallbackQuery != null) await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

                    //отправляем сообщение
                    message = await botClient.EditMessageText(chatId,messageId, "Введите название задачи:", cancellationToken: ct,
                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("ОТМЕНА","cancel")));

                    //message id помещаем в хранилище
                    context.Data["MessageId"] = message.Id;

                    context.CurrentStep = CurrentStep.Name;
                    return ScenarioResult.Transition;

                //пользователь ввел имя задачи
                case nameof(CurrentStep.Name): 
                    if(Text != null && Text != String.Empty)
                    {
                        //запоминаем имя задачи в память
                        context.Data[CurrentStep.Name] = Text;

                        //удаляем сообщение пользователя
                        await botClient.DeleteMessage(chatId, messageId, ct);

                        //текст сообщения
                        messText = $"Название: {Text}\n\n Введите описание задачи:";

                        //отправляем сообщение
                        message = await botClient.EditMessageText(chatId, (int)context.Data["MessageId"], messText, cancellationToken: ct,
                            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("ОТМЕНА", "cancel")));

                        //message id помещаем в хранилище
                        context.Data["MessageId"] = message.Id;

                        context.CurrentStep = CurrentStep.Description;
                        return ScenarioResult.Transition;
                    }
                    return ScenarioResult.Transition;

                //пользователь ввел описание задачи
                case nameof(CurrentStep.Description):
                    if (Text != null && Text != String.Empty)
                    {
                        //запоминаем описание задачи в память
                        context.Data[CurrentStep.Description] = Text;

                        //удаляем сообщение пользователя
                        await botClient.DeleteMessage(chatId, messageId, ct);

                        //текст сообщения
                        messText = $"Название: {context.Data[CurrentStep.Name]}\n" +
                            $"Описание:{Text}\n\n Введите срок выполнения задачи в формате ДД.ММ.ГГГГ:";

                        //отправляем сообщение
                        message = await botClient.EditMessageText(chatId, (int)context.Data["MessageId"], messText, cancellationToken: ct,
                            replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("ОТМЕНА", "cancel")));

                        //message id помещаем в хранилище
                        context.Data["MessageId"] = message.Id;

                        context.CurrentStep = CurrentStep.FinishedAt;
                        return ScenarioResult.Transition;
                    }
                    return ScenarioResult.Transition;

                //пользователь ввел дату окончания задачи
                case nameof(CurrentStep.FinishedAt):
                    if (Text != null && Text != String.Empty)
                    {
                        if(DateOnly.TryParseExact(Text, "dd.MM.yyyy",CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                        {
                            //создаем новую задачу
                            //получаем пользователя
                            var _user = await _userService.GetUserByTelegramIdAsync(chatId, ct);

                            if(_user != null)
                            {
                                //создаем задачу
                                var todo = new Todo(_user, (string)context.Data[CurrentStep.Name],
                                    (string)context.Data[CurrentStep.Description], result);

                                //добавляем задачу 
                                var n = await _todoService.AddAsync(todo, ct);

                                if (n == 1)
                                {
                                    //удаляем сообщение пользователя
                                    await botClient.DeleteMessage(chatId, messageId, ct);

                                    //отправляем сообщение
                                    message = await botClient.EditMessageText(chatId, (int)context.Data["MessageId"], 
                                        $"Задача {context.Data[CurrentStep.Name]} добавлена.", cancellationToken: ct,
                                        replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData
                                            ("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.Show).ToString())));

                                    return ScenarioResult.Completed;
                                }
                            }
                        }

                        //удаляем сообщение пользователя
                        await botClient.DeleteMessage(chatId, messageId, ct);
                        return ScenarioResult.Transition;
                    }
                    break;
            }

            return ScenarioResult.Transition;
        }

        private static class CurrentStep
        {
            public static string Name = nameof(Name);
            public static string Description = nameof(Description);
            public static string FinishedAt = nameof(FinishedAt);
        }
    }
}

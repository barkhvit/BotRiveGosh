using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Services.Interfaces;
using BotRiveGosh.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Views.Kpi
{
    //Kpi кассира - обновить
    public class UpdatekpiView : BaseView
    {
        private readonly InMemoryStorageService _inMemoryStorageService;
        private readonly IKpiService _kpiService;
        public UpdatekpiView(ITelegramBotClient botClient, 
            InMemoryStorageService inMemoryStorageService, IKpiService kpiService) : base(botClient)
        {
            _inMemoryStorageService = inMemoryStorageService;
            _kpiService = kpiService;
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage)
        {
            InitializeMessageInfo(update);

            //получаем список KPI для обновления
            var kpiForUpdate = await _inMemoryStorageService.GetKpiStorage(MessageInfo.GetTelegramUserId(update), ct);

            if(update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            if (kpiForUpdate.Count == 0)
            {
                await _botClient.EditMessageText(ChatId, MessageId, $"Нет данных для обновления.",
                    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString())),
                    cancellationToken: ct);
            }

            List<InlineKeyboardButton> buttons = new()
            {
                InlineKeyboardButton.WithCallbackData("Обновить",new CallBackDto(Dto_Objects.UpdatekpiView, Dto_Action.UpdateKpiConfirm).ToString()),
                InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString())
            };

            await _botClient.EditMessageText(ChatId, MessageId, $"Готово к обновлению {kpiForUpdate.Count} строк.",
                replyMarkup: new InlineKeyboardMarkup(buttons),
                cancellationToken: ct);
        }

        //подтверждения обновления таблицы KPI
        public async Task UpdateKpiConfirm(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            var kpiForUpdate = await _inMemoryStorageService.GetKpiStorage(MessageInfo.GetTelegramUserId(update), ct);
            if (kpiForUpdate != null)
            {
                var isUpdate = await _kpiService.UpdateKpiTable(kpiForUpdate.ToList(), ct);
                if (isUpdate)
                {
                    await _botClient.EditMessageText(chatId, messageId, "Таблица с результатами Kpi обновлена.",
                        cancellationToken: ct,
                        replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString())));
                }
            }

        }
    }
}

using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Views.Kpi
{
    public class MenuKpiView : BaseView
    {
        //меню KPI кассира
        //Dto_Objects.Kpi  Dto_Action.ShowMenu
        public MenuKpiView(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage)
        {
            InitializeMessageInfo(update);
            string text = "Выберите действие с Kpi:";
            List<InlineKeyboardButton[]> buttons = new();
            List<InlineKeyboardButton> row = new()
            {
                InlineKeyboardButton.WithCallbackData("Посмотреть результат",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowResultScenario).ToString()), //сценарий показать результат KPI
                InlineKeyboardButton.WithCallbackData("Обновить",new CallBackDto(Dto_Objects.UpdatekpiView, Dto_Action.UpdateKpi).ToString()),
            };
            List<InlineKeyboardButton> row2 = new()
            {
                InlineKeyboardButton.WithCallbackData("Условия",new CallBackDto(Dto_Objects.DetailPrizeView, Dto_Action.ShowKpiPrize).ToString()),
                InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString()),
            };
            buttons.Add(row.ToArray());
            buttons.Add(row2.ToArray());

            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    await _botClient.SendMessage(
                        ChatId,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(buttons),
                        cancellationToken: ct);
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    await _botClient.EditMessageText(
                        ChatId,
                        MessageId,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(buttons),
                        cancellationToken: ct);
                    break;
            }
        }
    }
}

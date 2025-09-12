using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers.Commands
{
    public class CommandsForKpi
    {
        private readonly IKpiResultService _kpiResultService;
        private readonly ITelegramBotClient _botClient;
        public CommandsForKpi(IKpiResultService kpiResultService, ITelegramBotClient botClient)
        {
            _kpiResultService = kpiResultService;
            _botClient = botClient;
        }

        //показать главное меню раздела KPI
        internal async Task ShowMenuKpi(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            string text = "Выберите действие с Kpi:";
            List<InlineKeyboardButton[]> buttons = new();
            List<InlineKeyboardButton> row = new()
            {
                InlineKeyboardButton.WithCallbackData("Результат",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowResult).ToString()), //сценарий показать результат KPI
                InlineKeyboardButton.WithCallbackData("Обновить",new CallBackDto(Dto_Objects.Update, Dto_Action.UpdateKpi).ToString()),
            };
            List<InlineKeyboardButton> row2 = new()
            {
                InlineKeyboardButton.WithCallbackData("Условия",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowRules).ToString()),
                InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString()),
            };
            buttons.Add(row.ToArray());
            buttons.Add(row2.ToArray());

            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    await _botClient.SendMessage(
                        chatId,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(buttons),
                        cancellationToken: ct);
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    await _botClient.EditMessageText(
                        chatId,
                        messageId,
                        text,
                        replyMarkup: new InlineKeyboardMarkup(buttons),
                        cancellationToken: ct);
                    break;
            }
        }

        
    }
}

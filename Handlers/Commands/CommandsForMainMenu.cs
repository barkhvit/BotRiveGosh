using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
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
    public class CommandsForMainMenu
    {
        private readonly ITelegramBotClient _botClient;

        public CommandsForMainMenu(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ShowMainMenu(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            List<List<InlineKeyboardButton>> buttons = new()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Kpi кассира",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowMenu).ToString())
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("О боте", new CallBackDto(Dto_Objects.AboutBot, Dto_Action.AboutBotShow).ToString())
                }
            };
            if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && messageType != MessageType.newMessage)
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                await _botClient.EditMessageText(chatId, messageId, "Выберите пункт меню:",
                    replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken:ct);
                return;
            }
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            await _botClient.SendMessage(chatId, "Выберите пункт меню:",
                    replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
        }

        internal async Task ShowAboutBot(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            string mesText = "Создатель: <a href=\"tg://user?id=1976535977\">Бархатов Виталий</a>\nДата: 06.09.2025\nВерсия: 1.01";
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                await _botClient.EditMessageText(chatId, messageId, mesText, cancellationToken: ct,
                    replyMarkup: InlineKeyboardButton.WithCallbackData(
                        "🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString()),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                return;
            }
            await _botClient.SendMessage(chatId, mesText, cancellationToken: ct,
                    replyMarkup: InlineKeyboardButton.WithCallbackData(
                        "🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString()),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}

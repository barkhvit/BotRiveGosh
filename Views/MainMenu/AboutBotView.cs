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

namespace BotRiveGosh.Views.MainMenu
{
    public class AboutBotView : BaseView
    {
        //информация о боте
        //Dto_Objects.AboutBotView Dto_Action.AboutBotShow

        public AboutBotView(ITelegramBotClient botClient) : base(botClient)
        {
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage)
        {
            InitializeMessageInfo(update);
            string sourceId = await Helpers.MessageInfo.GetUserProfileLinkAsync(1976535977, _botClient, ct);
            string mesText = $"Разработчик: {sourceId}\nДата: 06.09.2025\nВерсия: 1.01";
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                await _botClient.EditMessageText(ChatId, MessageId, mesText, cancellationToken: ct,
                    replyMarkup: InlineKeyboardButton.WithCallbackData(
                        "🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString()),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                return;
            }
            await _botClient.SendMessage(ChatId, mesText, cancellationToken: ct,
                    replyMarkup: InlineKeyboardButton.WithCallbackData(
                        "🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenu).ToString()),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}

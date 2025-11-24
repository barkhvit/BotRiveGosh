using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Keyboards;
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
    public class MainMenuView : BaseView
    {
        //Главное меню
        //Dto_Objects.MainMenuView Dto_Action.ShowMenu
        //Dto_Objects.MainMenuView Dto_Action.ShowMenuNewMessage
        public MainMenuView(ITelegramBotClient botClient) : base(botClient) { }

        public override async Task Show(Update update, CancellationToken ct, 
            MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            //получаем ChatId, UserId, MessageId, Text, User
            InitializeMessageInfo(update);

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && messageType != MessageType.newMessage)
            {
                if(update.CallbackQuery != null)
                {
                    await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                    await _botClient.EditMessageText(ChatId, MessageId, "Выберите пункт меню:",
                        replyMarkup: MainMenu(), cancellationToken: ct);
                    return;
                }
            }
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            await _botClient.SendMessage(ChatId, "Выберите пункт меню:",
                    replyMarkup: MainMenu(), cancellationToken: ct);
        }

        private static InlineKeyboardMarkup MainMenu()
        {
            List<List<InlineKeyboardButton>> buttons = new()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Премии ", new CallBackDto(Dto_Objects.AllPrizesView, Dto_Action.Show).ToString())
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Мои задачи ", new CallBackDto(Dto_Objects.TodoMenuView, Dto_Action.Show).ToString())
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("О боте", new CallBackDto(Dto_Objects.AboutBotView, Dto_Action.AboutBotShow).ToString())
                }
            };
            return new InlineKeyboardMarkup(buttons);
        }

    }
}

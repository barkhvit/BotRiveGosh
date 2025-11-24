using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotRiveGosh.Views
{
    public abstract class BaseView
    {
        public long ChatId { get; protected set; }
        public long UserId { get; protected set; }
        public int MessageId { get; protected set; }
        public User? User { get; protected set; }
        public string? Text { get; protected set; }

        protected readonly ITelegramBotClient _botClient;

        protected BaseView(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        protected (long chatId, long userId, int messageId, string? text, User user) GetMessageInfo(Update update)
        {
            return update switch
            {
                { Type: UpdateType.Message, Message: var msg }
                    => (msg.Chat.Id, msg.From.Id, msg.MessageId, msg.Text, msg.From),

                { Type: UpdateType.CallbackQuery, CallbackQuery: var cbq }
                    => (cbq.Message.Chat.Id, cbq.From.Id, cbq.Message.MessageId, cbq.Data, cbq.From),

                _ => throw new InvalidOperationException("Неизвестный тип сообщения от пользователя")
            };
        }

        protected void InitializeMessageInfo(Update update)
        {
            (ChatId, UserId, MessageId, Text, User) = GetMessageInfo(update);
        }

        public abstract Task Show(Update update, CancellationToken ct, 
            BotRiveGosh.Core.Common.Enums.MessageType messageType = Core.Common.Enums.MessageType.defaultMessage, 
            string inputDto = "");
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace BotRiveGosh.Helpers
{
    public static class MessageInfo
    {
        public static (long chatId, long userId, int messageId, string? text, User user) GetMessageInfo(Update update)
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

        public static string? GetUserName(Update update)
        {

            return update switch
            {
                { Type: UpdateType.Message, Message: var msg }
                    => msg.From.Username,

                { Type: UpdateType.CallbackQuery, CallbackQuery: var cbq }
                    => cbq.From.Username,

                _ => throw new InvalidOperationException("Неизвестный тип сообщения от пользователя")
            };
        }

        public static async Task<string?> GetUsernameByTelegramId(long telegramId, ITelegramBotClient botClient, CancellationToken ct)
        {
            Chat userChat = await botClient.GetChat(telegramId, ct);
            return userChat.Username;
        }

        public static long GetTelegramUserId(this Update update, long defaultValue = 0)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message?.From?.Id ?? defaultValue,
                UpdateType.CallbackQuery => update.CallbackQuery?.From?.Id ?? defaultValue,
                UpdateType.InlineQuery => update.InlineQuery?.From?.Id ?? defaultValue,
                UpdateType.ChosenInlineResult => update.ChosenInlineResult?.From?.Id ?? defaultValue,
                UpdateType.EditedMessage => update.EditedMessage?.From?.Id ?? defaultValue,
                UpdateType.ChannelPost => update.ChannelPost?.From?.Id ?? defaultValue,
                UpdateType.EditedChannelPost => update.EditedChannelPost?.From?.Id ?? defaultValue,
                UpdateType.ShippingQuery => update.ShippingQuery?.From?.Id ?? defaultValue,
                UpdateType.PreCheckoutQuery => update.PreCheckoutQuery?.From?.Id ?? defaultValue,
                UpdateType.PollAnswer => update.PollAnswer?.User?.Id ?? defaultValue,
                UpdateType.MyChatMember => update.MyChatMember?.From?.Id ?? defaultValue,
                UpdateType.ChatMember => update.ChatMember?.From?.Id ?? defaultValue,
                UpdateType.ChatJoinRequest => update.ChatJoinRequest?.From?.Id ?? defaultValue,
                _ => defaultValue
            };
        }
        public static async Task<string> GetUserProfileLinkAsync(long telegramId, ITelegramBotClient botClient, CancellationToken ct)
        {
            try
            {
                var chat = await botClient.GetChat(telegramId, cancellationToken: ct);
                string displayName = string.IsNullOrEmpty(chat.FirstName) ? "Разработчик" : $"{chat.FirstName} {chat.LastName}";

                if (!string.IsNullOrEmpty(chat.Username))
                {
                    // Если у пользователя есть username
                    return $"<a href=\"https://t.me/{chat.Username}\">{displayName}</a>";
                }
                else
                {
                    // Если username нет - используем deep link
                    return $"<a href=\"tg://user?id={telegramId}\">{displayName}</a>";
                }
            }
            catch
            {
                return "Пользователь не найден";
            }
        }
    }
}

using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Commands;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers
{
    public class DocumentUpdateHandler : IUpdateHandler
    {
        private readonly CommandsForMainMenu _commandsForMainMenu;
        private readonly InputFileService _inputFileService;
        public DocumentUpdateHandler(CommandsForMainMenu commandsForMainMenu, InputFileService inputFileService)
        {
            _commandsForMainMenu = commandsForMainMenu;
            _inputFileService = inputFileService;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            //пробуем сохранить файл и получаем ответ от бота
            var textMessage = await _inputFileService.ReadAndSafeFileAsync(update, ct);

            await botClient.SendMessage(chatId, textMessage, cancellationToken: ct);
            await _commandsForMainMenu.ShowMainMenu(update, ct, MessageType.newMessage);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        

    }
}

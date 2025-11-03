using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services;
using BotRiveGosh.Views.MainMenu;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers
{
    public class DocumentUpdateHandler : IUpdateHandler
    {
        private readonly InputFileService _inputFileService;
        private readonly MainMenuView _mainMenuView;
        public DocumentUpdateHandler(InputFileService inputFileService,
            MainMenuView mainMenuView)
        {
            _inputFileService = inputFileService;
            _mainMenuView = mainMenuView;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            //пробуем сохранить файл и получаем ответ от бота
            var textMessage = await _inputFileService.ReadAndSafeFileAsync(update, ct);

            await botClient.SendMessage(chatId, textMessage, cancellationToken: ct);
            await _mainMenuView.Show(update, ct, MessageType.newMessage);
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        

    }
}

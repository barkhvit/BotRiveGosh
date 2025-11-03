using BotRiveGosh.Helpers;
using BotRiveGosh.Views.Kpi;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotRiveGosh.Handlers
{
    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly MenuKpiView _menuKpiView;
        public MessageUpdateHandler(MenuKpiView menuKpiView)
        {
            _menuKpiView = menuKpiView;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            try
            {
                switch (Text)
                {
                    case "/kpi": await _menuKpiView.Show(update, ct); break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        
    }
}

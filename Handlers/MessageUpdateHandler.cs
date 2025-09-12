using BotRiveGosh.Handlers.Commands;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotRiveGosh.Handlers
{
    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly CommandsForKpi _commandsForKpi;
        public MessageUpdateHandler(CommandsForKpi commandsForKpi)
        {
            _commandsForKpi = commandsForKpi;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            try
            {
                switch (Text)
                {
                    case "/kpi": await _commandsForKpi.ShowMenuKpi(update, ct); break;
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

using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using BotRiveGosh.Views.Kpi;
using BotRiveGosh.Views.MainMenu;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace BotRiveGosh.Handlers
{
    public class MessageUpdateHandler : IUpdateHandler
    {
        private readonly PrizeMenuView _prizeMenuView;
        private readonly IPrizesService _prizesService;
        public MessageUpdateHandler(PrizeMenuView prizeMenuView, IPrizesService prizesService)
        {
            _prizeMenuView = prizeMenuView;
            _prizesService = prizesService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            try
            {
                switch (Text)
                {
                    case "/kpi":
                        var prize = await _prizesService.GetByNameAsync(Dto_NamePrizes.KpiCashier, ct);
                        if (prize != null)
                        {
                            string inputDto = new CallBackDto(Dto_Objects.PrizeMenuView, Dto_Action.Show, _id:prize.Id).ToString();
                            await _prizeMenuView.Show(update, ct, inputDto: inputDto);
                        }
                        break;
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

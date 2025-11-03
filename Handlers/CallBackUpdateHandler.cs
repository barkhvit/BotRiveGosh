using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
using BotRiveGosh.Views.Kpi;
using BotRiveGosh.Views.MainMenu;
using BotRiveGosh.Views.NewUser;
using BotRiveGosh.Views.Prize;
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
    public class CallBackUpdateHandler : IUpdateHandler
    {
        private readonly MainMenuView _mainMenuView;
        private readonly AboutBotView _aboutBotView;
        private readonly MenuKpiView _menuKpiView;
        private readonly GiveRequestView _giveRequestView;
        private readonly UpdatekpiView _updatekpiView;
        private readonly AllPrizesView _allPrizesView;
        private readonly DetailPrizeView _detailPrizeView;
        public CallBackUpdateHandler(MainMenuView mainMenuView,
            AboutBotView aboutBotView, MenuKpiView menuKpiView, 
            GiveRequestView giveRequestView, UpdatekpiView updatekpiView,
            AllPrizesView allPrizesView, DetailPrizeView detailPrizeView)
        {
            _mainMenuView = mainMenuView;
            _aboutBotView = aboutBotView;
            _menuKpiView = menuKpiView;
            _giveRequestView = giveRequestView;
            _updatekpiView = updatekpiView;
            _allPrizesView = allPrizesView;
            _detailPrizeView = detailPrizeView;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            CallBackDto callBackDto = CallBackDto.FromString(Text);
            switch (callBackDto.Object)
            {
                case nameof(Dto_Objects.MainMenuView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.ShowMenu): await _mainMenuView.Show(update, ct); break;
                        case nameof(Dto_Action.ShowMenuNewMessage): await _mainMenuView.Show(update, ct, MessageType.newMessage); break;
                        case nameof(Dto_Action.Show): await _mainMenuView.Show(update, ct); break;
                    }
                    break;

                case nameof(Dto_Objects.MenuKpiView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.Show): await _menuKpiView.Show(update, ct); break;
                    }break;

                case nameof(Dto_Objects.DetailPrizeView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.Show): await _detailPrizeView.Show(update, ct); break;
                        case nameof(Dto_Action.ShowKpiPrize): await _detailPrizeView.ShowKpiPrize(update, ct); break;
                    }
                    break;

                case nameof(Dto_Objects.AllPrizesView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.Show): await _allPrizesView.Show(update, ct); break;
                    }break;

                case nameof(Dto_Objects.Kpi):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.ShowMenu): await _menuKpiView.Show(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.AboutBotView):
                    switch (callBackDto.Action) 
                    {
                        case nameof(Dto_Action.AboutBotShow): await _aboutBotView.Show(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.UpdatekpiView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.UpdateKpi): await _updatekpiView.Show(update, ct); break;
                        case nameof(Dto_Action.UpdateKpiConfirm): await _updatekpiView.UpdateKpiConfirm(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.Reg):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.RegRequest): await _giveRequestView.Show(update, ct); break; //получили запрос на доступ
                        case nameof(Dto_Action.RegApprove): await _giveRequestView.GiveRequest(update, ct); break; //дать доступ
                        case nameof(Dto_Action.RegReject): await _giveRequestView.RejectReg(update, ct); break; //отклонить доступ
                    }
                    break;
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

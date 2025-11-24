using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Helpers;
using BotRiveGosh.Views.Kpi;
using BotRiveGosh.Views.MainMenu;
using BotRiveGosh.Views.NewUser;
using BotRiveGosh.Views.Prize;
using BotRiveGosh.Views.ToDo;
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
        private readonly AboutPrizeView _detailPrizeView;
        private readonly PrizeMenuView _prizeMenuView;
        private readonly TodoMenuView _todoMenuView;
        private readonly TodoListView _todoListView;
        private readonly TodoEditView _todoEditView;
        private readonly TodoDetailView _todoDetailView;
        private readonly TodoDoneView _todoDoneView;

        public CallBackUpdateHandler(MainMenuView mainMenuView,
            AboutBotView aboutBotView, MenuKpiView menuKpiView, 
            GiveRequestView giveRequestView, UpdatekpiView updatekpiView,
            AllPrizesView allPrizesView, AboutPrizeView detailPrizeView,
            PrizeMenuView prizeMenuView, TodoMenuView todoMenuView,
            TodoListView todoListView, TodoEditView todoEditView,
            TodoDetailView todoDetailView, TodoDoneView todoDoneView)
        {
            _mainMenuView = mainMenuView;
            _aboutBotView = aboutBotView;
            _menuKpiView = menuKpiView;
            _giveRequestView = giveRequestView;
            _updatekpiView = updatekpiView;
            _allPrizesView = allPrizesView;
            _detailPrizeView = detailPrizeView;
            _prizeMenuView = prizeMenuView;
            _todoMenuView = todoMenuView;
            _todoListView = todoListView;
            _todoEditView = todoEditView;
            _todoDetailView = todoDetailView;
            _todoDoneView = todoDoneView;
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

                case nameof(Dto_Objects.PrizeMenuView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.Show): await _prizeMenuView.Show(update, ct); break;
                    }
                    break;

                case nameof(Dto_Objects.DetailPrizeView):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.Show): await _detailPrizeView.Show(update, ct); break;
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

                //TODO -----------------------------------------------------
                //menu
                case nameof(Dto_Objects.TodoMenuView):await _todoMenuView.Show(update, ct); break;

                //список задач
                case nameof(Dto_Objects.TodoListView): await _todoListView.Show(update, ct); break;

                //редактировать - выводится список задач в виде кнопок
                case nameof(Dto_Objects.TodoEditView):await _todoEditView.Show(update, ct); break;

                //детальный показ - выводит детально задачу с кнопками редактировать, завершить
                case nameof(Dto_Objects.TodoDetailView): await _todoDetailView.Show(update, ct); break;

                //вы действительно хотите сделать задачу завершенной
                case nameof(Dto_Objects.TodoDoneView): await _todoDoneView.Show(update, ct); break;
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

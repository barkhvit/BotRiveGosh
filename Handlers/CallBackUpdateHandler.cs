using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Commands;
using BotRiveGosh.Helpers;
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
        private readonly CommandsForKpi _commandsForKpi;
        private readonly CommandsForMainMenu _commandsForMainMenu;
        private readonly CommandsForUpdate _commandsForUpdate;
        private readonly CommandsForRegistration _commandsForRegistration;
        public CallBackUpdateHandler(CommandsForKpi commandsForKpi, CommandsForMainMenu commandsForMainMenu,
            CommandsForUpdate commandsForUpdate, CommandsForRegistration commandsForRegistration)
        {
            _commandsForKpi = commandsForKpi;
            _commandsForMainMenu = commandsForMainMenu;
            _commandsForUpdate = commandsForUpdate;
            _commandsForRegistration = commandsForRegistration;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            CallBackDto callBackDto = CallBackDto.FromString(Text);
            switch (callBackDto.Object)
            {
                case nameof(Dto_Objects.MainMenu):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.ShowMenu): await _commandsForMainMenu.ShowMainMenu(update, ct); break;
                        case nameof(Dto_Action.ShowMenuNewMessage): await _commandsForMainMenu.ShowMainMenu(update, ct, MessageType.newMessage); break;
                    }
                    break;
                case nameof(Dto_Objects.Kpi):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.ShowMenu): await _commandsForKpi.ShowMenuKpi(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.AboutBot):
                    switch (callBackDto.Action) 
                    {
                        case nameof(Dto_Action.AboutBotShow): await _commandsForMainMenu.ShowAboutBot(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.Update):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.UpdateKpi): await _commandsForUpdate.UpdateKpi(update, ct); break;
                        case nameof(Dto_Action.UpdateKpiConfirm): await _commandsForUpdate.UpdateKpiConfirm(update, ct); break;
                    }
                    break;
                case nameof(Dto_Objects.Reg):
                    switch (callBackDto.Action)
                    {
                        case nameof(Dto_Action.RegRequest): await _commandsForRegistration.SendRegRequest(update, ct); break; //получили запрос на доступ
                        case nameof(Dto_Action.RegApprove): await _commandsForRegistration.ApprovedReg(update, ct); break; //дать доступ
                        case nameof(Dto_Action.RegReject): await _commandsForRegistration.RejectReg(update, ct); break; //отклонить доступ
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

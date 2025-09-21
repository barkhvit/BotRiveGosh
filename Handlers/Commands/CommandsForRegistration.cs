using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Handlers.Keyboards;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Handlers.Commands
{
    public class CommandsForRegistration
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;

        public CommandsForRegistration(ITelegramBotClient botClient, IUserService userService)
        {
            _botClient = botClient;
            _userService = userService;
        }

        //получили запрос от пользователя на доступ, отправляем запрос на доступ администраторам
        internal async Task SendRegRequest(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            //отправляем сообщение пользователю, что заявка на доступ отправлена
            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            await _botClient.EditMessageText(chatId, messageId, "Заявка на доступ отправлена.", cancellationToken:ct);

            var dto = CallBackDto.FromString(Text);

            //получаем список администраторов
            string? strAdmins = ConfigurationManager.AppSettings["AdminTelegramId"];
            if (strAdmins != null)
            {
                long[] adminsId = strAdmins.Split(',')
                .Select(long.Parse)
                .ToArray();
                //отправляем админам сообщение с Id пользователя в DTO
                foreach(var id in adminsId)
                {
                    List<InlineKeyboardButton> buttons = new()
                    {
                        InlineKeyboardButton.WithCallbackData("✅Дать доступ",new CallBackDto(Dto_Objects.Reg, Dto_Action.RegApprove,dto.Id).ToString()),
                        InlineKeyboardButton.WithCallbackData("❌Отклонить",new CallBackDto(Dto_Objects.Reg, Dto_Action.RegReject,dto.Id).ToString())
                    };
                    await _botClient.SendMessage(id,
                        $"Пользователь {user.FirstName} {user.LastName}({user.Username}) запросил доступ в бот.",
                        cancellationToken:ct, replyMarkup:new InlineKeyboardMarkup(buttons));
                }
            }
        }

        //админ нажал - дать доступ пользователю
        internal async Task ApprovedReg(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            bool result = false;
            //получаем dto
            var dto = CallBackDto.FromString(Text);
            
            if (dto.Id != null)
            {
                //получаем пользователя
                var inputUser = await _userService.GetUserByIdAsync((Guid)dto.Id, ct);
                //меняем доступ пользователя, обновляем базу
                if (inputUser != null)
                {
                    inputUser.AccessAllowed = true;
                    result = await _userService.UpdateAsync(inputUser, ct);
                    //отправляем сообщение пользователю и админу
                    if (result)
                    {
                        await _botClient.SendMessage(inputUser.TelegramId, "Доступ предоставлен", cancellationToken: ct,
                            replyMarkup: GetKeybords.MainMenu());
                        await _botClient.SendMessage(chatId, "Доступ предоставлен", cancellationToken: ct);
                    }
                    else
                    {
                        await _botClient.SendMessage(chatId, "Что-то пошло не так(.", cancellationToken: ct);
                    }
                }
            }

            
        }

        //админ нажал - отклонить доступ пользователю
        internal async Task RejectReg(Update update, CancellationToken ct)
        {
            //получаем dto
            //получаем пользователя
            //отправляем сообщение пользователю
            throw new NotImplementedException();
        }
    }
}

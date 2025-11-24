using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Handlers.Keyboards;
using BotRiveGosh.Services;
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
using BotRiveGosh.Helpers;

namespace BotRiveGosh.Views.NewUser
{
    public class GiveRequestView : BaseView
    {
        //Письмо админам с просьбой дать доступ
        //Dto_Objects.Reg  Dto_Action.RegRequest  Id пользователя, который запрашивает доступ

        private readonly IUserService _userService;

        public GiveRequestView(ITelegramBotClient botClient, IUserService userService) : base(botClient)
        {
            _userService = userService;
        }

        public override async Task Show(Update update, CancellationToken ct, 
            MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

            //отправляем сообщение пользователю, что заявка на доступ отправлена
            if(update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            await _botClient.EditMessageText(ChatId, MessageId, "Заявка на доступ отправлена.", cancellationToken: ct);
            var userSource = await MessageInfo.GetUserProfileLinkAsync(ChatId, _botClient, ct);

            CallBackDto dto = new("");
            if(Text != null) dto = CallBackDto.FromString(Text);

            //получаем список администраторов
            string? strAdmins = ConfigurationManager.AppSettings["AdminTelegramId"];
            if (strAdmins != null)
            {
                long[] adminsId = strAdmins.Split(',')
                .Select(long.Parse)
                .ToArray();

                //отправляем админам сообщение с Id пользователя в DTO
                foreach (var id in adminsId)
                {
                    List<InlineKeyboardButton> buttons = new()
                    {
                        InlineKeyboardButton.WithCallbackData("✅Дать доступ",new CallBackDto(Dto_Objects.Reg, Dto_Action.RegApprove,dto.Id).ToString()),
                        InlineKeyboardButton.WithCallbackData("❌Отклонить",new CallBackDto(Dto_Objects.Reg, Dto_Action.RegReject,dto.Id).ToString())
                    };
                    await _botClient.SendMessage(id,
                        $"Пользователь {User?.FirstName} {User?.LastName}({User?.Username}) запросил доступ в бот. \n {userSource} ",
                        cancellationToken: ct, replyMarkup: new InlineKeyboardMarkup(buttons),
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                }
            }
        }

        //кнопка ✅Дать доступ
        public async Task GiveRequest(Update update, CancellationToken ct)
        {
            InitializeMessageInfo(update);

            if(update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            bool result = false;

            //получаем dto
            CallBackDto dto = new("");
            if(Text != null) dto = CallBackDto.FromString(Text);

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
                        await _botClient.SendMessage(ChatId, "Доступ предоставлен", cancellationToken: ct);
                    }
                    else
                    {
                        await _botClient.SendMessage(ChatId, "Что-то пошло не так(.", cancellationToken: ct);
                    }
                }
            }
        }

        //кнопка ❌Отклонить
        public async Task RejectReg(Update update, CancellationToken ct)
        {
            InitializeMessageInfo(update);
            if (update.CallbackQuery != null) await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

            //получаем dto
            CallBackDto dto = new("");
            if (Text != null) dto = CallBackDto.FromString(Text);

            if(dto.Id != null)
            {
                //получаем пользователя, который запрашивал доступ
                var user = await _userService.GetUserByIdAsync((Guid)dto.Id, ct);

                if(user != null)
                {
                    //отправляем сообщение пользователю и админу
                    //пользователю:
                    await _botClient.SendMessage(user.TelegramId, "В доступе отказано.", cancellationToken: ct);
                    //админу:
                    await _botClient.SendMessage(ChatId, "В доступе отказано.", cancellationToken: ct);
                }
            }
        }
    }
}

using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Views.Prize
{
    public class AboutPrizeView : BaseView
    {
        private readonly IPrizesService _prizesService;
        public AboutPrizeView(ITelegramBotClient botClient, IPrizesService prizesService) : base(botClient)
        {
            _prizesService = prizesService;
        }

        public override async Task Show(Update update, CancellationToken ct, 
            MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

            //получаем Dto
            CallBackDto dto = new("");

            if (Text != null)
            {
                dto = CallBackDto.FromString(Text);

                //получаем премию
                if (dto.Id != null)
                {
                    var prize = await _prizesService.GetByIdAsync((Guid)dto.Id, ct);

                    //правим сообщение
                    if (update.CallbackQuery != null && prize != null)
                    {
                        //кнопка назад
                        var backButton = InlineKeyboardButton.WithCallbackData("⬅️ назад", new CallBackDto(Dto_Objects.PrizeMenuView, Dto_Action.Show, _id: prize.Id).ToString());
                        
                        await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                        await _botClient.EditMessageText(ChatId, MessageId, $"{prize?.Description}",
                            cancellationToken: ct,
                            replyMarkup: new InlineKeyboardMarkup(backButton));
                    }
                }
            }
        }

    }
}

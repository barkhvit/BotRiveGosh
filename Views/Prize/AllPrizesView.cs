using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
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
    public class AllPrizesView : BaseView
    {
        private readonly IPrizesService _prizesService;
        public AllPrizesView(ITelegramBotClient botClient, IPrizesService prizesService) : base(botClient)
        {
            _prizesService = prizesService;
        }

        public override async Task Show(Update update, CancellationToken ct, MessageType messageType = MessageType.defaultMessage)
        {
            InitializeMessageInfo(update);

            //получить все премии
            var prizes = await _prizesService.GetAllAsync(ct);

            if (update.CallbackQuery?.Id != null)
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                await _botClient.EditMessageText(ChatId, MessageId, "Выберите премию:",
                    replyMarkup: GetPrizesKeyboard((List<Prizes>)prizes),
                    cancellationToken: ct);
            }
        }

        private InlineKeyboardMarkup GetPrizesKeyboard(List<Prizes> prizes)
        {
            var prizeButtons = prizes.Select(p => InlineKeyboardButton.WithCallbackData(
                $"{p.Name}",
                new CallBackDto(Dto_Objects.DetailPrizeView, Dto_Action.Show, _id: p.Id).ToString()))
                .ToList();

            // Группируем кнопки по две в ряду
            var rows = new List<InlineKeyboardButton[]>();

            for (int i = 0; i < prizeButtons.Count; i += 2)
            {
                // Берем две кнопки или одну, если это последняя непарная кнопка
                var row = prizeButtons.Skip(i).Take(2).ToArray();
                rows.Add(row);
            }

            // Добавляем кнопку "назад" в отдельный ряд
            rows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "⬅️ назад",
                    new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.Show).ToString())
            });

            return new InlineKeyboardMarkup(rows);
        }
    }
}

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
using static System.Net.Mime.MediaTypeNames;

namespace BotRiveGosh.Views.MainMenu
{
    //меню премии
    public class PrizeMenuView : BaseView
    {
        private readonly IPrizesService _prizesService;
        public PrizeMenuView(ITelegramBotClient botClient, IPrizesService prizesService) : base(botClient)
        {
            _prizesService = prizesService;
        }

        public override async Task Show(Update update, CancellationToken ct, 
            MessageType messageType = MessageType.defaultMessage, string inputDto = "")
        {
            InitializeMessageInfo(update);

            //в зависимости от премии, делаем клавиатуру
            var dto = new CallBackDto("");
            if (Text != null)
            {
                if (inputDto == "")
                    dto = CallBackDto.FromString(Text);
                else dto = CallBackDto.FromString(inputDto);

                if (dto.Id != null)
                {
                    //получаем премию
                    var prize = await _prizesService.GetByIdAsync((Guid)dto.Id, ct);

                    //получаем клавиатуру и текст сообщения в зависимости от премии
                    InlineKeyboardMarkup buttons = new();
                    string text = "";

                    if (prize != null)
                    {
                        switch (prize.Name)
                        {
                            //премия kpi кассира
                            case Dto_NamePrizes.KpiCashier:
                                buttons = await GetKeybordForKpiCashier(ct);
                                text = "Премия Kpi кассира";
                                break;
                            //премия структура продаж
                            case Dto_NamePrizes.StrSales:
                                buttons = await GetKeyboardForStrSales(ct);
                                text = "Премия Структура продаж";
                                break;
                        }

                        //отправляем или правим сообщение
                        switch (update.Type)
                        {
                            case Telegram.Bot.Types.Enums.UpdateType.Message:
                                await _botClient.SendMessage(ChatId, text, replyMarkup: buttons, cancellationToken: ct);
                                break;
                            case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                                await _botClient.EditMessageText(ChatId, MessageId, text, replyMarkup: buttons, cancellationToken: ct);
                                break;
                        }
                    }
                }
            }
        }

        private async Task<InlineKeyboardMarkup> GetKeyboardForStrSales(CancellationToken ct)
        {
            //id премии структура продаж
            var prize = await _prizesService.GetByNameAsync(Dto_NamePrizes.StrSales, ct);
            if (prize == null) throw new ArgumentException("Ошибка в получении prize в MenuKpiView.GetKeyboardForStrSales");

            var buttons = new List<List<InlineKeyboardButton>>()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Условия",new CallBackDto(Dto_Objects.DetailPrizeView, Dto_Action.Show, _id: prize?.Id).ToString()),
                    InlineKeyboardButton.WithCallbackData("⬅️ назад",new CallBackDto(Dto_Objects.AllPrizesView, Dto_Action.Show).ToString()),
                }
            };

            return new InlineKeyboardMarkup(buttons);

        }

        private async Task<InlineKeyboardMarkup> GetKeybordForKpiCashier(CancellationToken ct)
        {
            //id премии kpi кассира
            var prize = await _prizesService.GetByNameAsync(Dto_NamePrizes.KpiCashier, ct);
            if (prize == null) throw new ArgumentException("Ошибка в получении prize в MenuKpiView.GetKeybordForKpiCashier");

            var buttons = new List<List<InlineKeyboardButton>>()
            {
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Посмотреть результат",new CallBackDto(Dto_Objects.Kpi, Dto_Action.ShowResultScenario).ToString()), //сценарий показать результат KPI
                    InlineKeyboardButton.WithCallbackData("Обновить",new CallBackDto(Dto_Objects.UpdatekpiView, Dto_Action.UpdateKpi).ToString())
                },
                new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Условия",new CallBackDto(Dto_Objects.DetailPrizeView, Dto_Action.Show, _id: prize?.Id).ToString()),
                    InlineKeyboardButton.WithCallbackData("⬅️ назад",new CallBackDto(Dto_Objects.AllPrizesView, Dto_Action.Show).ToString()),
                }
            };

            return new InlineKeyboardMarkup(buttons);
        }
    }
}

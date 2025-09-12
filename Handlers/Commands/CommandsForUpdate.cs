using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Services;
using BotRiveGosh.Helpers;
using Telegram.Bot.Types.ReplyMarkups;
using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Services.Interfaces;

namespace BotRiveGosh.Handlers.Commands
{
    public class CommandsForUpdate
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InMemoryStorageService _inMemoryStorageService;
        private readonly IKpiService _kpiService;
        public CommandsForUpdate(ITelegramBotClient botClient, InMemoryStorageService inMemoryStorageService, IKpiService kpiService)
        {
            _botClient = botClient;
            _inMemoryStorageService = inMemoryStorageService;
            _kpiService = kpiService;
        }

        //нажатие на ОБНОВИТЬ kpi, получаем список kpi из файла, выводим кол-во строк и просим подтвердить обновление
        internal async Task UpdateKpi(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);

            //получаем список KPI для обновления
            var kpiForUpdate = await _inMemoryStorageService.GetKpiStorage(MessageInfo.GetTelegramUserId(update), ct);

            if(kpiForUpdate.Count == 0)
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                await _botClient.EditMessageText(chatId, messageId, $"Нет данных для обновления.",
                    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString())),
                    cancellationToken: ct);
            }

            List<InlineKeyboardButton> buttons = new()
            {
                InlineKeyboardButton.WithCallbackData("Обновить",new CallBackDto(Dto_Objects.Update, Dto_Action.UpdateKpiConfirm).ToString()),
                InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString())
            };

            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
            await _botClient.EditMessageText(chatId,messageId, $"Готово к обновлению {kpiForUpdate.Count} строк.",
                replyMarkup: new InlineKeyboardMarkup(buttons),
                cancellationToken:ct);
            
        }

        //подтверждения обновления таблицы KPI
        internal async Task UpdateKpiConfirm(Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            var kpiForUpdate = await _inMemoryStorageService.GetKpiStorage(MessageInfo.GetTelegramUserId(update), ct);
            if (kpiForUpdate != null)
            {
                var isUpdate = await _kpiService.UpdateKpiTable(kpiForUpdate.ToList(), ct);
                if (isUpdate)
                {
                    await _botClient.EditMessageText(chatId, messageId, "Таблица с результатами Kpi обновлена.",
                        cancellationToken: ct,
                        replyMarkup: new InlineKeyboardMarkup(
                            InlineKeyboardButton.WithCallbackData("🏠 Главное меню", new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenu).ToString())));
                }
            }
            
        }
    }
}

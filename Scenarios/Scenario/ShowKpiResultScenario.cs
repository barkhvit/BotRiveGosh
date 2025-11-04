using BotRiveGosh.Core.DTOs;
using BotRiveGosh.Core.Entities;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotRiveGosh.Scenarios.Scenario
{
    public class ShowKpiResultScenario : IScenario
    {
        private readonly IKpiResultService _kpiResultService;
        public ShowKpiResultScenario(IKpiResultService kpiResultService)
        {
            _kpiResultService = kpiResultService;
        }
        public bool CanHandle(ScenarioType scenarioType)
        {
            return scenarioType == ScenarioType.ShowKpiResult;
        }

        public async Task<ScenarioResult> HandleScenarioAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            var lastDateUpdate = await _kpiResultService.GetLastDateUpdate(ct);
            switch (context.CurrentStep)
            {
                case null:
                    if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                    {
                        if(update.CallbackQuery!=null) await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

                        string lastDateStr = lastDateUpdate == null ? "нет данных для kpi" : $"{lastDateUpdate.ToString()}";
                        await botClient.EditMessageText(chatId, messageId, 
                            $"Последнее обновление: {lastDateStr}\nВведите фамилию сотрудника или нажмите ОТМЕНА:", cancellationToken: ct,
                            replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                        context.CurrentStep = "WaitingName";
                    }
                    return ScenarioResult.Transition;
                case "WaitingName":
                    if (String.IsNullOrEmpty(Text))
                    {
                        await botClient.SendMessage(chatId, "Введите фамилию сотрудника или нажмите ОТМЕНА:", cancellationToken: ct,
                            replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                        return ScenarioResult.Transition;
                    }

                    //получаем результат 
                    var result = await _kpiResultService.GetByNameAsync(Text, ct);

                    //помещаем результат в хранилище
                    context.Data["result"] = result;

                    //если результатов более 30, то не выводим результат
                    if (result.Count > 30)
                    {
                        await botClient.SendMessage(chatId, "Результатов слишком много, введите более точнее:", cancellationToken: ct,
                            replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                        return ScenarioResult.Transition;
                    }

                    //проверяем сколько уникальных месяцев в результате, если больше 2х, то запрашиваем у пользователя месяц
                    var countOfMonth = result.Select(r => r.Month).Distinct().ToList();
                    if(countOfMonth.Count > 1)
                    {
                        List<List<InlineKeyboardButton>> monthButtons = new();
                        monthButtons.Add(result.Select(r => InlineKeyboardButton.WithCallbackData($"{r.Month}", $"{r.Month}")).ToList());
                        monthButtons.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData("Отмена", "cancel") });

                        await botClient.SendMessage(chatId, "Выберите месяц:", cancellationToken: ct, 
                            replyMarkup:new InlineKeyboardMarkup(monthButtons));
                        context.CurrentStep = "Month";

                        return ScenarioResult.Transition;
                    }

                    //если месяцев 1 и результатов более 10, то не выводим результат
                    if (result.Count > 10)
                    {
                        await botClient.SendMessage(chatId, "Результатов слишком много, введите более точнее:", cancellationToken: ct,
                            replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                        return ScenarioResult.Transition;
                    }

                    //если месяц только 1, то отправвляем результат
                    await SendResult(update, (List<KpiResult>)result, botClient, ct);
                    return ScenarioResult.Completed;

                case "Month":
                    //пользователь выбрал месяц, за который нужен результат
                    if (update.CallbackQuery != null) await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);

                    string month = "";
                    if(Text != null)
                    {
                        month = Text;
                        var resultKpi = (List<KpiResult>)context.Data["result"];
                        //вытаскиваем только результаты с нужным месяцем
                        var finalResult = resultKpi.Where(r => r.Month == month).ToList();

                        if (finalResult.Count > 10)
                        {
                            await botClient.SendMessage(chatId, "Результатов слишком много, введите более точнее:", cancellationToken: ct,
                                replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                            return ScenarioResult.Transition;
                        }

                        await SendResult(update, finalResult, botClient, ct);
                        return ScenarioResult.Completed;
                    }
                    return ScenarioResult.Transition;
            }
            return ScenarioResult.Transition;
        }

        private async Task SendResult(Update update, List<KpiResult> result, ITelegramBotClient botClient, CancellationToken ct)
        {
            var (chatId, userId, messageId, Text, user) = MessageInfo.GetMessageInfo(update);
            List<List<InlineKeyboardButton>> buttons = new()
            {
                new()
                {
                    InlineKeyboardButton.WithCallbackData("Смотреть еще", new CallBackDto(Dto_Objects.Kpi,Dto_Action.ShowResultScenario).ToString()),
                    InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenuNewMessage).ToString())
                }
            };

            if(update.CallbackQuery != null)
            {
                await botClient.EditMessageText(chatId, messageId, GetResult((List<KpiResult>)result), cancellationToken: ct,
                replyMarkup: new InlineKeyboardMarkup(buttons));
                return;
            }

            await botClient.SendMessage(chatId, GetResult((List<KpiResult>)result), cancellationToken: ct,
                replyMarkup: new InlineKeyboardMarkup(buttons));
        }

        private string GetResult(List<KpiResult> list)
        {
            if (list.Count == 0) return "Результат не найден.";
            string text = "";
            foreach(var l in list)
            {
                text += $"{l.Name} - {l.Month}\n{l.Shop} (кат {l.Category})\nВсего чеков: {l.TotalChecks} Результат: {l.Result}\n" +
                    $"Кассир: {CalculateKpi(l).Cashier} | Продавец: {CalculateKpi(l).Sales}\n-------------------\n";
            }
            return text;
        }

        private KpiCalculate CalculateKpi(KpiResult kpiResult)
        {
            var result = new KpiCalculate() { Cashier = 5000, Sales = 5000 };
            int salesLimit = 250;
            int cashierLimit = 250;
            if (kpiResult.Category == "C") salesLimit = 200;
            if (kpiResult.Result > 7.0m)
            {
                result.Sales = 0; result.Cashier = 0;
                return result;
            }
            
            if (kpiResult.Result > 5.0m)
            {
                result.Sales = 4000; result.Cashier = 4000;
            }

            if (result.Cashier == 5000 && kpiResult.TotalChecks > 800) result.Cashier = 7000;

            //проверка на лимит
            if (kpiResult.TotalChecks < salesLimit) result.Sales = 0;
            if (kpiResult.TotalChecks < cashierLimit) result.Cashier = 0;

            return result;
        }

        private class KpiCalculate
        {
            public int Sales { get; set; }
            public int Cashier { get; set; } 
        }

    }
}

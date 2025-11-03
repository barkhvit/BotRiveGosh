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
            switch (context.CurrentStep)
            {
                case null:
                    if(update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
                    {
                        if(update.CallbackQuery!=null) await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
                        await botClient.EditMessageText(chatId, messageId, "Введите фамилию сотрудника или нажмите ОТМЕНА:", cancellationToken: ct,
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

                    var result = await _kpiResultService.GetByNameAsync(Text, ct);
                    if (result.Count > 5)
                    {
                        await botClient.SendMessage(chatId, "Результатов слишком много, введите более точнее:", cancellationToken: ct,
                            replyMarkup: InlineKeyboardButton.WithCallbackData("Отмена", "cancel"));
                        return ScenarioResult.Transition;
                    }

                    List<List<InlineKeyboardButton>> buttons = new()
                    {
                        new()
                        {
                            InlineKeyboardButton.WithCallbackData("Смотреть еще", new CallBackDto(Dto_Objects.Kpi,Dto_Action.ShowResult).ToString()),
                            InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenuView, Dto_Action.ShowMenuNewMessage).ToString())
                        }
                    };

                    await botClient.SendMessage(chatId, GetResult((List<KpiResult>)result), cancellationToken: ct,
                        replyMarkup: new InlineKeyboardMarkup(buttons));
                    return ScenarioResult.Completed;
            }
            return ScenarioResult.Transition;
        }

        private string GetResult(List<KpiResult> list)
        {
            if (list.Count == 0) return "Результат не найден.";
            string text = "";
            foreach(var l in list)
            {
                text += $"{l.Name}\n{l.Shop} (кат {l.Category})\nВсего чеков: {l.TotalChecks} Рез-т: {l.Result}\n" +
                    $"Премия кассир: {CalculateKpi(l).Cashier}\nПремия продавец: {CalculateKpi(l).Sales}\n-------------------\n";
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

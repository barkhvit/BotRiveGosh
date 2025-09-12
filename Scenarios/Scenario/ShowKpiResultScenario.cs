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
                        await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: ct);
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
                            InlineKeyboardButton.WithCallbackData("🏠 Главное меню",new CallBackDto(Dto_Objects.MainMenu, Dto_Action.ShowMenuNewMessage).ToString())
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
                text += $"{l.Name}\n{l.Shop}\nВсего чеков: {l.TotalChecks}\nЧеков со спец позиц: {l.SpChecks}\n" +
                    $"Результат: {l.Result}\n-------------------\n";
            }
            return text;
        }

    }
}

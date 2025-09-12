using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotRiveGosh.Scenarios
{
    public interface IScenario
    {
        bool CanHandle(ScenarioType scenarioType);
        Task<ScenarioResult> HandleScenarioAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct);
    }
}

using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRiveGosh.Services.Interfaces
{
    public interface IKpiResultService
    {
        Task<IReadOnlyList<KpiResult>> GetByNameAsync(string name, CancellationToken ct);
        Task<DateOnly?> GetLastDateUpdate(CancellationToken ct);
    }
}

using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Models;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Helpers;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRiveGosh.Services
{
    public class KpiResultService : IKpiResultService
    {
        private readonly IKpiResultRepository _kpiResultRepository;
        public KpiResultService(IKpiResultRepository kpiResultRepository)
        {
            _kpiResultRepository = kpiResultRepository;
        }
        public async Task<IReadOnlyList<KpiResult>> GetByNameAsync(string name, CancellationToken ct)
        {
            return await _kpiResultRepository.GetByNameAsync(name, ct);
        }
    }
}

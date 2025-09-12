using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services
{
    public class KpiService : IKpiService
    {
        private readonly IKpiRepository _kpiRepository;
        public KpiService(IKpiRepository kpiRepository)
        {
            _kpiRepository = kpiRepository;
        }
        public async Task<bool> UpdateKpiTable(List<Kpi> kpis, CancellationToken ct)
        {
            var isUpdate = await _kpiRepository.UpdateAsync(kpis, ct);
            return isUpdate;
        }

       
    }
}

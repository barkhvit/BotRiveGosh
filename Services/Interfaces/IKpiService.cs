using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services.Interfaces
{
    public interface IKpiService
    {
        Task<bool> UpdateKpiTable(List<Kpi> kpis, CancellationToken ct);
    }
}

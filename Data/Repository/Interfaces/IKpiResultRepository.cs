using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository.Interfaces
{
    public interface IKpiResultRepository
    {
        Task<IReadOnlyList<KpiResult>> GetByNameAsync(string name, CancellationToken ct);
        Task<DateOnly?> GetLastDateUpdate(CancellationToken ct);
    }
}

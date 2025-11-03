using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services.Interfaces
{
    public interface IPrizesService
    {
        Task<IReadOnlyList<Prizes>> GetAllAsync(CancellationToken ct);
        Task<Prizes?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Prizes?> GetByNameAsync(string name, CancellationToken ct);
    }
}

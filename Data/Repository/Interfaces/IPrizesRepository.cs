using BotRiveGosh.Core.Entities;

namespace BotRiveGosh.Data.Repository.Interfaces
{
    public interface IPrizesRepository
    {
        Task<Prizes?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Prizes?> GetByNameAsync(string name, CancellationToken ct);
        Task AddAsync(Prizes prize, CancellationToken ct);
        Task<bool> ExistsAsync(string name, CancellationToken ct);
        Task<bool> UpdateAsync(Prizes prize, CancellationToken ct);
        Task<IReadOnlyList<Prizes>> GetAllAsync(CancellationToken ct);
    }
}

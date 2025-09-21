using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct);
        Task CreateAsync(User user, CancellationToken ct);
        Task<bool> ExistsAsync(long telegramId, CancellationToken ct);
        Task<bool> UpdateAsync(User user, CancellationToken ct);
    }
}

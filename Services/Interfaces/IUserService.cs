using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(Telegram.Bot.Types.Update update, CancellationToken ct);
        Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct);
        Task<User?> GetUserByTelegramIdAsync(long telegramId, CancellationToken ct);
    }
}

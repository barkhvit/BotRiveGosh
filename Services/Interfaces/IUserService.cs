using BotRiveGosh.Core.Entities;

namespace BotRiveGosh.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetOrCreateUserAsync(Telegram.Bot.Types.Update update, CancellationToken ct);
        Task<User?> GetUserByIdAsync(Guid id, CancellationToken ct);
        Task<User?> GetUserByTelegramIdAsync(long telegramId, CancellationToken ct);
        Task<bool> UpdateAsync(Core.Entities.User user, CancellationToken ct);
    }
}

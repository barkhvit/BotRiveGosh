using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace BotRiveGosh.Data.Repository
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlUserRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }

        public async Task CreateAsync(User user, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            await dbContext.InsertAsync(ModelMapper.MapToModel(user), token: ct);
        }

        public async Task<bool> ExistsAsync(long telegramId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            return await dbContext.users.AnyAsync(u => u.TelegramId == telegramId, token: ct);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var user = await dbContext.users.FirstOrDefaultAsync(u => u.Id == id, token: ct);
            return user != null ? ModelMapper.MapFromModel(user) : null;
        }

        public async Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var user = await dbContext.users.FirstOrDefaultAsync(u => u.TelegramId == telegramId, token: ct);
            return user != null ? ModelMapper.MapFromModel(user) : null;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var updated = await dbContext.UpdateAsync(ModelMapper.MapToModel(user), token: ct);
            return updated > 0;
        }
    }
}

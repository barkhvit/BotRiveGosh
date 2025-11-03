using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository
{
    public class SqlPrizesRepository : IPrizesRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlPrizesRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }
        public async Task AddAsync(Prizes prize, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            await dbContext.InsertAsync(ModelMapper.MapToModel(prize), token: ct);
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            return await dbContext.prizes.AnyAsync(p => p.Name == name, token: ct);
        }

        public async Task<IReadOnlyList<Prizes>> GetAllAsync(CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var prizes = await dbContext.prizes
                .Select(p => ModelMapper.MapFromModel(p))
                .ToListAsync(ct);
            return prizes;
        }

        public async Task<Prizes?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var prize = await dbContext.prizes.FirstOrDefaultAsync(p => p.Id == id, token: ct);
            return prize != null ? ModelMapper.MapFromModel(prize) : null;
        }

        public async Task<Prizes?> GetByNameAsync(string name, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var prize = await dbContext.prizes.FirstOrDefaultAsync(p => p.Name == name, token: ct);
            return prize != null ? ModelMapper.MapFromModel(prize) : null;
        }

        public async Task<bool> UpdateAsync(Prizes prize, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var updated =  await dbContext.UpdateAsync(ModelMapper.MapToModel(prize), token: ct);
            return updated > 0;
        }
    }
}

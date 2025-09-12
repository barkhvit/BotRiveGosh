using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository
{
    public class SqlKpiResultRepository : IKpiResultRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlKpiResultRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }
        public Task<IReadOnlyList<KpiResult>> GetByNameAsync(string name, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var models = dbContext.GetKpi(name, DateOnly.FromDateTime(DateTime.UtcNow)).ToList();
            var result = models.Select(ModelMapper.MapFromModel).ToList();
            return Task.FromResult<IReadOnlyList<KpiResult>>(result);
        }
    }
}

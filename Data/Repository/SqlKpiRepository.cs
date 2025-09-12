using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository
{
    public class SqlKpiRepository : IKpiRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlKpiRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }
        public async Task<bool> UpdateAsync(List<Kpi> kpis, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            using var transaction = await dbContext.BeginTransactionAsync(ct);
            try
            {
                //очищаем таблицу
                await dbContext.kpis.DeleteAsync(ct);

                //добавляем новые записи
                if (kpis.Count > 0)
                {
                    var options = new BulkCopyOptions
                    {
                        BulkCopyType = BulkCopyType.MultipleRows,
                        MaxBatchSize = 10000
                    };

                    await dbContext.BulkCopyAsync(options, kpis.Select(k => ModelMapper.MapToModel(k)), ct);
                }
                //foreach(var kpi in kpis)
                //{
                //    await dbContext.InsertAsync(ModelMapper.MapToModel(kpi), token:ct);
                //}

                //коммитим транзакцию
                await transaction.CommitAsync(ct);
                return true;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(ct);
                Console.WriteLine($"Ошибка при обновлении таблицы KPI: {ex.Message}");
                return false;
            }
        }
    }
}

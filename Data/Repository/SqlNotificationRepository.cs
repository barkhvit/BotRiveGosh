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
    public class SqlNotificationRepository : INotificationRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlNotificationRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }

        // ADD
        public async Task<int> AddAsync(Notification notification, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            return await context.InsertAsync(ModelMapper.MapToModel(notification), token: ct);
        }

        // DELETE
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var n = await context.notifications
                .Where(n => n.Id == id)
                .DeleteAsync(token: ct);
            return n > 0;
        }

        // GET
        public async Task<IReadOnlyList<Notification>> GetAllByStatusAsync(bool isSenting, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var notifications = await context.notifications
                .LoadWith(n => n.User)
                .Where(n => n.Sent == isSenting)
                .Select(n => ModelMapper.MapFromModel(n))
                .ToListAsync(ct);
            return notifications != null ? notifications : new List<Notification>();
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var notification = await context.notifications
                .LoadWith(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id, token: ct);
            return notification == null ? null : ModelMapper.MapFromModel(notification);
        }

        public async Task<Notification?> GetBySubjectAsync(string subject, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var notification = await context.notifications
                .LoadWith(n => n.User)
                .FirstOrDefaultAsync(n => n.Subject == subject, token: ct);
            return notification == null ? null : ModelMapper.MapFromModel(notification);
        }

        public async Task<IReadOnlyList<Notification>> GetForSendingAsync(CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var notifications = await context.notifications
                .LoadWith(n => n.User)
                .Where(n => n.Sent == false && n.SendTo < DateTime.UtcNow)
                .Select(n => ModelMapper.MapFromModel(n))
                .ToListAsync(ct);
            return notifications != null ? notifications : new List<Notification>();
        }

        // UPDATE
        public async Task<bool> MarkAsSentAsync(Guid id, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            var notification = await context.notifications
                .LoadWith(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id, token: ct);

            if(notification != null)
            {
                notification.Sent = true;
                var n = await context.UpdateAsync(notification, token: ct);
                return n > 1;
            }

            return false;
        }

        public async Task<int> UpdateAsync(Notification notification, CancellationToken ct)
        {
            using var context = _factory.CreateDataContext();
            return await context.UpdateAsync(ModelMapper.MapToModel(notification), token: ct);
        }
    }
}

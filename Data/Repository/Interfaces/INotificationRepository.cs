using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository.Interfaces
{
    public interface INotificationRepository
    {
        // ADD
        Task<int> AddAsync(Notification notification, CancellationToken ct);

        // GET
        Task<IReadOnlyList<Notification>> GetAllByStatusAsync(bool isSenting, CancellationToken ct);
        Task<IReadOnlyList<Notification>> GetForSendingAsync(CancellationToken ct);
        Task<Notification?> GetBySubjectAsync(string subject, CancellationToken ct);
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct);

        // UPDATE
        Task<int> UpdateAsync(Notification notification, CancellationToken ct);
        Task<bool> MarkAsSentAsync(Guid id, CancellationToken ct);

        // DELETE
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
    }
}

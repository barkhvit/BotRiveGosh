using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services.Interfaces
{
    public interface INotificationService
    {
        // GET
        Task<Notification?> GetNotificationById(Guid Id, CancellationToken ct);
        Task<Notification?> GetNotificationBySubject(string subject, CancellationToken ct);
        Task<IReadOnlyList<Notification>> GetAllByStatus(bool isSending, CancellationToken ct);
        Task<IReadOnlyList<Notification>> GetForSentingAsync(CancellationToken ct);

        // UPDATE
        Task<bool> MarkAsSentAsync(Guid Id, CancellationToken ct);

        // ADD
        Task<int> AddAsync(Notification notification, CancellationToken ct);
    }
}

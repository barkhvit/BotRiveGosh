using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository.Interfaces;
using BotRiveGosh.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public async Task<int> AddAsync(Notification notification, CancellationToken ct)
        {
            return await _notificationRepository.AddAsync(notification, ct);
        }

        public async Task<IReadOnlyList<Notification>> GetAllByStatus(bool isSending, CancellationToken ct)
        {
            return await _notificationRepository.GetAllByStatusAsync(isSending, ct);
        }

        public async Task<IReadOnlyList<Notification>> GetForSentingAsync(CancellationToken ct)
        {
            return await _notificationRepository.GetForSendingAsync(ct);
        }

        public async Task<Notification?> GetNotificationById(Guid Id, CancellationToken ct)
        {
            return await _notificationRepository.GetByIdAsync(Id, ct);
        }

        public async Task<Notification?> GetNotificationBySubject(string subject, CancellationToken ct)
        {
            return await _notificationRepository.GetBySubjectAsync(subject, ct);
        }

        public async Task<bool> MarkAsSentAsync(Guid Id, CancellationToken ct)
        {
            return await _notificationRepository.MarkAsSentAsync(Id, ct);
        }
    }
}

using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotRiveGosh.Services
{
    public class InMemoryStorageService
    {
        private readonly InMemoryRepository _inMemoryRepository;
        private readonly List<long> _admins = new() { 1976535977 };

        public InMemoryStorageService(InMemoryRepository inMemoryRepository)
        {
            _inMemoryRepository = inMemoryRepository;
        }

        public async Task<IReadOnlyList<Kpi>> GetKpiStorage(long telegramUserId, CancellationToken ct)
        {
            if (_admins.Contains(telegramUserId))
            {
                return await _inMemoryRepository.GetKpiStorageAsync(ct);
            }
            return new List<Kpi>();
        }

    }
}

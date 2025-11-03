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
    public class PrizesService : IPrizesService
    {
        private readonly IPrizesRepository _prizesRepository;
        public PrizesService(IPrizesRepository prizesRepository)
        {
            _prizesRepository = prizesRepository;
        }

        public async Task<IReadOnlyList<Prizes>> GetAllAsync(CancellationToken ct)
        {
            return await _prizesRepository.GetAllAsync(ct);
        }

        public async Task<Prizes?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _prizesRepository.GetByIdAsync(id, ct);
        }

        public async Task<Prizes?> GetByNameAsync(string name, CancellationToken ct)
        {
            return await _prizesRepository.GetByNameAsync(name, ct);
        }
    }
}

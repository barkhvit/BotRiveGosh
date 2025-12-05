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
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _todoRepository;
        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<int> AddAsync(Todo todo, CancellationToken ct)
        {
            return await _todoRepository.AddAsync(todo, ct);
        }

        public async Task<IReadOnlyList<Todo>> GetActiveAndOverdueAsync(CancellationToken ct)
        {
            return await _todoRepository.GetActiveAndOverdueAsync(ct);
        }

        public async Task<IReadOnlyList<Todo>> GetActiveByFinishedDateAsync(DateOnly finishedDate, CancellationToken ct)
        {
            return await _todoRepository.GetActiveByFinishedDateAsync(finishedDate, ct);
        }

        public async Task<IReadOnlyList<Todo>> GetActiveByUserid(Guid Id, CancellationToken ct)
        {
            return await _todoRepository.GetByUserAndStatusAsync(Id, false, ct);
        }

        public async Task<Todo?> GetTodoById(Guid Id, CancellationToken ct)
        {
            return await _todoRepository.GetById(Id, ct);
        }

        public async Task<bool> MarkAsCompletedAsync(Guid Id, CancellationToken ct)
        {
            return await _todoRepository.MarkAsCompletedAsync(Id, ct);
        }
    }
}

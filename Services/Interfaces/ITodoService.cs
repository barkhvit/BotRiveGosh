using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Services.Interfaces
{
    public interface ITodoService
    {
        //GET
        Task<IReadOnlyList<Todo>> GetActiveByFinishedDateAsync(DateOnly finishedDate, CancellationToken ct);
        Task<IReadOnlyList<Todo>> GetActiveAndOverdueAsync(CancellationToken ct);
        Task<IReadOnlyList<Todo>> GetActiveByUserid(Guid Id, CancellationToken ct);
        Task<Todo?> GetTodoById(Guid Id, CancellationToken ct);




        Task<bool> MarkAsCompletedAsync(Guid Id, CancellationToken ct);
        Task<int> AddAsync(Todo todo, CancellationToken ct);
    }
}

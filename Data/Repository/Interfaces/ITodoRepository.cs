using BotRiveGosh.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Repository.Interfaces
{
    public interface ITodoRepository
    {
        //ADD
        Task<int> AddAsync(Todo todo, CancellationToken ct);

        //GET
        Task<IReadOnlyList<Todo>> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<Todo>> GetByUserAndStatusAsync(Guid userId, bool isCompleted, CancellationToken ct);
        Task<Todo?> GetById(Guid Id, CancellationToken ct);
        Task<IReadOnlyList<Todo>> GetActiveByFinishedDateAsync(DateOnly finishedDate, CancellationToken ct);
        Task<IReadOnlyList<Todo>> GetActiveAndOverdueAsync(CancellationToken ct);

        // UPDATE
        Task<int> UpdateAsync(Todo todo, CancellationToken ct);
        Task<bool> MarkAsCompletedAsync(Guid id, CancellationToken ct);

        // DELETE
        Task<bool> DeleteAsync(Guid id, CancellationToken ct);
        Task<int> DeleteCompletedByUserAsync(Guid userId, CancellationToken ct);
    }
}
 
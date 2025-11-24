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
    public class SqlTodoRepository : ITodoRepository
    {
        private readonly IDataContextFactory<DBContext> _factory;
        public SqlTodoRepository(IDataContextFactory<DBContext> factory)
        {
            _factory = factory;
        }
        public async Task<int> AddAsync(Todo todo, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            return await dbContext.InsertAsync(ModelMapper.MapToModel(todo), token: ct);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var n = await dbContext.todos
                .Where(t => t.Id == id)
                .DeleteAsync(ct);
            return n > 0;
        }

        public async Task<int> DeleteCompletedByUserAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            return await dbContext.todos
                .Where(t => t.UserId == userId && t.IsCompleted == true)
                .DeleteAsync(ct);
        }

        public async Task<Todo?> GetById(Guid Id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var todo = await dbContext.todos
                .LoadWith(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == Id, token: ct);
            return todo == null ? null : ModelMapper.MapFromModel(todo);
        }

        public async Task<IReadOnlyList<Todo>> GetByUserAndStatusAsync(Guid userId, bool isCompleted, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var todos = await dbContext.todos
                .LoadWith(t => t.User)
                .Where(t => t.UserId == userId && t.IsCompleted == isCompleted)
                .OrderBy(t => t.FinishedAt)
                .Select(t => ModelMapper.MapFromModel(t))
                .ToListAsync(ct);
            return todos ?? new List<Todo>();
        }

        public async Task<IReadOnlyList<Todo>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var todos = await dbContext.todos
                .LoadWith(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.FinishedAt)
                .Select(t => ModelMapper.MapFromModel(t))
                .ToListAsync(ct);
            return todos ?? new List<Todo>();
        }

        public async Task<bool> MarkAsCompletedAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var todo = await dbContext.todos.FirstOrDefaultAsync(t => t.Id == id, ct);
            
            if(todo != null)
            {
                todo.IsCompleted = true;
                var affectedRows = await dbContext.UpdateAsync(todo, token: ct);
                return affectedRows > 0;
            }
            return false;
        }

        public async Task<int> UpdateAsync(Todo todo, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            return await dbContext.UpdateAsync(ModelMapper.MapToModel(todo), token: ct);
        }
    }
}

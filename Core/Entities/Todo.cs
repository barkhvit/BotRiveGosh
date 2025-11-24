using BotRiveGosh.Core.Common.Enums;
using BotRiveGosh.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.Entities
{
    public class Todo
    {
        public Guid Id { get; set; }
        public User User { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly CreatedAt { get; set; }
        public DateOnly FinishedAt { get; set; }
        public bool IsCompleted { get; set; }

        public Todo(User user, string name, string description, DateOnly finishedat)
        {
            Id = Guid.NewGuid();
            User = user;
            Name = name;
            Description = description;
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            FinishedAt = finishedat;
        }
        public Todo()
        {
            Id = Guid.NewGuid();
        }
    }
}

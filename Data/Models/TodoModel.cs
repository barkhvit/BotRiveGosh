using BotRiveGosh.Core.Entities;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Models
{
    [Table("todo")]
    public class TodoModel
    {
        [PrimaryKey][Column("id")]public Guid Id { get; set; }
        [Column("userid")]public Guid UserId { get; set; }
        [Column("name")] public string Name { get; set; } = string.Empty;
        [Column("description")] public string Description { get; set; } = string.Empty;
        [Column("createdat")]public DateOnly CreatedAt { get; set; }
        [Column("finishedat")]public DateOnly FinishedAt { get; set; }
        [Column("iscompleted")] public bool IsCompleted { get; set; } = false;

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(UserModel.Id))]
        public UserModel User { get; set; } = null!;
    }
}

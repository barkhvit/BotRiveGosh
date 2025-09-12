using System;
using System.Collections.Generic;
using LinqToDB.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Models
{
    [Table("users")]
    public class UserModel
    {
        [PrimaryKey][Column("id")]public Guid Id { get; set; }
        [Column("telegramid")] public long TelegramId { get; set; }
        [Column("username")] public string? Username { get; set; }
        [Column("firstname")] public string? FirstName { get; set; }
        [Column("lastname")] public string? LastName { get; set; }
        [Column("createdat")] public DateTime CreatedAt { get; set; }
    }
}

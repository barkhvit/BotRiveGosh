using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace BotRiveGosh.Data.Models
{
    [Table("notification")]
    public class NotificationModel
    {
        [PrimaryKey][Column("id")] public Guid Id { get; set; }
        [Column("userid")] public Guid UserId { get; set; }
        [Column("subject")] public string Subject { get; set; } = String.Empty;
        [Column("text")] public string Text { get; set; } = String.Empty;
        [Column("sendto")] public DateTime SendTo { get; set; }
        [Column("sent")] public bool Sent { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(UserModel.Id))]
        public UserModel User { get; set; } = null!;
    }
}

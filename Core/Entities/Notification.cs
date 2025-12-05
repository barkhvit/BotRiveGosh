using BotRiveGosh.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public User User { get; set; } = null!;
        public string Subject { get; set; } = String.Empty;
        public string Text { get; set; } = String.Empty;
        public DateTime SendTo { get; set; }
        public bool Sent { get; set; }
    }
}

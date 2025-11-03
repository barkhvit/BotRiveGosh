using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.Entities
{
    public class KpiResult
    {
        public string Shop { get; set; } = "";
        public string Category { get; set; } = "";
        public string Name { get; set; } = "";
        public long TotalChecks { get; set; }
        public long SpChecks { get; set; }
        public decimal Result { get; set; }
    }
}

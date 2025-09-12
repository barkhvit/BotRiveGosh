using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.Entities
{
    public class Kpi
    {
        public long Id { get; set; }
        public int ShopId { get; set; }
        public DateTime Date { get; set; }
        public string Position { get; set; }
        public string Name { get; set; }
        public string? LocalId { get; set; }
        public string? TNumber { get; set; }
        public string? CardType { get; set; }
        public int Checks { get; set; }
        public int SpecialChecks { get; set; }

    }
}

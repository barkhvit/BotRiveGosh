using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Models
{
    public class KpiResultModel
    {
        [Column("month")] public string Month { get; set; } = "";
        [Column("shop")] public string Shop { get; set; } = "";
        [Column("category")] public string Category { get; set; } = "";
        [Column("name")] public string Name { get; set; } = "";
        [Column("total_checks")]public long TotalChecks { get; set; }
        [Column("sp_checks")]public long SpChecks { get; set; }
        [Column("result")]public decimal Result { get; set; }
    }
}

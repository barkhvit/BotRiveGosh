using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotRiveGosh.Core.Entities;

namespace BotRiveGosh.Data.Models
{
    [Table("kpi")]
    public class KpiModel
    {
        [PrimaryKey][Column("id")]public long Id { get; set; }
        [Column("shopid")] public int ShopId { get; set; }
        [Column("date")] public DateTime Date { get; set; }
        [Column("position")] public string Position { get; set; } = "";
        [Column("name")] public string Name { get; set; } = "";
        [Column("localid")] public string? LocalId { get; set; }
        [Column("tnumber")] public string? TNumber { get; set; }
        [Column("cardtype")] public string? CardType { get; set; }
        [Column("checks")] public int Checks { get; set; }
        [Column("specialchecks")] public int SpecialChecks { get; set; }


        [Association(ThisKey = nameof(ShopId), OtherKey = nameof(ShopsModel.Id))]
        public ShopsModel Shops { get; set; } = null!;
    }
}

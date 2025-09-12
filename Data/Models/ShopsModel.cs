using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace BotRiveGosh.Data.Models
{
    [Table("shops")]
    public class ShopsModel
    {
        [Column("id"), PrimaryKey] public int Id { get; set; }
        [Column("namebu")] public string NameBU { get; set; } = "";
        [Column("nameuu")] public string NameUU { get; set; } = "";
        [Column("nameqv")] public string NameQV { get; set; } = "";
        [Column("region")] public string Region { get; set; } = "";
        [Column("city")] public string City { get; set; } = "";
        [Column("location")] public string Location { get; set; } = "";
        [Column("category")] public string Category { get; set; } = "";
        [Column("rk")] public double RK { get; set; }
        [Column("sn")] public double SN { get; set; }
    }
}

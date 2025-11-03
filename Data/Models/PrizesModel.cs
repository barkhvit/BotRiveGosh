using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data.Models
{
    [Table("prizes")]
    public class PrizesModel
    {
        [Column("id"), PrimaryKey] public Guid Id { get; set; }
        [Column("name")] public string Name { get; set; } = ""; 
        [Column("description")] public string Description { get; set; } = "";
        [Column("inarchive")] public bool InArchive { get; set; } = false;
    }
}

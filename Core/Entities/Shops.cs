using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.Entities
{
    public class Shops
    {
        public int Id { get; set; }
        public string NameBU { get; set; } = "";
        public string NameUU { get; set; } = "";
        public string NameQV { get; set; } = "";
        public string Region { get; set; } = "";
        public string City { get; set; } = "";
        public string Location { get; set; } = "";
        public string Category { get; set; } = "";
        public double RK { get; set; }
        public double SN { get; set; }
    }
}

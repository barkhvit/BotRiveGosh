using BotRiveGosh.Data.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data
{
    public class DBContext : DataConnection
    {
        public DBContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }
        public ITable<UserModel> users => this.GetTable<UserModel>();
        public ITable<KpiModel> kpis => this.GetTable<KpiModel>();

        // Метод для вызова функции
        public IEnumerable<KpiResultModel> GetKpi(string searchName)
        {
            return this.Query<KpiResultModel>(
                "SELECT * FROM get_kpi(@search_name)",
                new DataParameter("search_name", searchName)
            );
        }

        public IEnumerable<KpiResultModel> GetKpi(string searchName, DateOnly dateOnly)
        {
            return this.Query<KpiResultModel>(
                "SELECT * FROM get_kpi(@search_name, @date)",
                new DataParameter("search_name", searchName),
                new DataParameter("date", dateOnly)
            );
        }
    }
}

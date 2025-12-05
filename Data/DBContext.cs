using BotRiveGosh.Core.Entities;
using BotRiveGosh.Data.Models;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Data
{
    public class DBContext : DataConnection
    {
        public DBContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }
        public ITable<UserModel> users => this.GetTable<UserModel>();
        public ITable<KpiModel> kpis => this.GetTable<KpiModel>();
        public ITable<PrizesModel> prizes => this.GetTable<PrizesModel>();
        public ITable<TodoModel> todos => this.GetTable<TodoModel>();
        public ITable<NotificationModel> notifications => this.GetTable<NotificationModel>();

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

        public IEnumerable<KpiResultModel> GetKpiWithMonth(string searchName)
        {
            return this.Query<KpiResultModel>(
                "SELECT * FROM get_kpi_withmonth(@search_name)",
                new DataParameter("search_name", searchName)
            );
        }

        //последняя дата обновления таблицы kpi
        public DateOnly? GetLastUpdateKpi()
        {
            var lastDates = this.Query<DateTime?>("SELECT public.get_max_kpi_date()");
            var lastDate = lastDates.Max();
            if(lastDate!=null)
                return DateOnly.FromDateTime((DateTime)lastDate);

            return null;
        } 
    }
}

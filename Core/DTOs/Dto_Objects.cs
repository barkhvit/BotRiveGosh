using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.DTOs
{
    public static class Dto_Objects
    {
        public static string Kpi { get; } = nameof(Kpi);
        public static string MainMenuView { get; } = nameof(MainMenuView);
        public static string AboutBotView { get; } = nameof(AboutBotView);
        public static string UpdatekpiView { get; } = nameof(UpdatekpiView);
        public static string Reg { get; } = nameof(Reg);

        //views
        public static string AllPrizesView { get; } = nameof(AllPrizesView); // все премии
        public static string DetailPrizeView { get; } = nameof(DetailPrizeView); //премия подробно
        public static string MenuKpiView { get; } = nameof(MenuKpiView); //меню kpi кассира
    }
}

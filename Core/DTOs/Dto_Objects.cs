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
        public static string Todo { get; } = nameof(Todo);
        public static string MainMenuView { get; } = nameof(MainMenuView);
        public static string AboutBotView { get; } = nameof(AboutBotView);
        public static string UpdatekpiView { get; } = nameof(UpdatekpiView);
        public static string Reg { get; } = nameof(Reg);

        //views
        public static string AllPrizesView { get; } = nameof(AllPrizesView); // все премии
        public static string DetailPrizeView { get; } = nameof(DetailPrizeView); //премия подробно
        public static string PrizeMenuView { get; } = nameof(PrizeMenuView); //меню премии
        public static string TodoMenuView { get; } = nameof(TodoMenuView); //меню мои задачи
        public static string TodoListView { get; } = nameof(TodoListView); //список задач
        public static string TodoEditView { get; } = nameof(TodoEditView); //редактировать задачи
        public static string TodoDetailView { get; internal set; } = nameof(TodoDetailView); //детальный просмотр задачи
        public static string TodoDoneView { get; internal set; } = nameof(TodoDoneView); //завершить задачу Да/Нет
        
    }
}

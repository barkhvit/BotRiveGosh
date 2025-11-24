using BotRiveGosh.Views.Prize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh.Core.DTOs
{
    public static class Dto_Action
    {
        public static string ShowMenu { get; } = nameof(ShowMenu);
        public static string ShowMenuNewMessage { get; } = nameof(ShowMenuNewMessage);
        public static string Calculate { get; } = nameof(Calculate);
        public static string ShowRules { get; } = nameof(ShowRules);

        //AboutBotView
        public static string AboutBotShow { get; } = nameof(AboutBotShow);

        //UpdatekpiView
        public static string UpdateKpi { get; } = nameof(UpdateKpi);
        public static string UpdateKpiConfirm { get; } = nameof(UpdateKpiConfirm);

        //Registration
        public static string RegRequest { get; } = nameof(RegRequest);//запрос на доступ
        public static string RegApprove { get; } = nameof(RegApprove);//разрешить доступ
        public static string RegReject { get; } = nameof(RegReject);//отклонить доступ

        //views
        public static string Show { get; } = nameof(Show);
        public static string ShowKpiPrize { get; } = nameof(ShowKpiPrize);


        //TodoListView и TodoEditView
        public static string ShowToday { get; } = nameof(ShowToday);
        public static string ShowOverdue { get; } = nameof(ShowOverdue);
        public static string TodoDone { get; internal set; } = nameof(TodoDone);

        //сценарии
        public static string AddTodoScenario { get; } = nameof(AddTodoScenario); //создать новую задачу
        public static string ShowResultScenario { get; } = nameof(ShowResultScenario); //показать результат kpi
    }
}

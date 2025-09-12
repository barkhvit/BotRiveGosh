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
        public static string ShowResult { get; } = nameof(ShowResult);
        public static string Calculate { get; } = nameof(Calculate);
        public static string ShowRules { get; } = nameof(ShowRules);

        //AboutBot
        public static string AboutBotShow { get; } = nameof(AboutBotShow);

        //Update
        public static string UpdateKpi { get; } = nameof(UpdateKpi);
        public static string UpdateKpiConfirm { get; } = nameof(UpdateKpiConfirm);

    }
}

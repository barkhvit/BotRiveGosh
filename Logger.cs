using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BotRiveGosh
{
    public static class Logger
    {
        public static void Log(string message, [CallerMemberName] string methodName = "", [CallerFilePath] string filePath = "")
        {
            var className = Path.GetFileNameWithoutExtension(filePath);
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {className}.{methodName}: {message}");
        }
    }
}

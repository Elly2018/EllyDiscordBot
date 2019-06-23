using System;
using System.Collections.Generic;
using System.Text;

namespace EllyDiscordCore
{
    public sealed class Debug
    {
        /* The discord framework log main function */
        public static void Log(string tag, object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("[" + tag + "] " + message);
        }

        public static void BotLog(string botname, string tag, object message, ConsoleColor color)
        {
            Log(botname + " | " + tag, message, color);
        }
    }
}

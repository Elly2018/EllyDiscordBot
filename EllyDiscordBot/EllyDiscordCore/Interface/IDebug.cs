using System;
using System.Collections.Generic;
using System.Text;

namespace EllyDiscordCore.Interface
{
    public interface IDebug
    {
        void Log(string tag, object message, ConsoleColor color);
        void Log(string tag, string botname, object message, ConsoleColor color);
    }
}

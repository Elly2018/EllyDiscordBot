using EllyDiscordCore.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Discord.WebSocket;
using Discord.Commands;
using System.Threading.Tasks;

namespace EllyDiscordCore
{
    public abstract class CustomModule
    {
        public DiscordSocketClient m_DiscordSocketClient;
        public string m_PathRoot;

        public void SetBotRoot(string pathroot)
        {
            m_PathRoot = pathroot;
        }

        public string GetBotRootPath()
        {
            return m_PathRoot;
        }

        public string GetModPath()
        {
            return Path.Combine(GetBotRootPath(), Utility.BotsModFolder);
        }

        public string GetConfigPath()
        {
            return Path.Combine(GetBotRootPath(), Utility.BotsConfigFolder);
        }

        public string GetDataPath()
        {
            return Path.Combine(GetBotRootPath(), Utility.BotsDataFolder);
        }
    }
}

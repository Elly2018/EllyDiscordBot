using EllyDiscordCore.Setup;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EllyDiscordCore
{
    public abstract class CustomModule
    {
        public string GetBotRootPath()
        {
            return Path.GetPathRoot(Assembly.GetCallingAssembly().Location);
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

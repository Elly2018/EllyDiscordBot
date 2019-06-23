using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EllyDiscordCore.Interface;
using EllyDiscordCore.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace EllyDiscordCore
{
    public enum BotState
    {
        None, Continue, Out
    }

    public abstract class BotHandleBase
    {
        /* Discord client */
        private DiscordSocketClient m_DiscordSocketClient;
        /* Services */
        private ServiceProvider m_ServiceProvider;
        /* The given bot setting */
        private BotSetting m_BotSetting;
        /* Bot path */
        private string m_Path = String.Empty;
        /* All the modules */
        private Assembly[] m_Modules;


        /* Bot current state */
        private BotState m_BotState = BotState.None;

        /* When create a bot handle we need to pass bot setting information  */
        /* In order to make program work */
        public BotHandleBase(BotSetting botSetting)
        {
            /* Store setting */
            m_BotSetting = botSetting;

            /* Configure service */
            Debug.BotLog(m_BotSetting.name, "Initialize", "Create " + m_BotSetting.name + " bot configure services", (ConsoleColor)m_BotSetting.color);
            m_ServiceProvider = ConfigureServices();

            /* Socket client */
            Debug.BotLog(m_BotSetting.name, "Initialize", "Create " + m_BotSetting.name + " bot socket client", (ConsoleColor)m_BotSetting.color);
            m_DiscordSocketClient = m_ServiceProvider.GetRequiredService<DiscordSocketClient>();
        }

        public void RegisterBotPath(string path)
        {
            Debug.BotLog(m_BotSetting.name, "Initialize", "Register path " + path, (ConsoleColor)m_BotSetting.color);
            m_Path = path;
        }

        /* Loading the mods from the mod directory */
        public void LoadModule()
        {
            Debug.BotLog(m_BotSetting.name, "Modules", "Start loading modules...", (ConsoleColor)m_BotSetting.color);
            bool PathExist = Directory.Exists(m_Path);

            if (!PathExist)
            {
                Debug.BotLog(m_BotSetting.name, "Modules", "Path " + m_Path + " does not exist", (ConsoleColor)m_BotSetting.color);
                return;
            }

            bool ModExist = Directory.Exists(Path.Combine(m_Path, Utility.BotsModFolder));

            if (!ModExist)
            {
                Debug.BotLog(m_BotSetting.name, "Modules", "Mod path does not exist", ConsoleColor.Yellow);
                Debug.BotLog(m_BotSetting.name, "Modules", "Please check your setup", ConsoleColor.Yellow);
                return;
            }

            /* Modules */
            string[] modules = Directory.GetFiles(Path.Combine(m_Path, Utility.BotsModFolder));
            List<Assembly> mods = new List<Assembly>();

            /* Loading all the mod in Mod directory */
            for(int i = 0; i < modules.Length; i++)
            {
                Assembly mod = Assembly.LoadFile(Path.GetFullPath(modules[i]));
                mods.Add(mod);

                for(int k = 0; k < mod.GetTypes().Length; k++)
                {
                    /* We create a instance object for every single types in the assembly */
                    object instance = mod.CreateInstance(mod.GetTypes()[k].Name);

                    ICustomModule module_interface = instance as ICustomModule;
                    if(module_interface != null)
                    {
                        Struct.ModuleInfo info = module_interface.GetInfo();
                        Debug.BotLog(m_BotSetting.name, "Modules", "Detect module: " + info.m_ModuleName, (ConsoleColor)m_BotSetting.color);
                        Debug.BotLog(m_BotSetting.name, "Modules", "Module descrption: " + info.m_ModuleDescription, (ConsoleColor)m_BotSetting.color);
                        Debug.BotLog(m_BotSetting.name, "Modules", "Module Version: " + info.m_VersionMajor + "." + info.m_VersionMinor, (ConsoleColor)m_BotSetting.color);
                    }
                }

            }

            /* store into array */
            m_Modules = mods.ToArray();

            Debug.BotLog(m_BotSetting.name, "Modules", "Finished loading modules... this bot's modules amount: " + mods.Count, (ConsoleColor)m_BotSetting.color);
        }

        public async void Login()
        {
            await m_ServiceProvider.GetService<CommandHandlingService>().SetPrefix(m_BotSetting.prefix + m_BotSetting.syntax);

            m_DiscordSocketClient.Log += LogAsync;
            m_ServiceProvider.GetRequiredService<CommandService>().Log += LogAsync;

            try
            {
                await m_DiscordSocketClient.LoginAsync(Discord.TokenType.Bot, m_BotSetting.token);
                await m_DiscordSocketClient.StartAsync();

                m_BotState = BotState.Continue;
                Debug.BotLog(m_BotSetting.name, "Login", "Successfully loading ", (ConsoleColor)m_BotSetting.color);
            }
            catch
            {
                Debug.BotLog(m_BotSetting.name, "Login", "Login failed...", ConsoleColor.Red);
                Debug.BotLog(m_BotSetting.name, "Login", "Please check you token", ConsoleColor.Red);
                m_BotState = BotState.Out;
                return;
            }

            m_DiscordSocketClient.MessageReceived += SendMessage;
            await m_ServiceProvider.GetRequiredService<CommandHandlingService>().InitializeAsync(m_Modules);
        }

        /* Discord api log message output */
        private Task LogAsync(LogMessage log)
        {
            Debug.BotLog(m_BotSetting.name, "Discord", log.ToString(), (ConsoleColor)m_BotSetting.color);
            return Task.CompletedTask;
        }

        public virtual async Task Update()
        {
            if (m_Modules == null) return;
        }

        public virtual async Task SendMessage(SocketMessage rawMessage)
        {
            if (m_Modules == null) return;
            // Ignore system messages, or messages from other bots
            
        }

        public BotState GetBotCurrentState()
        {
            return m_BotState;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}

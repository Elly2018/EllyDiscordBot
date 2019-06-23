using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EllyDiscordCore;
using EllyDiscordCore.Setup;
using Newtonsoft.Json;

namespace EllyDiscordFramework
{
    class Program
    {
        private static bool Failed = true;
        private static string[] BotFolderNames;
        private static Properties m_Properties;
        private static BotHandle[] m_EllyDiscordFramework;

        static void Main(string[] args)
        {
            /* Initialize message */
            Console.Beep();
            Debug.Log("Framework", "Elly discord server initialize...", ConsoleColor.White);
            Debug.Log("Framework", "Check the setup configuration", ConsoleColor.White);

            /* Check if the setup is good to run the framework */
            CheckSetup();

            /* Mean failed to initialize, setup error, throw exception */
            if (Failed)
            {
                Console.ReadLine();
                return;
            }

            /* Start create bot */
            BotHandleCreate().GetAwaiter().GetResult();

            /* Ending pause */
            Console.Beep();
            Console.ReadLine();
        }

        /* Program will check all the setup */
        /* Directory exist */
        /* And get all setting file */
        /* Make sure the amount of bot can be use is bigger than 0 */
        static void CheckSetup()
        {
            /* Check bot root directory exist */
            bool BotsExist = Directory.Exists(Utility.BotsRootFolder);

            /* Make sure the bot root directory exist */
            if (!BotsExist)
            {
                /* Tell user check setup wiki */
                Debug.Log("Framework", "Cannot find " + Utility.BotsRootFolder + " in the working directory", ConsoleColor.Red);
                Debug.Log("Framework", "Please check your setup", ConsoleColor.Red);
                return;
            }

            /* Check bot directories exist */
            BotFolderNames = Directory.GetDirectories(Utility.BotsRootFolder);

            /* Make sure bot directories bigger than 0 */
            if(BotFolderNames.Length == 0)
            {
                /* Tell user check setup wiki */
                Debug.Log("Framework", "Your " + Utility.BotsRootFolder + " Should have directories contain all the bot", ConsoleColor.Red);
                Debug.Log("Framework", "Each directory represent every single bot", ConsoleColor.Red);
                Debug.Log("Framework", "Please check your setup", ConsoleColor.Red);
                return;
            }

            /* If program pass to here, this mean user have finished the setup.exe setting */
            /* But user also need to override the default blank setting, in order to make bot work */

            /* Create a buffer */
            List<BotHandle> botlist = new List<BotHandle>();

            /* Make sure the every single setting file in the bot directory can be use */
            for (int i = 0; i < BotFolderNames.Length; i++)
            {
                /* Check setting file exist */
                bool SettingExist = File.Exists(Path.Combine(BotFolderNames[i], Utility.BotsSettingFile));

                /* If there is no setting file */
                if (!SettingExist)
                {
                    /* Tell user check your setup */
                    Debug.Log("Framework", "Cannot find " + Utility.BotsSettingFile + " in " + BotFolderNames[i], ConsoleColor.Yellow);
                    Debug.Log("Framework", "Please check your setup", ConsoleColor.Yellow);
                }
                else
                {
                    bool readsuccess = false;
                    BotSetting botsetting = new BotSetting();

                    try
                    {
                        /* Try to read the file and convert into object */
                        string botsettingstring = File.ReadAllText(Path.Combine(BotFolderNames[i], Utility.BotsSettingFile));
                        botsetting = JsonConvert.DeserializeObject<BotSetting>(botsettingstring);
                        Debug.Log("Framework", "Successfully load file: " + Utility.BotsSettingFile + " in " + BotFolderNames[i], ConsoleColor.White);
                        readsuccess = true;
                    }
                    catch
                    {
                        /* This mean the file is broken */
                        Debug.Log("Framework", "Read " + Utility.BotsSettingFile + " in " + BotFolderNames[i] + " failed.", ConsoleColor.Yellow);
                        Debug.Log("Framework", "The file might be broken", ConsoleColor.Yellow);
                        Debug.Log("Framework", "Please check your setup", ConsoleColor.Yellow);
                        readsuccess = false;
                    }

                    /* If the setting file read success */
                    if (readsuccess)
                    {
                        Debug.Log("Framework", "Create bot instance...", ConsoleColor.White);
                        BotHandle bothandle = new BotHandle(botsetting);

                        /* Tell the bot path */
                        /* Let the bot object store path info */
                        bothandle.RegisterBotPath(BotFolderNames[i]);

                        /* Add it in to list */
                        botlist.Add(bothandle);
                    }
                }
            }

            /* Finished all setting file reading */
            /* Also finished all bot creation */

            /* Put the buffer into variable */
            m_EllyDiscordFramework = botlist.ToArray();

            /* Check if the available bot exist */
            if (m_EllyDiscordFramework.Length == 0)
            {
                Debug.Log("Framework", "There is no bot available in this setup", ConsoleColor.Yellow);
                Debug.Log("Framework", "Please check your setup", ConsoleColor.Yellow);
                return;
            }

            /* When program pass to here, this mean the setup is satisfied condition that can make bot program run */
            /* So when the action finished, program will active bot by the setting */
            Failed = false;
            Debug.Log("Framework", "Successfully pass setup check", ConsoleColor.White);

            /* Check framework properties exist */
            bool PropertiesExist = File.Exists(Utility.FrameworkProperties);

            /* If properties is not exist, create a default one */
            if (!PropertiesExist)
            {
                Debug.Log("Framework", "Cannot find framework properties file", ConsoleColor.Yellow);
                Debug.Log("Framework", "It's okay, we create a default one in the directory for you", ConsoleColor.Yellow);
                File.WriteAllText(Utility.FrameworkProperties, JsonConvert.SerializeObject(new Properties(), Formatting.Indented));
                m_Properties = new Properties();
            }
            else
            {
                /* If properties exist, read it in to variable */
                try
                {
                    string propertiesstring = File.ReadAllText(Utility.FrameworkProperties);
                    m_Properties = JsonConvert.DeserializeObject<Properties>(propertiesstring);
                    Debug.Log("Framework", "Successfully load file: " + Utility.FrameworkProperties, ConsoleColor.White);
                }
                catch
                {
                    Debug.Log("Framework", "Read " + Utility.FrameworkProperties + " failed.", ConsoleColor.Yellow);
                    Debug.Log("Framework", "The file might be broken", ConsoleColor.Yellow);
                    Debug.Log("Framework", "Do you want to delete current one and make a default one?", ConsoleColor.Yellow);
                    string answer = Console.ReadLine();

                    if (answer.ToUpper() == "Y" || answer.ToUpper() == "Yes")
                    {
                        Debug.Log("Framework", "Recreate default setting file", ConsoleColor.White);
                        File.Delete(Utility.FrameworkProperties);
                        File.WriteAllText(Utility.FrameworkProperties, JsonConvert.SerializeObject(new Properties(), Formatting.Indented));
                        m_Properties = new Properties();
                    }
                    else if (answer.ToUpper() == "N" || answer.ToUpper() == "No")
                    {
                        Debug.Log("Framework", "We will load the default version one", ConsoleColor.White);
                        m_Properties = new Properties();
                    }
                    else
                    {
                        Debug.Log("System", "Your answer either yes or no, program continue...", ConsoleColor.Cyan);
                        Debug.Log("Framework", "We will load the default version one", ConsoleColor.White);
                        m_Properties = new Properties();
                    }
                }
            }
        }

        /* Bot loop */
        static async Task BotHandleCreate()
        {
            bool Continue = true;

            Debug.Log("Framework", "Start active the bots...", ConsoleColor.White);

            /* We make every bot login */
            for (int i = 0; i < m_EllyDiscordFramework.Length; i++)
            {
                m_EllyDiscordFramework[i].LoadModule();
                m_EllyDiscordFramework[i].Login();
            }
            Debug.Log("Framework", "Update loop start", ConsoleColor.White);

            /* Enter update loop */
            while (Continue)
            {

                /* Call for update for every bot */
                for (int i = 0; i < m_EllyDiscordFramework.Length; i++)
                {
                    await m_EllyDiscordFramework[i].Update();
                }

                /* Get all bots state */
                bool[] BotsstateAlive = GetAllBotState(m_EllyDiscordFramework);

                /* Check leave loop */
                Continue = ShouldContinueProgram(BotsstateAlive);
                if (!Continue) break;

                /* Tick */
                await Task.Delay(m_Properties.UpdateMilliseconds);
            }

            Debug.Log("Framework", "Program exit...", ConsoleColor.White);
        }

        /* Loop get all the bot state */
        static bool[] GetAllBotState(BotHandle[] handles)
        {
            bool[] result = new bool[handles.Length];
            for (int i = 0; i < m_EllyDiscordFramework.Length; i++)
            {
                if (m_EllyDiscordFramework[i].GetBotCurrentState() == BotState.Out)
                    result[i] = false;
                else
                    result[i] = true;
            }
            return result;
        }

        /* Check if at least one bot alive */
        static bool ShouldContinueProgram(bool[] states)
        {
            for(int i = 0; i < states.Length; i++)
            {
                /* If anyone of bot alive */
                if (states[i] == true) return true;
            }
            return false;
        }
    }
}

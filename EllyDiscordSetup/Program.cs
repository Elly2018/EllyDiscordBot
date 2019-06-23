using System;
using System.IO;
using Newtonsoft.Json;
using EllyDiscordCore.Setup;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using EllyDiscordCore;
using EllyDiscordCore.Interface;

namespace EllyDiscordSetup
{
    class Program
    {
        private static string[] BotFolder;
        private static string[] ModArray;
        private static bool Failed = true;

        static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            objects.Sort();
            return objects;
        }

        static void Main(string[] args)
        {
            /* Setup the directory */
            /* And presetup */
            Setup();

            /* This mean the process failed at the earily state */
            /* Program will skip mod intialize process because directory need user to setup */
            if (Failed) {
                Console.ReadLine();
                return;
            }

            /* Make every mod initialize */
            /* Create config file etc.. etc.. */
            ModSetup();

            /* Ending pause */
            Console.ReadLine();
        }

        /* Setup function handle the directory setup */
        /* This will guide the user create right tree structor for program to work */
        static void Setup()
        {
            /* Check if the bots directory exist first */
            bool BotsExist = Directory.Exists(Utility.BotsRootFolder);
            Debug.Log("System", "Setup start...", ConsoleColor.Cyan);

            /* User don't even create the bots directory yet */
            if (!BotsExist)
            {
                Debug.Log("Error", "You should create " + Utility.BotsRootFolder + " directory first", ConsoleColor.Red);
                Debug.Log("Error", Utility.BotsRootFolder + " directory will contain all the bot", ConsoleColor.Red);

                /* Auto create one for user */
                Directory.CreateDirectory(Utility.BotsRootFolder);
                Debug.Log("Error", "Here, we create one for you", ConsoleColor.Red);
                Debug.Log("Error", "You can create bot directory for continue", ConsoleColor.Red);

                /* Then quit the program */
                Console.ReadLine();
                return;
            }

            Debug.Log("System", "Successfully detected " + Utility.BotsRootFolder + " directory", ConsoleColor.Cyan);
            BotFolder = Directory.GetDirectories(Utility.BotsRootFolder);

            /* User have no sub directory in the bots directory */
            /* We need at least one for bot setup */
            if (BotFolder.Length == 0)
            {
                /* Metion why user need create sub directory */
                Debug.Log("Error", "In order to make setup.exe work", ConsoleColor.Red);
                Debug.Log("Error", "You have to create sub directories in the Bots directory", ConsoleColor.Red);
                Debug.Log("Error", "Each directory repersent a single bot", ConsoleColor.Red);

                /* Then quit the program */
                Console.ReadLine();
                return;
            }

            Debug.Log("System", "Successfully detected sub bot directory", ConsoleColor.Cyan);

            /* This mean program can initialize mod setup */
            Failed = false;

            /* Loop handle all the bot initialization */
            /* Because we might have multiple bot */
            for (int i = 0; i < BotFolder.Length; i++)
            {
                Debug.Log("System", "Handle sub directory: " + BotFolder[i], ConsoleColor.Cyan);

                /* The worker have these things to do */
                bool ModExist = Directory.Exists(Path.Combine(BotFolder[i], Utility.BotsModFolder));
                bool ConfigExist = Directory.Exists(Path.Combine(BotFolder[i], Utility.BotsConfigFolder));
                bool DataExist = Directory.Exists(Path.Combine(BotFolder[i], Utility.BotsDataFolder));
                bool SettingExist = File.Exists(Path.Combine(BotFolder[i], Utility.BotsSettingFile));

                if (ModExist)
                {
                    Debug.Log("Warning", Utility.BotsModFolder + " directory already exist", ConsoleColor.Yellow);
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(BotFolder[i], Utility.BotsModFolder));
                    Debug.Log("System", "Create " + Utility.BotsModFolder + " directory...", ConsoleColor.Cyan);
                }

                if (ConfigExist)
                {
                    Debug.Log("Warning", Utility.BotsConfigFolder + " directory already exist", ConsoleColor.Yellow);
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(BotFolder[i], Utility.BotsConfigFolder));
                    Debug.Log("System", "Create " + Utility.BotsConfigFolder + " directory...", ConsoleColor.Cyan);
                }

                if (DataExist)
                {
                    Debug.Log("Warning", Utility.BotsDataFolder + " directory already exist", ConsoleColor.Yellow);
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(BotFolder[i], Utility.BotsDataFolder));
                    Debug.Log("System", "Create " + Utility.BotsDataFolder + " directory...", ConsoleColor.Cyan);
                }

                /* This is check setting.json */
                if (SettingExist)
                {
                    /* Setting.json already exist in the directory */
                    Debug.Log("Warning", Utility.BotsSettingFile + " already exist", ConsoleColor.Yellow);
                    Debug.Log("Warning", "Trying to load detect broken...", ConsoleColor.Yellow);

                    /* We will try to use Newtonsoft.json to deserialize the json file */
                    /* Check if the file format is broken */
                    try
                    {
                        string botsettingstring = File.ReadAllText(Path.Combine(BotFolder[i], Utility.BotsSettingFile));
                        BotSetting botsetting = JsonConvert.DeserializeObject<BotSetting>(botsettingstring);
                        Debug.Log("System", "Successfully load file: " + Utility.BotsSettingFile, ConsoleColor.Cyan);
                    }
                    catch
                    {
                        /* The program have expection when reading the json file */
                        /* Asking user if user want create a default Setting.json */
                        Debug.Log("Error", "Well, your " + Utility.BotsSettingFile + " is broken", ConsoleColor.Red);
                        Debug.Log("Error", "Do you want current " + Utility.BotsSettingFile + " and create default one?", ConsoleColor.Red);
                        Debug.Log("Error", "[Y] or [N]", ConsoleColor.Red);
                        string answer = Console.ReadLine();


                        if (answer.ToUpper() == "Y" || answer.ToUpper() == "Yes")
                        {
                            Debug.Log("System", "Create default " + Utility.BotsSettingFile + " process start...", ConsoleColor.Cyan);
                            File.Delete(Path.Combine(BotFolder[i], Utility.BotsSettingFile));
                            File.WriteAllText(Path.Combine(BotFolder[i], Utility.BotsSettingFile), JsonConvert.SerializeObject(new BotSetting(), Formatting.Indented));
                            Debug.Log("System", "Create default process finished...", ConsoleColor.Cyan);
                        }
                        else if (answer.ToUpper() == "N" || answer.ToUpper() == "No")
                        {
                            Debug.Log("System", "Well, ok then", ConsoleColor.Cyan);
                        }
                        else
                        {
                            Debug.Log("System", "Your answer either yes or no, program continue...", ConsoleColor.Cyan);
                        }
                    }
                }
                else
                {
                    /* Setting.json doesn't exist in the sub bot directory */
                    /* Create a default one for user */
                    File.WriteAllText(Path.Combine(BotFolder[i], Utility.BotsSettingFile), JsonConvert.SerializeObject(new BotSetting(), Formatting.Indented));
                }

                Debug.Log("System", "Sub directory finished: " + BotFolder[i], ConsoleColor.Cyan);
            }

            Debug.Log("System", "Setup finished...", ConsoleColor.Cyan);
        }

        /* And after finished the tree structor setup */
        /* We will check all the .dll file in mod directory that is in sub bot directory */
        static void ModSetup()
        {
            Debug.Log("System", "Mod Setup start...", ConsoleColor.Cyan);

            /* Loop all bot setup config */
            for (int i = 0; i < BotFolder.Length; i++)
            {
                /* This will check all the file in the .dll */
                ModArray = Directory.GetFiles(Path.Combine(BotFolder[i], Utility.BotsModFolder));

                /* Loop all mod dll */
                for(int j = 0; j < ModArray.Length; j++)
                {
                    /* If the file extension is not dll */
                    /* It mean this is not the module */
                    /* Because the module will have .dll extension */
                    if (Path.GetExtension(Path.GetFullPath(Path.Combine(ModArray[j]))).ToUpper() != ".DLL") break;

                    /* Read that file in the assembly variable */
                    Assembly mod = Assembly.LoadFile(Path.GetFullPath(Path.Combine(ModArray[j])));

                    /* And check all the types in that .dll */
                    for(int k = 0; k < mod.GetTypes().Length; k++)
                    {
                        /* We create a instance object for every single types in the assembly */
                        object instance = mod.CreateInstance(mod.GetTypes()[k].Name);

                        /* Then we check if this dude have CustomModule interface in it */
                        /* Because we only want to control the interface */
                        /* We don't need to work with the class */
                        ICustomModule CM = instance as ICustomModule;
                        if(CM != null)
                        {
                            /* Sweet Jesus, we got them */
                            Console.WriteLine("Have interface custom module");

                            /* Tell the interface setup the default config */
                            /* Then initialize the mod */
                            CM.ConfigSetup();
                            CM.Initialize();
                        }
                        else
                        {
                            Console.WriteLine("Have no interface custom module");
                        }
                    }
                }
            }
            Debug.Log("System", "Mod Setup finished...", ConsoleColor.Cyan);
        }
    }
}

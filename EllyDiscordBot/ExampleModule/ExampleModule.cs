using System;
using System.Threading.Tasks;
using Discord.Commands;
using EllyDiscordCore;
using EllyDiscordCore.Interface;
using EllyDiscordCore.Struct;

public class Module : CustomModule, ICustomModule
{
    public void ConfigSetup()
    {
        Console.WriteLine("This is the test module.");
    }

    public EllyDiscordCore.Struct.ModuleInfo GetInfo()
    {
        return new EllyDiscordCore.Struct.ModuleInfo(
            "Example Mod",
            "This is for the test",
            1,
            0
            );
    }

    public void Initialize()
    {
        
    }

    public void Update()
    {
        
    }
}

public class Command : ModuleBase<SocketCommandContext>
{
    [Command("Test")]
    public async Task Test()
    {
        await ReplyAsync("Test successfully");
    }
}

using System.Threading.Tasks;
using Discord.Commands;
using EllyDiscordCore;
using EllyDiscordCore.Interface;


public class AdministratorMod : CustomModule, ICustomModule
{
    public void ConfigSetup()
    {

    }

    public EllyDiscordCore.Struct.ModuleInfo GetInfo()
    {
        return new EllyDiscordCore.Struct.ModuleInfo(
            "AdministratorMod",
            "Contain command that manager your channel",
            0,
            1
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

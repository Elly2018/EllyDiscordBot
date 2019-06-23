using System.Threading.Tasks;
using Discord.WebSocket;
using EllyDiscordCore;
using EllyDiscordCore.Setup;

namespace EllyDiscordFramework
{
    public class BotHandle : BotHandleBase
    {
        public BotHandle(BotSetting botSetting) : base(botSetting) { }

        public override async Task SendMessage(SocketMessage rawMessage)
        {
            await base.SendMessage(rawMessage);
        }

        public override async Task Update()
        {
            await base.Update();
        }
    }
}

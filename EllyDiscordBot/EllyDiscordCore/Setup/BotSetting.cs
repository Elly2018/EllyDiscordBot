namespace EllyDiscordCore.Setup
{
    [System.Serializable]
    public class BotSetting
    {
        public string name;
        public string token;
        public char syntax;
        public string prefix;
        public int color;

        public BotSetting()
        {
            name = "TestBot";
            token = "";
            syntax = '!';
            prefix = "";
            color = 11;
        }
    }
}

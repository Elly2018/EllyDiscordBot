using System;
using System.Collections.Generic;
using System.Text;

namespace EllyDiscordCore.Setup
{
    public class Properties
    {
        public int UpdateMilliseconds;
        public string EmbedColor;

        public Properties()
        {
            UpdateMilliseconds = 10000;
            EmbedColor = "255:255:255";
        }
    }
}

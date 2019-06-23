using System;
using System.Collections.Generic;
using System.Text;

namespace EllyDiscordCore.Struct
{
    public class ModuleInfo
    {
        public string m_ModuleName;
        public string m_ModuleDescription;
        public int m_VersionMajor;
        public int m_VersionMinor;

        public ModuleInfo(string m_ModuleName, string m_ModuleDescription, int m_VersionMajor, int m_VersionMinor)
        {
            this.m_ModuleName = m_ModuleName;
            this.m_ModuleDescription = m_ModuleDescription;
            this.m_VersionMajor = m_VersionMajor;
            this.m_VersionMinor = m_VersionMinor;
        }
    }
}

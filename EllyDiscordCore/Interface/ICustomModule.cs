using EllyDiscordCore.Struct;

namespace EllyDiscordCore.Interface
{
    public interface ICustomModule
    {
        ModuleInfo GetInfo();

        void ConfigSetup();
        void Initialize();
        void Update();
    }
}

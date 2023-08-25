using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace RPVoiceChat
{
    public abstract class RPVoiceChatMod : ModSystem
    {
        protected RPVoiceChatConfig config;
        protected const string modID = "rpvoicechat";
        protected string[] authors = {"Ridderrasmus", "blakdragan7", "purplep_", "Dmitry221060"};

        protected INetworkChannel networkChannel;

        public override void StartPre(ICoreAPI api)
        {
            ModConfig.ReadConfig(api);
            config = ModConfig.Config;
        }

        public override void Start(ICoreAPI api)
        {
            // Register network channel
            networkChannel = api.Network.RegisterChannel("rpvoicechat")
                .RegisterMessageType(typeof(ConnectionInfo))
                .RegisterMessageType(typeof(DebugCommand));

        }
    }
}

using Vintagestory.API.Common;

namespace RPVoiceChat.Networking
{
    public class RPVoiceChatNativeNetwork
    {
        protected const string ChannelName = "RPAudioChannel";
        public RPVoiceChatNativeNetwork(ICoreAPI api)
        {
            api.Network.RegisterChannel(ChannelName)
                .RegisterMessageType<AudioPacket>()
                .RegisterMessageType<DebugCommand>();
        }
    }
}

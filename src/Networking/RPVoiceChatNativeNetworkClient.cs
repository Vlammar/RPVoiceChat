using System;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace rpvoicechat.Networking
{
    public class RPVoiceChatNativeNetworkClient : RPVoiceChatNativeNetwork
    {
        public event Action<AudioPacket> OnAudioReceived;
        public event Action<DebugCommand> OnDebugReceived;
        private IClientNetworkChannel channel;

        public RPVoiceChatNativeNetworkClient(ICoreClientAPI api) : base(api)
        {
            channel = api.Network.GetChannel(ChannelName)
                .SetMessageHandler<AudioPacket>(HandleAudioPacket)
                .SetMessageHandler<DebugCommand>(HandleDebugCommand);
        }

        private void HandleDebugCommand(DebugCommand packet)
        {
            OnDebugReceived?.Invoke(packet);
        }

        public async void SendAudioToServer(AudioPacket packet)
        {
            await Task.Run(() =>
            {
                channel.SendPacket(packet);
            });
        }

        private void HandleAudioPacket(AudioPacket packet)
        {
            OnAudioReceived?.Invoke(packet);
        }
    }
}

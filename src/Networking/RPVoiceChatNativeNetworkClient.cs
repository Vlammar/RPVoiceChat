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

        /// <summary>
        /// Sends audio data from the client to the server for it to distribute around to the clients.
        /// </summary>
        /// <param name="packet">Audio packet containing the audio data to send to the server.</param>
        public async void SendAudioToServer(AudioPacket packet)
        {
            await Task.Run(() =>
            {
                channel.SendPacket(packet);
            });
        }

        /// <summary>
        /// Invokes the audio recieved event.
        /// </summary>
        /// <param name="packet">Audio packet containing the audio data to send to the server.</param>
        private void HandleAudioPacket(AudioPacket packet)
        {
            OnAudioReceived?.Invoke(packet);
        }

        /// <summary>
        /// Invokes the debug command received event.
        /// </summary>
        /// <param name="packet">The Debugcommand object that contains debug commands being passed along.</param>
        private void HandleDebugCommand(DebugCommand packet)
        {
            OnDebugReceived?.Invoke(packet);
        }
    }
}

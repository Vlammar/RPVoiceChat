using System;
using System.Collections.Generic;
using Vintagestory.API.Server;

namespace RPVoiceChat.Networking
{
    public class RPVoiceChatNativeNetworkServer : RPVoiceChatNativeNetwork
    {
        public event Action<IServerPlayer, AudioPacket> OnReceivedPacket;
        private ICoreServerAPI api;
        private IServerNetworkChannel channel;
        private static Dictionary<VoiceLevel, string> configKeyByVoiceLevel = new Dictionary<VoiceLevel, string> 
        {
            { VoiceLevel.Whispering, "rpvoicechat:distance-whisper" },
            { VoiceLevel.Talking, "rpvoicechat:distance-talk" },
            { VoiceLevel.Shouting, "rpvoicechat:distance-shout" },
        };
        private string defaultKey = configKeyByVoiceLevel[VoiceLevel.Talking];

        public RPVoiceChatNativeNetworkServer(ICoreServerAPI api) : base(api)
        {
            this.api = api;
            channel = api.Network.GetChannel(ChannelName).SetMessageHandler<AudioPacket>(ReceivedAudioPacketFromClient);
        }

        /// <summary>
        /// This handles what to do when audio is received from a client.
        /// </summary>
        /// <param name="player">Player from which the audio was recieved.</param>
        /// <param name="packet">Audio packet containing the audio data to send to the server.</param>
        private void ReceivedAudioPacketFromClient(IServerPlayer player, AudioPacket packet)
        {
            OnReceivedPacket?.Invoke(player, packet);
            SendAudioToAllClientsInRange(player, packet);
        }

        /// <summary>
        /// Sends the audio packet to all clients within the voice level range of the audio packet from the sending players position.
        /// </summary>
        /// <param name="player">Player from which the audio was recieved.</param>
        /// <param name="packet">Audio packet containing the audio data to send to the server.</param>
        public void SendAudioToAllClientsInRange(IServerPlayer player, AudioPacket packet)
        {
            configKeyByVoiceLevel.TryGetValue(packet.VoiceLevel, out string key);

            int distance = api.World.Config.GetInt(key ?? defaultKey);
            int squareDistance = distance * distance;

            foreach (var closePlayer in api.World.AllOnlinePlayers)
            {
                if (closePlayer == player ||
                    closePlayer.Entity == null ||
                    player.Entity.Pos.SquareDistanceTo(closePlayer.Entity.Pos.XYZ) > squareDistance)
                    continue;

                channel.SendPacket(packet, closePlayer as IServerPlayer);
            }
        }

        /// <summary>
        /// Sends the given debug command to the given player.
        /// </summary>
        /// <param name="player">Player that should receive this command.</param>
        /// <param name="command">Command to be sent.</param>
        public void SendDebugCommand(IServerPlayer player, string command)
        {
            DebugCommand packet = new DebugCommand() { Command = "OpenDebugMenu"};
            channel.SendPacket(packet, player);
        }
    }
}

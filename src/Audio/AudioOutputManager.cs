using Vintagestory.API.Client;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using OpenTK.Audio.OpenAL;
using Vintagestory.API.Common;

namespace rpvoicechat
{
    public class AudioOutputManager
    {
        ICoreClientAPI capi;
        RPVoiceChatConfig _config;
        private bool isLoopbackEnabled;
        public bool IsLoopbackEnabled { 
            get => isLoopbackEnabled;

            set
            {
                isLoopbackEnabled = value;
                if (LocalPlayerAudioSource == null)
                    return;

                if (isLoopbackEnabled)
                {
                    LocalPlayerAudioSource.StartPlaying();
                }
                else
                {
                    LocalPlayerAudioSource.StopPlaying();
                }
            }
        }

        public bool isReady = false;
        public EffectsExtension EffectsExtension;
        public ConcurrentDictionary<string, PlayerAudioSource> PlayerSources = new ConcurrentDictionary<string, PlayerAudioSource>();
        public PlayerAudioSource LocalPlayerAudioSource { get; private set; }

        public AudioOutputManager(ICoreClientAPI api)
        {
            _config = ModConfig.Config;
            IsLoopbackEnabled = _config.IsLoopbackEnabled;
            capi = api;

            EffectsExtension = new EffectsExtension();
        }

        public void Launch()
        {
            isReady = true;
            capi.Event.PlayerEntitySpawn += PlayerSpawned;
            capi.Event.PlayerEntityDespawn += PlayerDespawned;
            ClientLoaded();
        }

        // Called when the client receives an audio packet supplying the audio packet
        public async void HandleAudioPacket(AudioPacket packet)
        {
            if (!isReady) return;

            await Task.Run(() =>
            {
                if (PlayerSources.TryGetValue(packet.PlayerId, out var source))
                {
                    // Update the voice level if it has changed
                    // Not sure about this one, might be better to just update the voice level every time we update the player
                    if (source.VoiceLevel != packet.VoiceLevel)
                        source.UpdateVoiceLevel(packet.VoiceLevel);

                    source.QueueAudio(packet.AudioData, packet.Length);
                }
                else
                {
                    var player = capi.World.PlayerByUid(packet.PlayerId);
                    if (player == null)
                    {
                        capi.Logger.Error("Could not find player for playerId !");
                        return;
                    }

                    var newSource = new PlayerAudioSource(player, this, capi);
                    newSource.QueueAudio(packet.AudioData, packet.Length);
                    if (!PlayerSources.TryAdd(packet.PlayerId, newSource))
                    {
                        capi.Logger.Error("Could not add new player to sources !");
                    }
                }
            });
        }

        public void HandleLoopback(byte[] audioData, int length)
        {
            if (!IsLoopbackEnabled)
                return;

            LocalPlayerAudioSource.QueueAudio(audioData, length);
        }

        public void ClientLoaded()
        {
            LocalPlayerAudioSource = new PlayerAudioSource(capi.World.Player, this, capi)
            {
                IsMuffled = false,
                IsReverberated = false,
                IsLocational = false
            };

            if (!isLoopbackEnabled) return;
            LocalPlayerAudioSource.StartPlaying();
        }

        public void PlayerSpawned(IPlayer player)
        {
            if (player.ClientId == capi.World.Player.ClientId) return;

            var playerSource = new PlayerAudioSource(player, this, capi)
            {
                IsMuffled = false,
                IsReverberated = false,
                IsLocational = true
            };

            if (PlayerSources.TryAdd(player.PlayerUID, playerSource) == false)
            {
                capi.Logger.Warning($"Failed to add player {player.PlayerName} as source !");
            }
            else
            {
                playerSource.StartPlaying();
            }
        }

        public void PlayerDespawned(IPlayer player)
        {
            if (player.ClientId == capi.World.Player.ClientId)
            {
                LocalPlayerAudioSource.Dispose();
                LocalPlayerAudioSource = null;
                return;
            }
            
            if (PlayerSources.TryRemove(player.PlayerUID, out var playerAudioSource))
            {
                playerAudioSource.Dispose();
            }
            else
            {
                capi.Logger.Warning($"Failed to remove player {player.PlayerName}");
            }
        }

        public void SpawnTestSource(string sourceKey)
        {
            var testSource = new PlayerAudioSource(null, this, capi)
            {
                IsLocational = true,
                IsMuffled = false,
                IsReverberated = false
            };

            if (PlayerSources.TryAdd(sourceKey, testSource))
            {
                testSource.StartPlaying();
            }
            else
            {
                capi.Logger.Warning($"Failed to add test source {sourceKey}");
            }
        }
    }
}


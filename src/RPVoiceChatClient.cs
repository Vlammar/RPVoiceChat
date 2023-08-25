using rpvoicechat.Networking;
using RPVoiceChat;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.CommandAbbr;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace rpvoicechat
{
    public class RPVoiceChatClient : RPVoiceChatMod
    {
        private MicrophoneManager micManager;
        private AudioOutputManager audioOutputManager;
        private PatchManager patchManager;
        private RPVoiceChatNativeNetworkClient client;

        public ICoreClientAPI capi;

        private MainConfig configGui;
        private DebugSourceListDialog debugMenu;

        private bool mutePressed = false;
        private bool voiceMenuPressed = false;
        private bool voiceLevelPressed = false;

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Client;
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;

            // Init microphone, audio output and harmony patch managers
            micManager = new MicrophoneManager(capi);
            audioOutputManager = new AudioOutputManager(capi);
            patchManager = new PatchManager(modID);
            patchManager.Patch();

            // Init voice chat client
            client = new RPVoiceChatNativeNetworkClient(api);

            // Add voice chat client event handlers
            //client.OnMessageReceived += OnMessageReceived;
            //client.OnClientConnected += VoiceClientConnected;
            //client.OnClientDisconnected += VoiceClientDisconnected;

            client.OnAudioReceived += OnAudioReceived;
            client.OnDebugReceived += OnDebugReceived;

            // Initialize guis
            configGui = new MainConfig(capi, micManager, audioOutputManager);
            debugMenu = new DebugSourceListDialog(capi, audioOutputManager);
            capi.Gui.RegisterDialog(new SpeechIndicator(capi, micManager));
            capi.Gui.RegisterDialog(new VoiceLevelIcon(capi, micManager));

            // Set up keybinds
            capi.Input.RegisterHotKey("voicechatMenu", "RPVoice: Config menu", GlKeys.P, HotkeyType.GUIOrOtherControls);
            capi.Input.RegisterHotKey("voicechatVoiceLevel", "RPVoice: Change speech volume", GlKeys.Tab, HotkeyType.GUIOrOtherControls, false, false, true);
            capi.Input.RegisterHotKey("voicechatPTT", "RPVoice: Push to talk", GlKeys.CapsLock, HotkeyType.GUIOrOtherControls);
            capi.Input.RegisterHotKey("voicechatMute", "RPVoice: Toggle mute", GlKeys.N, HotkeyType.GUIOrOtherControls);
            capi.Event.KeyUp += Event_KeyUp;

            // Set up keybind event handlers
            capi.Input.SetHotKeyHandler("voicechatMenu", (t1) =>
            {
                if (voiceMenuPressed)
                    return true;

                voiceMenuPressed = true;

                configGui.Toggle();
                return true;
            });

            capi.Input.SetHotKeyHandler("voicechatVoiceLevel", (t1) =>
            {
                if (voiceLevelPressed)
                    return true;

                voiceLevelPressed = true;

                micManager.CycleVoiceLevel();
                return true;
            });

            capi.Input.SetHotKeyHandler("voicechatMute", (t1) =>
            {
                if (mutePressed)
                    return true;

                mutePressed = true;

                config.IsMuted = !config.IsMuted;
                ModConfig.Save(capi);
                return true;
            });

            // Recording eventhandler
            micManager.OnBufferRecorded += (buffer, length, voiceLevel) =>
            {
                if (buffer == null)
                    return;

                audioOutputManager.HandleLoopback(buffer, length);

                AudioPacket packet = new AudioPacket()
                {
                    PlayerId = capi.World.Player.PlayerUID,
                    AudioData = buffer,
                    Length = length,
                    VoiceLevel = voiceLevel
                };
                client.SendAudioToServer(packet);
            };

            // Register clientside commands
            

            // At world load launch the audio managers
            capi.Event.LevelFinalize += OnLoad;
        }

        private void OnLoad()
        {
            micManager.Launch();
            audioOutputManager.Launch();
        }

        private void Event_KeyUp(KeyEvent e)
        {

            if (e.KeyCode == capi.Input.HotKeys["voicechatMenu"].CurrentMapping.KeyCode)
                voiceMenuPressed = false;
            else if (e.KeyCode == capi.Input.HotKeys["voicechatVoiceLevel"].CurrentMapping.KeyCode)
                voiceLevelPressed = false;
            else if (e.KeyCode == capi.Input.HotKeys["voicechatMute"].CurrentMapping.KeyCode)
                mutePressed = false;

        }

        // Network Events
        private void OnDebugReceived(DebugCommand command)
        {
            if (command.Command == "OpenDebugMenu")
                debugMenu.TryOpen();
        }

        private void OnAudioReceived(AudioPacket obj)
        {
            audioOutputManager.HandleAudioPacket(obj);
        }

        public override void Dispose()
        {
            micManager?.Dispose();
            patchManager?.Dispose();
            //client?.Dispose();
            configGui?.Dispose();
            debugMenu?.Dispose();
            //client = null;
        }
    }
}

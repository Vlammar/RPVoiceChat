using OpenTK.Audio.OpenAL;
using RPVoiceChat;
using System;
using System.Collections.Generic;
using Vintagestory.API.Client;

namespace RPVoiceChat
{
    public class BaseAudioSource : IDisposable
    {
        public const int BufferCount = 4;

        protected int source;

        public EffectsExtension EffectsExtension;

        protected CircularAudioBuffer buffer;
        private AudioOutputManager manager;
        private ICoreClientAPI capi;

        public bool IsMuffled { get; set; } = false;
        public bool IsReverberated { get; set; } = false;
        public bool IsLocational { get; set; } = true;
        public VoiceLevel VoiceLevel { get; set; } = VoiceLevel.Talking;
        protected static Dictionary<VoiceLevel, string> configKeyByVoiceLevel = new Dictionary<VoiceLevel, string>
        {
            { VoiceLevel.Whispering, "rpvoicechat:distance-whisper" },
            { VoiceLevel.Talking, "rpvoicechat:distance-talk" },
            { VoiceLevel.Shouting, "rpvoicechat:distance-shout" },
        };

        public BaseAudioSource (AudioOutputManager manager, ICoreClientAPI capi) 
        {
            this.manager = manager;
            this.capi = capi;
            this.EffectsExtension = manager.EffectsExtension;
        }

        public void Dispose()
        {
            AL.SourceStop(source);
            Util.CheckError("Error stop playing source", capi);

            buffer?.Dispose();
            AL.DeleteSource(source);
            Util.CheckError("Error deleting source", capi);
        }
    }
}

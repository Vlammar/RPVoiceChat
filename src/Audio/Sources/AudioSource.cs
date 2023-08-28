using HarmonyLib;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using Vintagestory.API.Client;

namespace RPVoiceChat
{
    public class AudioSource : IDisposable
    {
        private long gameTickId;
        public const int BufferCount = 4;

        protected int source;

        public EffectsExtension EffectsExtension;

        protected CircularAudioBuffer buffer;
        private AudioOutputManager manager;
        private ICoreClientAPI capi;

        public bool IsMuffled { get; set; } = false;
        public bool IsReverberated { get; set; } = false;
        public bool IsLocational { get; set; }
        protected static Dictionary<VoiceLevel, string> configKeyByVoiceLevel = new Dictionary<VoiceLevel, string>
        {
            { VoiceLevel.Whispering, "rpvoicechat:distance-whisper" },
            { VoiceLevel.Talking, "rpvoicechat:distance-talk" },
            { VoiceLevel.Shouting, "rpvoicechat:distance-shout" },
        };

        public AudioSource (AudioOutputManager manager, ICoreClientAPI capi) 
        {
            this.manager = manager;
            this.capi = capi;
            this.EffectsExtension = manager.EffectsExtension;

            StartTick();

            capi.Event.EnqueueMainThreadTask(() =>
            {
                source = AL.GenSource();
                Util.CheckError("Error gen source", capi);
                buffer = new CircularAudioBuffer(source, BufferCount, capi);

                AL.Source(source, ALSourceb.Looping, false);
                Util.CheckError("Error setting source looping", capi);
                AL.Source(source, ALSourceb.SourceRelative, false);
                Util.CheckError("Error setting source SourceRelative", capi);
                AL.Source(source, ALSourcef.Gain, 1.0f);
                Util.CheckError("Error setting source Gain", capi);
                AL.Source(source, ALSourcef.Pitch, 1.0f);
                Util.CheckError("Error setting source Pitch", capi);

            }, "AudioSource Init");
        }

        protected void StartTick()
        {
            if (gameTickId != 0)
                return;
            capi.Event.EnqueueMainThreadTask(() => { gameTickId = capi.Event.RegisterGameTickListener(UpdateSource, 100); }, "AudioSource Start");
        }

        protected void StopTick()
        {
            if (gameTickId == 0)
                return;

            capi.Event.EnqueueMainThreadTask(() =>
            {
                capi.Event.UnregisterGameTickListener(gameTickId);
                gameTickId = 0;
            }, "AudioSource Start");
        }

        public void StartPlaying()
        {
            StartTick();
            capi.Event.EnqueueMainThreadTask(() =>
            {
                AL.SourcePlay(source);
                Util.CheckError("Error playing source", capi);
            }, "AudioSource StartPlaying");
        }

        public void StopPlaying()
        {
            StopTick();
            capi.Event.EnqueueMainThreadTask(() =>
            {
                AL.SourceStop(source);
                Util.CheckError("Error stop playing source", capi);
            }, "AudioSource StopPlaying");
        }

        public void UpdateSource(float dt)
        {

        }

        public void QueueAudio(byte[] audioBytes, int bufferLength)
        {
            capi.Event.EnqueueMainThreadTask(() =>
            {
                buffer.TryDequeBuffers();
                buffer.QueueAudio(audioBytes, bufferLength, ALFormat.Mono16, MicrophoneManager.Frequency);

                var state = AL.GetSourceState(source);
                Util.CheckError("Error getting source state", capi);
                // the source can stop playing if it finishes everything in queue
                if (state != ALSourceState.Playing)
                {
                    StartPlaying();
                }
            }, "AudioSource QueueAudio");
        }

        public void Dispose()
        {
            AL.SourceStop(source);
            Util.CheckError("Error stop playing source", capi);

            buffer?.Dispose();
            AL.DeleteSource(source);
            Util.CheckError("Error deleting source", capi);

            StopTick();
        }
    }
}

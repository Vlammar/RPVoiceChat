using OpenTK.Audio.OpenAL;
using RPVoiceChat.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace RPVoiceChat.Audio
{
    public class AudioSource : IDisposable
    {
        protected const int BufferCount = 20;
        protected int source;
        protected CircularAudioBuffer buffer;
        protected SortedList orderingQueue = SortedList.Synchronized(new SortedList());
        protected int orderingDelay = 100;
        protected long lastAudioSequenceNumber = -1;
        protected Dictionary<string, IAudioCodec> codecs = new Dictionary<string, IAudioCodec>();
        protected FilterLowpass lowpassFilter;
        protected EffectsExtension effectsExtension;

        protected ICoreClientAPI capi;
        protected AudioOutputManager outputManager;

        public bool IsLocational { get; set; } = true;

        protected DateTime? lastSpeakerUpdate;

        public AudioSource(AudioOutputManager manager, ICoreClientAPI capi)
        {
            effectsExtension = manager.effectsExtension;
            outputManager = manager;
            this.capi = capi;

            lastSpeakerUpdate = DateTime.Now;

            source = OALW.GenSource();
            buffer = new CircularAudioBuffer(source, BufferCount);

            float gain = Math.Min(PlayerListener.gain, 1);
            OALW.Source(source, ALSourceb.Looping, false);
            OALW.Source(source, ALSourceb.SourceRelative, true);
            OALW.Source(source, ALSourcef.Gain, gain);
            OALW.Source(source, ALSourcef.Pitch, 1.0f);
        }

        public IAudioCodec GetOrCreateAudioCodec(int frequency, int channels)
        {
            string codecSettings = $"{frequency}:{channels}";
            IAudioCodec codec;

            if (!codecs.TryGetValue(codecSettings, out codec))
            {
                codec = new OpusCodec(frequency, channels);
                codecs.Add(codecSettings, codec);
            }

            return codec;
        }

        public void EnqueueAudio(AudioData audio, long sequenceNumber)
        {
            if (orderingQueue.ContainsKey(sequenceNumber))
            {
                Logger.client.Debug("Audio sequence already received, skipping enqueueing");
                return;
            }

            if (lastAudioSequenceNumber > sequenceNumber)
            {
                Logger.client.Debug("Audio sequence arrived too late, skipping enqueueing");
                return;
            }

            orderingQueue.Add(sequenceNumber, audio);
            capi.Event.EnqueueMainThreadTask(() =>
            {
                capi.Event.RegisterCallback(DequeueAudio, orderingDelay);
            }, "PlayerAudioSource EnqueueAudio");
        }

        public void DequeueAudio(float _)
        {
            AudioData audio;

            lock (orderingQueue.SyncRoot)
            {
                audio = orderingQueue.GetByIndex(0) as AudioData;
                lastAudioSequenceNumber = (long)orderingQueue.GetKey(0);
                orderingQueue.RemoveAt(0);
            }

            var state = OALW.GetSourceState(source);
            if (state == ALSourceState.Stopped)
                buffer.TryDequeueBuffers(); //Calling this can dequeue unprocessed audio so we want to make sure the source is stopped

            byte[] audioBytes = audio.data;
            buffer.QueueAudio(audioBytes, audioBytes.Length, audio.format, audio.frequency);

            // The source can stop playing if it finishes everything in queue
            if (state != ALSourceState.Playing)
            {
                StartPlaying();
            }
        }

        public void StartPlaying()
        {
            OALW.SourcePlay(source);
        }

        public void StopPlaying()
        {
            OALW.SourceStop(source);
        }

        public void Dispose()
        {
            OALW.SourceStop(source);

            buffer?.Dispose();
            OALW.DeleteSource(source);
        }
    }
}

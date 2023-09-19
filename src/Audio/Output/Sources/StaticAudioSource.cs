using System;
using Vintagestory.API.Client;

namespace RPVoiceChat.Audio
{
    public class StaticAudioSource : AudioSource, IAudioSource
    {
        /// <summary>
        /// Creates a static audio source.
        /// This audio source does not move relative to the world. (Used for stuff like blocks)
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="capi"></param>
        public StaticAudioSource(AudioOutputManager manager, ICoreClientAPI capi) : base(manager, capi)
        {
        }

        public void UpdateSource()
        {
            throw new NotImplementedException();
        }
    }
}

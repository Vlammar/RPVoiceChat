using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace RPVoiceChat
{
    public class StaticAudioSource : AudioSource
    {
        private AudioOutputManager manager;
        private ICoreClientAPI capi;
        private Vec3d AudioSourceWorldPos;

        public StaticAudioSource(AudioOutputManager manager, ICoreClientAPI capi, Vec3d AudioSourceWorldPos) : base(manager, capi)
        {
            this.manager = manager;
            this.capi = capi;
            this.AudioSourceWorldPos = AudioSourceWorldPos;
        }
    }
}

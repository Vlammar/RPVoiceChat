using OpenTK.Audio.OpenAL;
using RPVoiceChat.Utils;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace RPVoiceChat.Audio
{
    public class StaticAudioSource : AudioSource
    {
        private Vec3f coords;

        /// <summary>
        /// Creates a static audio source.
        /// This audio source does not move relative to the world. (Used for stuff like blocks)
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="capi"></param>
        public StaticAudioSource(Vec3f vec3f, AudioOutputManager manager, ICoreClientAPI capi) : base(manager, capi)
        {
            this.SourceType = SourceType.Static;
            coords = vec3f;
        }

        private Vec3f GetRelativeSourcePosition()
        {
            return coords - capi.World.Player.Entity?.SidedPos.AsBlockPos.ToVec3f();
        }

        public override void UpdateSource()
        {

            EntityPos listenerPos = capi.World.Player.Entity?.SidedPos;
            if (coords == null || listenerPos == null || !outputManager.isReady)
                return;


            float gain = Math.Min(PlayerListener.gain, 1);
            var sourcePosition = new Vec3f();
            var velocity = new Vec3f();
            if (IsLocational)
            {
                sourcePosition = GetRelativeSourcePosition();
            }

            OALW.ClearError();
            OALW.Source(source, ALSourcef.Gain, gain);
            OALW.Source(source, ALSource3f.Position, sourcePosition.X, sourcePosition.Y, sourcePosition.Z);
            OALW.Source(source, ALSourceb.SourceRelative, true);
        }
    }
}

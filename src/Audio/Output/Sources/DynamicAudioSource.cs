﻿using System;
using OpenTK.Audio.OpenAL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;
using RPVoiceChat.Utils;
using System.Collections.Generic;

namespace RPVoiceChat.Audio
{
    public class DynamicAudioSource : AudioSource, IAudioSource
    {
        private IPlayer player;
        public VoiceLevel voiceLevel { get; private set; } = VoiceLevel.Talking;
        private Dictionary<VoiceLevel, float> referenceDistanceByVoiceLevel = new Dictionary<VoiceLevel, float>()
        {
            { VoiceLevel.Whispering, 1.25f },
            { VoiceLevel.Talking, 2.25f },
            { VoiceLevel.Shouting, 6.25f },
        };
        private Vec3f lastSpeakerCoords;

        /// <summary>
        /// Creates a dynamic audio source.
        /// This audio source moves relative to the world.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="manager"></param>
        /// <param name="capi"></param>
        public DynamicAudioSource(IPlayer player, AudioOutputManager manager, ICoreClientAPI capi) : base(manager, capi)
        {
            this.player = player;

            lastSpeakerCoords = player.Entity?.SidedPos?.XYZFloat;

            UpdateVoiceLevel(voiceLevel);
        }

        public void UpdateVoiceLevel(VoiceLevel voiceLevel)
        {
            this.voiceLevel = voiceLevel;
            float referenceDistance = referenceDistanceByVoiceLevel[voiceLevel];
            float distanceFactor = GetDistanceFactor();
            float rolloffFactor = referenceDistance * distanceFactor;

            OALW.Source(source, ALSourcef.ReferenceDistance, referenceDistance);
            OALW.Source(source, ALSourcef.RolloffFactor, rolloffFactor);
        }

        public void UpdateSource()
        {
            EntityPos speakerPos = player.Entity?.SidedPos;
            EntityPos listenerPos = capi.World.Player.Entity?.SidedPos;
            if (speakerPos == null || listenerPos == null || !outputManager.isReady)
                return;

            // If the player is on the other side of something to the listener, then the player's voice should be muffled
            float wallThickness = LocationUtils.GetWallThickness(capi, player, capi.World.Player);
            if (capi.World.Player.Entity.Swimming)
                wallThickness += 1.0f;

            lowpassFilter?.Stop();
            if (wallThickness != 0)
            {
                lowpassFilter = lowpassFilter ?? new FilterLowpass(effectsExtension, source);
                lowpassFilter.Start();
                lowpassFilter.SetHFGain(Math.Max(1.0f - wallThickness / 2, 0.1f));
            }

            // If the player is in a reverberated area, then the player's voice should be reverberated
            bool isReverberated = false;
            if (isReverberated)
            {

            }

            // If the player has a temporal stability of less than 0.5, then the player's voice should be distorted
            // Values are temporary currently
            if (player.Entity.WatchedAttributes.GetDouble("temporalStability") < 0.5)
            {

            }

            /* --------- DISABLED FOR NOW ---------
            // If the player is drunk, then the player's voice should be affected
            // Values are temporary currently
            float drunkness = player.Entity.WatchedAttributes.GetFloat("intoxication");
            float pitch = drunkness <= 0.2 ? 1 : 1 - (drunkness / 5);
            OALW.Source(source, ALSourcef.Pitch, pitch);
            */

            float gain = Math.Min(PlayerListener.gain, 1);
            var sourcePosition = new Vec3f();
            var velocity = new Vec3f();
            if (IsLocational)
            {
                sourcePosition = GetRelativeSourcePosition(speakerPos, listenerPos);
                velocity = GetRelativeVelocity(speakerPos, listenerPos, sourcePosition);
            }

            OALW.ClearError();
            OALW.Source(source, ALSourcef.Gain, gain);
            OALW.Source(source, ALSource3f.Position, sourcePosition.X, sourcePosition.Y, sourcePosition.Z);
            OALW.Source(source, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
            OALW.Source(source, ALSourceb.SourceRelative, true);
        }

        private float GetDistanceFactor()
        {
            // Distance in blocks at which audio source normally considered quiet.
            const float quietDistance = 10;
            float maxHearingDistance = WorldConfig.GetVoiceDistance(capi, voiceLevel);
            var exponent = quietDistance < maxHearingDistance ? 2 : -0.33;
            var distanceFactor = Math.Pow(quietDistance / maxHearingDistance, exponent);

            return (float)distanceFactor;
        }

        private Vec3f GetRelativeSourcePosition(EntityPos speakerPos, EntityPos listenerPos)
        {
            var relativeSourcePosition = LocationUtils.GetRelativeSpeakerLocation(speakerPos, listenerPos);
            return relativeSourcePosition;
        }

        private Vec3f GetRelativeVelocity(EntityPos speakerPos, EntityPos listenerPos, Vec3f relativeSpeakerPosition)
        {
            var speakerVelocity = GetVelocity(speakerPos);
            var futureSpeakerPosition = speakerPos.XYZFloat + speakerVelocity;
            var relativeFuturePosition = LocationUtils.GetRelativeSpeakerLocation(futureSpeakerPosition, listenerPos);
            var relativeVelocity = relativeSpeakerPosition - relativeFuturePosition;

            return relativeVelocity;
        }

        private Vec3f GetVelocity(EntityPos speakerPos)
        {
            var currentTime = DateTime.Now;
            if (lastSpeakerUpdate == null) lastSpeakerUpdate = currentTime;
            var dt = (currentTime - (DateTime)lastSpeakerUpdate).TotalSeconds;
            dt = GameMath.Clamp(dt, 0.1, 1);

            var speakerCoords = speakerPos.XYZFloat;
            if (lastSpeakerCoords == null || dt == 1) lastSpeakerCoords = speakerCoords;

            var velocity = (lastSpeakerCoords - speakerCoords) / (float)dt;
            lastSpeakerCoords = speakerCoords;
            lastSpeakerUpdate = currentTime;

            return velocity;
        }


    }
}
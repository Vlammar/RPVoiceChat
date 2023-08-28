using System;
using OpenTK.Audio.OpenAL;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common.Entities;

namespace RPVoiceChat
{
    public class DynamicAudioSource : AudioSource
    {
        //private ReverbEffect reverbEffect;

        private ICoreClientAPI capi;

        private Vec3f lastSpeakerCoords;


        public new VoiceLevel VoiceLevel { get; private set; } = VoiceLevel.Talking;

        public IPlayer Player { get; private set; }

        private FilterLowpass lowpassFilter;

        public DynamicAudioSource(IPlayer player, AudioOutputManager manager, ICoreClientAPI capi) : base(manager, capi)
        {
            this.Player = player;
            IsLocational = true;
            
            capi.Event.EnqueueMainThreadTask(() =>
            {
                lastSpeakerCoords = player.Entity?.SidedPos?.XYZFloat;
            }, "PlayerAudioSource Init");
        }

        public void UpdateVoiceLevel(VoiceLevel voiceLevel)
        {
            VoiceLevel = voiceLevel;
            string key = configKeyByVoiceLevel[voiceLevel];

            capi.Event.EnqueueMainThreadTask(() =>
            {
                AL.Source(source, ALSourcef.MaxDistance, (float)capi.World.Config.GetInt(key));
                Util.CheckError("Error setting max audible distance", capi);
            }, "PlayerAudioSource update max distance");
        }

        public new void UpdateSource(float dt)
        {
            EntityPos speakerPos = Player.Entity?.SidedPos;
            EntityPos listenerPos = capi.World.Player.Entity?.SidedPos;
            if (speakerPos == null || listenerPos == null)

                return;

            // If the player is on the other side of something to the listener, then the player's voice should be muffled
            BlockSelection blocks = new BlockSelection();
            EntitySelection entities = new EntitySelection();
            capi.World.RayTraceForSelection(
                LocationUtils.GetLocationOfPlayer(Player),
                LocationUtils.GetLocationOfPlayer(capi),
                ref blocks,
                ref entities
            );
            if (blocks != null)
            {

                if (lowpassFilter == null)
                    lowpassFilter = new FilterLowpass(EffectsExtension, source);

                lowpassFilter?.Start();
                float muffling = 7.0f;
                //if (blocks.Block.CollisionBoxes != null)
                //    muffling = (float)blocks.Block.CollisionBoxes.Length;

                lowpassFilter?.SetHFGain(Math.Max(1.0f - (muffling / 10), 0.1f));



            }
            else
            {
                lowpassFilter?.Stop();
            }

            // If the player is in a reverberated area, then the player's voice should be reverberated
            if (IsReverberated)
            {

            }

            // If the player has a temporal stability of less than 0.7, then the player's voice should be distorted
            // Values are temporary currently
            if (Player.Entity.WatchedAttributes.GetDouble("temporalStability") < 0.5)
            {

            }

            // If the player is drunk, then the player's voice should be affected
            // Values are temporary currently
            if (Player.Entity.WatchedAttributes.GetFloat("intoxication") > 0.2)
            {
                var drunkness = Player.Entity.WatchedAttributes.GetFloat("intoxication");
                var pitch = 1 - (drunkness / 2);
                AL.Source(source, ALSourcef.Pitch, pitch);
                Util.CheckError("Error setting source Pitch", capi);
            }
            else
            {
                AL.Source(source, ALSourcef.Pitch, 1.0f);
                Util.CheckError("Error setting source Pitch", capi);
            }


            if (IsLocational)
            {
                if (lastSpeakerCoords == null)
                {
                    lastSpeakerCoords = speakerPos.XYZFloat;
                }

                var speakerCoords = speakerPos.XYZFloat;
                var velocity = (lastSpeakerCoords - speakerCoords) / dt;
                lastSpeakerCoords = speakerCoords;

                // Adjust volume change due to distance based on speaker's voice level
                string key = configKeyByVoiceLevel[VoiceLevel];
                var maxHearingDistance = capi.World.Config.GetInt(key);
                float distanceFactor = (float)(1.5 / Math.Sqrt(maxHearingDistance));
                var relativeSpeakerCoords = LocationUtils.GetRelativeSpeakerLocation(speakerPos, listenerPos);

                var sourcePosition = relativeSpeakerCoords * distanceFactor;

                AL.Source(source, ALSource3f.Position, sourcePosition.X, sourcePosition.Y, sourcePosition.Z);
                Util.CheckError("Error setting source pos", capi);

                AL.Source(source, ALSource3f.Velocity, velocity.X, velocity.Y, velocity.Z);
                Util.CheckError("Error setting source velocity", capi);

                AL.Source(source, ALSourceb.SourceRelative, true);
                Util.CheckError("Error making source relative to client", capi);
            }
            else
            {
                AL.Source(source, ALSource3f.Position, 0, 0, 0);
                Util.CheckError("Error setting source direction", capi);

                AL.Source(source, ALSource3f.Velocity, 0, 0, 0);
                Util.CheckError("Error setting source velocity", capi);

                AL.Source(source, ALSourceb.SourceRelative, true);
                Util.CheckError("Error making source relative to client", capi);
            }
        }

        

        

        
    }
}
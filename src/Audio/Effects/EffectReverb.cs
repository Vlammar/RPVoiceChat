using OpenTK.Audio.OpenAL;
using System.Collections.Generic;

namespace RPVoiceChat.Audio
{
    public class ReverbEffect : EffectBase
    {
        // Literally just used copilot for these values. I have no idea what they mean.
        private Dictionary<string, float[]> reverbPresets = new Dictionary<string, float[]>
        {
            { "Default", new float[] { 3.0f, 0.91f, 0.7f, 0.9f, 3.1f, 0.723f, 0.03f, 0.23f } },
            { "Cave", new float[] { 2.91f, 1.3f, 1.0f, 0.5f, 2.91f, 0.5f, 0.1f, 0.23f } },
            { "Something else", new float[] { 3.0f, 0.91f, 0.7f, 0.9f, 3.1f, 0.723f, 0.03f, 0.23f } }
        };


        public ReverbEffect(EffectsExtension efx, int source) : base(efx, source, EfxEffectType.Reverb)
        {
            SetEffectProperty(EfxEffectf.ReverbDecayTime, 3.0f);
            SetEffectProperty(EfxEffectf.ReverbDecayHFRatio, 0.91f);
            SetEffectProperty(EfxEffectf.ReverbDensity, 0.7f);
            SetEffectProperty(EfxEffectf.ReverbDiffusion, 0.9f);
            SetEffectProperty(EfxEffectf.ReverbRoomRolloffFactor, 3.1f);
            SetEffectProperty(EfxEffectf.ReverbReflectionsGain, 0.723f);
            SetEffectProperty(EfxEffectf.ReverbReflectionsDelay, 0.03f);
            SetEffectProperty(EfxEffectf.ReverbGain, 0.23f);
        }

        public void SetPreset(string preset)
        {
            float[] result = new float[] { 3.0f, 0.91f, 0.7f, 0.9f, 3.1f, 0.723f, 0.03f, 0.23f };
            reverbPresets.TryGetValue(preset, out result);

            SetDecayTime(result[0]);
            SetDecayHFRatio(result[1]);
            SetDensity(result[2]);
            SetDiffusion(result[3]);
            SetRoomRolloffFactor(result[4]);
            SetReflectionsGain(result[5]);
            SetReflectionsDelay(result[6]);
            SetGain(result[7]);
        }

        public void SetDecayTime(float decayTime)
        {
            SetEffectProperty(EfxEffectf.ReverbDecayTime, decayTime);
        }

        public void SetDecayHFRatio(float decayHFRatio)
        {
            SetEffectProperty(EfxEffectf.ReverbDecayHFRatio, decayHFRatio);
        }

        public void SetDensity(float density)
        {
            SetEffectProperty(EfxEffectf.ReverbDensity, density);
        }

        public void SetDiffusion(float diffusion)
        {
            SetEffectProperty(EfxEffectf.ReverbDiffusion, diffusion);
        }

        public void SetRoomRolloffFactor(float roomRolloffFactor)
        {
            SetEffectProperty(EfxEffectf.ReverbRoomRolloffFactor, roomRolloffFactor);
        }

        public void SetReflectionsGain(float reflectionsGain)
        {
            SetEffectProperty(EfxEffectf.ReverbReflectionsGain, reflectionsGain);
        }

        public void SetReflectionsDelay(float reflectionsDelay)
        {
            SetEffectProperty(EfxEffectf.ReverbReflectionsDelay, reflectionsDelay);
        }

        public void SetGain(float gain)
        {
            SetEffectProperty(EfxEffectf.ReverbGain, gain);
        }
    }
}

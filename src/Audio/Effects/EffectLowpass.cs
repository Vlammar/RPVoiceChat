using OpenTK.Audio.OpenAL;

namespace RPVoiceChat.Audio
{
    public class EffectLowpass : EffectBase
    {
        public EffectLowpass(EffectsExtension efx, int source)  : base(efx, source ,EfxEffectType.Equalizer)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighCutoff, 0f);
            SetEffectProperty(EfxEffectf.EqualizerHighGain, 0f);
        }

        public void SetGain(float gain)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighGain, gain);
        }

        public void SetCutoff(float cutoff)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighCutoff, cutoff);
        }
    }
}

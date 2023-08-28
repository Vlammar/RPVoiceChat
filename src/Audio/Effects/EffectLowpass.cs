using OpenTK.Audio.OpenAL;

namespace RPVoiceChat
{
    public class EffectLowpass : EffectBase
    {
        public EffectLowpass(EffectsExtension effectsExtension, int source) : base(effectsExtension, source)
        {
            // Not sure this is gonna give the best effect as we have no choice over the frequencies being affected
            GenerateEffect(EfxEffectType.Equalizer);

            SetEffectSetting(EfxEffectf.EqualizerHighCutoff, 0f);
            SetEffectSetting(EfxEffectf.EqualizerHighGain, 0f);
        }

        public void SetGain(float gain)
        {
            SetEffectSetting(EfxEffectf.EqualizerHighGain, gain);
        }

        public void SetCutoff(float cutoff)
        {
            SetEffectSetting(EfxEffectf.EqualizerHighCutoff, cutoff);
        }
    }
}

using OpenTK.Audio.OpenAL;

namespace RPVoiceChat
{
    public class EffectLowpass : EffectBase
    {

        // Without finding out some way to make a custom sound effect
        // this equalizer effect is the closest we can get to a lowpass
        // filter as an effect

        public EffectLowpass(EffectsExtension effectsExtension, int source) : base(effectsExtension, source)
        {
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

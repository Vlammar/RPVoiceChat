using OpenTK.Audio.OpenAL;
using System;

namespace RPVoiceChat.Audio
{
    public class EffectLowpass : EffectBase
    {
        public EffectLowpass(EffectsExtension efx, int source)  : base(efx, source ,EfxEffectType.Equalizer)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighCutoff, 4000f);
            SetEffectProperty(EfxEffectf.EqualizerHighGain, 1f);
        }

        /// <summary>
        /// Set's gain of the high frequency band
        /// </summary>
        /// <param name="gain">The total gain</param>
        public void SetHFGain(float gain)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighGain, Math.Max(gain, 0.126f));
        }

        /// <summary>
        /// Set's the cutoff frequency of the high frequency band <br/>
        /// Max value is 16000.0f, min value is 4000.0f <br/>
        /// Default value is 6000.0f
        /// </summary>
        /// <param name="cutoff">The cutoff frequency</param>
        public void SetHFCutoff(float cutoff)
        {
            SetEffectProperty(EfxEffectf.EqualizerHighCutoff, cutoff);
        }
    }
}

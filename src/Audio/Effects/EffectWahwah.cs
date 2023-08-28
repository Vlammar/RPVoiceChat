using OpenTK.Audio.OpenAL;
using System;
using System.Runtime.CompilerServices;

namespace RPVoiceChat
{
    public class EffectWahwah : EffectBase
    {

        public EffectWahwah(EffectsExtension effectsExtension, int source) : base(effectsExtension, source)
        {
            GenerateEffect(EfxEffectType.Autowah);

            SetEffectSetting(EfxEffectf.AutowahResonance, 1f);
            SetEffectSetting(EfxEffectf.AutowahReleaseTime, 1f);
            SetEffectSetting(EfxEffectf.AutowahPeakGain, 1f);
            SetEffectSetting(EfxEffectf.AutowahAttackTime, 1f);
        }
    }
}

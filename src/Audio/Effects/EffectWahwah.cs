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
        }


        public override void GenerateEffect(EfxEffectType t)
        {
            base.GenerateEffect(t);

            effectsExtension.Effect(effect, EfxEffectf.AutowahResonance, 1f);
            effectsExtension.Effect(effect, EfxEffectf.AutowahReleaseTime, 1f);
            effectsExtension.Effect(effect, EfxEffectf.AutowahPeakGain, 1f);
            effectsExtension.Effect(effect, EfxEffectf.AutowahAttackTime, 1f);

        }
    }
}

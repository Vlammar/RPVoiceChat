using OpenTK.Audio.OpenAL;

namespace RPVoiceChat
{
    public class EffectReverb : EffectBase
    {


        public EffectReverb(EffectsExtension effectsExtension, int source) : base(effectsExtension, source)
        {
            GenerateEffect(EfxEffectType.Reverb);
        }

        public override void GenerateEffect(EfxEffectType t)
        {
            base.GenerateEffect(t);

            effectsExtension.Effect(effect, EfxEffectf.ReverbDecayTime, 3.0f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbDecayHFRatio, 0.91f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbDensity, 0.7f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbDiffusion, 0.9f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbRoomRolloffFactor, 3.1f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbReflectionsGain, 0.723f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbReflectionsDelay, 0.03f);
            effectsExtension.Effect(effect, EfxEffectf.ReverbGain, 0.23f);
        }

    }
}

using OpenTK.Audio.OpenAL;

namespace RPVoiceChat
{
    public class EffectReverb : EffectBase
    {
        public EffectReverb(EffectsExtension effectsExtension, int source) : base(effectsExtension, source)
        {
            GenerateEffect(EfxEffectType.Reverb);

            SetEffectSetting(EfxEffectf.ReverbDecayTime, 3.0f);
            SetEffectSetting(EfxEffectf.ReverbDecayHFRatio, 0.91f);
            SetEffectSetting(EfxEffectf.ReverbDensity, 0.7f);
            SetEffectSetting(EfxEffectf.ReverbDiffusion, 0.9f);
            SetEffectSetting(EfxEffectf.ReverbRoomRolloffFactor, 3.1f);
            SetEffectSetting(EfxEffectf.ReverbReflectionsGain, 0.723f);
            SetEffectSetting(EfxEffectf.ReverbReflectionsDelay, 0.03f);
            SetEffectSetting(EfxEffectf.ReverbGain, 0.23f);
        }
    }
}

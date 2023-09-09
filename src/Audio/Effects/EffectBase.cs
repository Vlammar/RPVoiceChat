using OpenTK;
using OpenTK.Audio.OpenAL;
using RPVoiceChat.Audio;

namespace RPVoiceChat
{
    public class EffectBase
    {
        private EfxEffectType effectType;
        private EffectsExtension effectsExtension;
        private int source;
        private int effect;
        private int slot;

        public bool IsEnabled { get; set; } = false;

        public EffectBase(EffectsExtension effectsExtension, int source, EfxEffectType efxType)
        {
            this.effectsExtension = effectsExtension;
            this.source = source;
            this.effectType = efxType;
            GenerateEffect(efxType);
        }

        private void GenerateEffect(EfxEffectType type)
        {
            OALW.ExecuteInContext(() =>
            {
                effect = effectsExtension.GenEffect();
                slot = effectsExtension.GenAuxiliaryEffectSlot();

                effectsExtension.AuxiliaryEffectSlot(slot, EfxAuxiliaryi.EffectslotEffect, effect);
                effectsExtension.BindSourceToAuxiliarySlot(source, slot, 0, 0);
            });
        }

        protected void SetEffectProperty(EfxEffectf param, float value)
        {
            OALW.ExecuteInContext(() =>
            {
                effectsExtension.Effect(effect, param, value);
            });
        }

        protected void SetEffectProperty(EfxEffecti param, int value)
        {
            OALW.ExecuteInContext(() =>
            {
                effectsExtension.Effect(effect, param, value);
            });
        }

        protected void SetEffectProperty(EfxEffect3f param, float value1, float value2, float value3)
        {
            var params3f = new Vector3(value1, value2, value3);
            OALW.ExecuteInContext(() =>
            {
                effectsExtension.Effect(effect, param, ref params3f);
            });
        }

        protected void SetEffectProperty(EfxEffect3f param, float[] values)
        {
            SetEffectProperty(param, values[0], values[1], values[2]);
        }


        public void Start()
        {
            if(IsEnabled)
            {
                IsEnabled = true;

                OALW.ExecuteInContext(() =>
                {
                    effectsExtension.BindEffect(effect, effectType);
                });
            }
        }

        public void Stop()
        {
            if(IsEnabled)
            {
                IsEnabled = false;

                OALW.ExecuteInContext(() =>
                {
                    effectsExtension.BindEffect(effect, EfxEffectType.Null);
                });
            }
        }
    }
}

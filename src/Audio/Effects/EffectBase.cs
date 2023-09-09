using Newtonsoft.Json.Linq;
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
            GenerateEffect();
        }

        private void GenerateEffect()
        {
            OALW.ExecuteInContext(() =>
            {
                effect = effectsExtension.GenEffect();
                slot = effectsExtension.GenAuxiliaryEffectSlot();


                effectsExtension.AuxiliaryEffectSlot(slot, EfxAuxiliaryi.EffectslotEffect, effect);
                effectsExtension.BindSourceToAuxiliarySlot(source, slot, 0, 0);
            });
            OALW.CheckError($"Error generating effect {effectType}");
        }

        protected void SetEffectProperty(EfxEffectf param, float value)
        {
            effectsExtension.Effect(effect, param, value);
            OALW.CheckError($"Error setting effect {param} with {value}");
        }

        protected void SetEffectProperty(EfxEffecti param, int value)
        {
            effectsExtension.Effect(effect, param, value);
            OALW.CheckError($"Error setting effect {param} with {value}");
        }

        protected void SetEffectProperty(EfxEffect3f param, float value1, float value2, float value3)
        {
            var params3f = new Vector3(value1, value2, value3);
            effectsExtension.Effect(effect, param, ref params3f);

            OALW.CheckError($"Error setting effect {param} with {value1}, {value2}, and {value3}");
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
                OALW.CheckError($"Error binding effect {effectType}");
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
                OALW.CheckError($"Error unbinding effect {effectType}");
            }
        }
    }
}

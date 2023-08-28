using OpenTK.Audio.OpenAL;
using System;

namespace RPVoiceChat
{
    public class EffectBase
    {
        protected EffectsExtension effectsExtension;
        protected int source;
        public int effect;
        public int slot;

        private EfxEffectType effectType;

        public bool IsEnabled { get; set; } = false;

        public EffectBase(EffectsExtension effectsExtension, int source)
        {
            this.effectsExtension = effectsExtension;
            this.source = source;
        }

        // Override this with base.Start() in child classes
        public void Start()
        {
            if (IsEnabled) return;
            IsEnabled = true;

            effectsExtension.BindEffect(effect, effectType);
        }

        // Override with base.Stop() in child classes
        public void Stop()
        {
            if (!IsEnabled) return;
            IsEnabled = false;

            effectsExtension.BindEffect(effect, EfxEffectType.Null);
        }

        // Override this
        public virtual void GenerateEffect(EfxEffectType type)
        {
            effect = effectsExtension.GenEffect();
            slot = effectsExtension.GenAuxiliaryEffectSlot();

            effectsExtension.AuxiliaryEffectSlot(slot, EfxAuxiliaryi.EffectslotEffect, effect);
            effectsExtension.BindSourceToAuxiliarySlot(source, slot, 0, 0);
        }
    }
}

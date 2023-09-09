using OpenTK.Audio.OpenAL;

namespace RPVoiceChat.Audio
{
    public class EffectPitchShift : EffectBase
    {
        public EffectPitchShift(EffectsExtension efx, int source) : base(efx, source, EfxEffectType.PitchShifter)
        {
            SetEffectProperty(EfxEffecti.PitchShifterCoarseTune, 0);
            SetEffectProperty(EfxEffecti.PitchShifterFineTune, 0);
        }

        public void SetCoarseTune(int coarseTune)
        {
            SetEffectProperty(EfxEffecti.PitchShifterCoarseTune, coarseTune);
        }

        public void SetFineTune(int fineTune)
        {
            SetEffectProperty(EfxEffecti.PitchShifterFineTune, fineTune);
        }

        /// <summary>
        /// Sets the total tune change. <br/>
        /// Max value is 12.0f, min value is -12.0f.
        /// </summary>
        /// <param name="tune">The pitch change in semitones with the decimals being cents</param>
        public void SetTotalTune(float tune)
        {
            if (tune > 12.0f) tune = 12.0f;
            if (tune < -12.0f) tune = -12.0f;

            int coarseTune = (int)tune;
            int fineTune = (int)((tune - coarseTune) * 100);

            SetEffectProperty(EfxEffecti.PitchShifterCoarseTune, coarseTune);
            SetEffectProperty(EfxEffecti.PitchShifterFineTune, fineTune);
        }
    }
}

namespace RPVoiceChat.Audio
{
    public interface IAudioSource
    {
        public void UpdateSource();
        public IAudioCodec GetOrCreateAudioCodec(int frequency, int channels);

        public void EnqueueAudio(AudioData audio, long sequenceNumber);

        public void DequeueAudio(float _);

        public void StartPlaying();
        public void StopPlaying();
    }
}

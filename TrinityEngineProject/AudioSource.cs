using NAudio.Wave;

namespace TrinityEngineProject
{
    internal class AudioSource : Component
    {
        private WaveOutEvent waveOut;
        private AudioFileReader audioFile;

        public AudioSource(string filePath)
        {
            waveOut = new WaveOutEvent();
            audioFile = new AudioFileReader(filePath);
            waveOut.Init(audioFile);
        }

        public void Play()
        {
            Stop();
            waveOut.Play();
        }

        public void Stop()
        {
            if (waveOut.PlaybackState == PlaybackState.Playing)
            {
                waveOut.Stop();
            }
        }
    }
}

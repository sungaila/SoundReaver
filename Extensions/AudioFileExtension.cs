using NAudio.Wave;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sungaila.SoundReaver.Extensions
{
    internal static class AudioFileExtension
    {
        private const float STEPS = 20f;

        public static async Task FadeInAsync(this AudioFileReader audioFile, TimeSpan duration, float targetVolume = 1.0f, CancellationToken cancellationToken = default)
        {
            float interval = (float)(duration.TotalMilliseconds / STEPS);
            float volumeStep = targetVolume / STEPS;

            for (int i = 0; i <= STEPS; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                audioFile.Volume = volumeStep * i;
                await Task.Delay((int)interval, cancellationToken);
            }

            audioFile.Volume = targetVolume;
        }

        public static async Task FadeOutAsync(this AudioFileReader audioFile, TimeSpan duration, CancellationToken cancellationToken = default)
        {
            float interval = (float)(duration.TotalMilliseconds / STEPS);
            float initialVolume = audioFile.Volume;
            float volumeStep = initialVolume / STEPS;

            for (int i = 0; i <= STEPS; i++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                audioFile.Volume = initialVolume - volumeStep * i;
                await Task.Delay((int)interval, cancellationToken);
            }

            audioFile.Volume = 0.0f;
        }
    }
}

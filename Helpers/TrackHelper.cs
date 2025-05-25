using NAudio.Wave;
using System.Collections.Generic;
using System.IO;

namespace Sungaila.SoundReaver.Helpers
{
    internal static class TrackHelper
    {
        private static readonly Dictionary<string, AudioFileReader> _files = [];

        public static IReadOnlyDictionary<string, AudioFileReader> Files => _files.AsReadOnly();

        static TrackHelper()
        {
            foreach (var file in Directory.GetFiles(AssetsHelper.GetFullPath(@"Music"), "*.mp3"))
            {
                try
                {
                    _files.TryAdd(Path.GetFileName(file), new AudioFileReader(file));
                }
                catch
                {
                }
            }
        }
    }
}

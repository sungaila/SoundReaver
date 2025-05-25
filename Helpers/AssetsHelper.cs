using System;
using System.IO;
using Windows.ApplicationModel;

namespace Sungaila.SoundReaver.Helpers
{
    internal static class AssetsHelper
    {
        public static string GetFullPath(string relativePath)
        {
            string basePath;

            try
            {
                basePath = Package.Current.InstalledLocation.Path;
            }
            catch (Exception)
            {
                // if an exception is thrown -> this app might be unpackaged
                basePath = AppContext.BaseDirectory;
            }

            return Path.Combine(basePath, relativePath);
        }
    }
}
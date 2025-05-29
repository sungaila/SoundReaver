using Microsoft.UI.Xaml.Media.Imaging;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Search;

namespace Sungaila.SoundReaver.Manager
{
    public static class TrackManager
    {
        private static bool _initialized = false;
        private static readonly Dictionary<Package, List<StorageFile>> _files = [];

        public static IReadOnlyDictionary<Package, List<StorageFile>> Files => _files.AsReadOnly();

        public static async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            await InitializeAsync();
        }

        public static async Task InitializeAsync()
        {
            try
            {
                _files.Clear();

                var optionalMusicPacks = Package.Current.Dependencies
                    .Where(d => d.IsOptional && d.Id.Name.StartsWith(Package.Current.Id.Name + ".Music."))
                    .ToList();

                foreach (var pack in optionalMusicPacks)
                {
                    try
                    {
                        var folder = await pack.InstalledLocation.GetFolderAsync("Tracks");
                        var files = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                        _files.Add(pack, [.. files.Where(f => f.FileType.Equals(".ogg", StringComparison.OrdinalIgnoreCase))]);
                    }
                    catch { }
                }
            }
            finally
            {
                _initialized = true;
            }
        }

        public static async IAsyncEnumerable<CategoryViewModel> GenerateViewModelsAsync()
        {
            await EnsureInitializedAsync();

            foreach (var pair in _files)
            {
                StorageFile? GetFile(string fileName) => pair.Value.SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

                var logoFolder = await pair.Key.InstalledLocation.TryGetItemAsync("Assets") as StorageFolder;
                var logoBitmap = logoFolder != null ? await CreateBitmapFromLogoAsync(await logoFolder.TryGetItemAsync("StoreLogo.png") as StorageFile) : null;

                if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.Underworld")
                {
                    yield return new()
                    {
                        Name = "Underworld",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 002 - Underworld.ogg"), Spectral = GetFile("1 - 013 - Underworld [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 003 - Underworld [Suspense].ogg"), Spectral = GetFile("1 - 014 - Underworld [Spectral - Suspense].ogg") },
                            new() { Name = "Abyss", Material = GetFile("1 - 004 - Underworld [Abyss].ogg"), Spectral = GetFile("1 - 015 - Underworld [Spectral - Abyss].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 005 - Underworld [Puzzle].ogg"), Spectral = GetFile("1 - 016 - Underworld [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 006 - Underworld [Danger].ogg"), Spectral = GetFile("1 - 017 - Underworld [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 007 - Underworld [Combat].ogg"), Spectral = GetFile("1 - 018 - Underworld [Spectral - Combat].ogg") },
                            new() { Name = "Normal (Exterior)", Material = GetFile("1 - 008 - Underworld (Exterior).ogg"), Spectral = GetFile("1 - 019 - Underworld (Exterior) [Spectral].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 009 - Underworld (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 020 - Underworld (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Exterior)", Material = GetFile("1 - 010 - Underworld (Exterior) [Combat].ogg"), Spectral = GetFile("1 - 021 - Underworld (Exterior) [Spectral - Combat].ogg") },
                            new() { Name = "Kain Encounter (Anticipation)", Material = GetFile("1 - 061 - Kain Encounter (Anticipation).ogg"), Spectral = GetFile("1 - 062 - Kain Encounter (Anticipation) [Spectral].ogg") },
                            new() { Name = "Kain Encounter", Material = GetFile("1 - 063 - Kain Encounter.ogg"), Spectral = GetFile("1 - 064 - Kain Encounter [Spectral].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.RazielsStronghold")
                {
                    yield return new()
                    {
                        Name = "Raziel’s Stronghold",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 025 - Raziel's Stronghold.ogg"), Spectral = GetFile("1 - 032 - Raziel's Stronghold [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 026 - Raziel's Stronghold [Suspense].ogg"), Spectral = GetFile("1 - 033 - Raziel's Stronghold [Spectral - Suspense].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 027 - Raziel's Stronghold [Danger].ogg"), Spectral = GetFile("1 - 034 - Raziel's Stronghold [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 028 - Raziel's Stronghold [Combat].ogg"), Spectral = GetFile("1 - 035 - Raziel's Stronghold [Spectral - Combat].ogg") },
                            new() { Name = "Suspense (Exterior)", Material = GetFile("1 - 029 - Raziel's Stronghold (Exterior) [Suspense].ogg"), Spectral = GetFile("1 - 036 - Raziel's Stronghold (Exterior) [Spectral - Suspense].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 030 - Raziel's Stronghold (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 037 - Raziel's Stronghold (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Exterior)", Material = GetFile("1 - 031 - Raziel's Stronghold (Exterior) [Combat].ogg"), Spectral = GetFile("1 - 038 - Raziel's Stronghold (Exterior) [Spectral - Combat].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.Necropolis")
                {
                    yield return new()
                    {
                        Name = "Necropolis",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 039 - Necropolis.ogg"), Spectral = GetFile("1 - 052 - Necropolis [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 040 - Necropolis [Suspense].ogg"), Spectral = GetFile("1 - 053 - Necropolis [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 041 - Necropolis [Puzzle].ogg"), Spectral = GetFile("1 - 054 - Necropolis [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 042 - Necropolis [Danger].ogg"), Spectral = GetFile("1 - 055 - Necropolis [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 044 - Necropolis [Combat].ogg"), Spectral = GetFile("1 - 056 - Necropolis [Spectral - Combat].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.SilencedCathedral")
                {
                    yield return new()
                    {
                        Name = "Silenced Cathedral",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 065 - Silenced Cathedral.ogg"), Spectral = GetFile("1 - 076 - Silenced Cathedral [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 066 - Silenced Cathedral [Suspense].ogg"), Spectral = GetFile("1 - 077 - Silenced Cathedral [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 067 - Silenced Cathedral [Puzzle].ogg"), Spectral = GetFile("1 - 078 - Silenced Cathedral [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 068 - Silenced Cathedral [Danger].ogg"), Spectral = GetFile("1 - 079 - Silenced Cathedral [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 069 - Silenced Cathedral [Combat].ogg"), Spectral = GetFile("1 - 080 - Silenced Cathedral [Spectral - Combat].ogg") },
                            new() { Name = "Suspense (Exterior)", Material = GetFile("1 - 070 - Silenced Cathedral (Exterior) [Suspense].ogg"), Spectral = GetFile("1 - 081 - Silenced Cathedral (Exterior) [Spectral - Suspense].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 071 - Silenced Cathedral (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 082 - Silenced Cathedral (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Exterior)", Material = GetFile("1 - 072 - Silenced Cathedral (Exterior) [Combat].ogg"), Spectral = GetFile("1 - 083 - Silenced Cathedral (Exterior) [Spectral - Combat].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.HumanCitadel")
                {
                    yield return new()
                    {
                        Name = "Human Citadel",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 087 - Human Citadel.ogg"), Spectral = GetFile("1 - 099 - Human Citadel [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 088 - Human Citadel [Suspense].ogg"), Spectral = GetFile("1 - 100 - Human Citadel [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 089 - Human Citadel [Puzzle].ogg"), Spectral = GetFile("1 - 101 - Human Citadel [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 090 - Human Citadel [Danger].ogg"), Spectral = GetFile("1 - 102 - Human Citadel [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 091 - Human Citadel [Combat].ogg"), Spectral = GetFile("1 - 103 - Human Citadel [Spectral - Combat].ogg") },
                            new() { Name = "Normal (Water Tower)", Material = GetFile("1 - 092 - Human Citadel (Water Tower).ogg"), Spectral = GetFile("1 - 104 - Human Citadel (Water Tower) [Spectral].ogg") },
                            new() { Name = "Danger (Water Tower)", Material = GetFile("1 - 093 - Human Citadel (Water Tower) [Danger].ogg"), Spectral = GetFile("1 - 105 - Human Citadel (Water Tower) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Water Tower)", Material = GetFile("1 - 094 - Human Citadel (Water Tower) [Combat].ogg"), Spectral = GetFile("1 - 106 - Human Citadel (Water Tower) [Spectral - Combat].ogg") },
                            new() { Name = "Unused (Undercity)", Material = GetFile("1 - 095 - Human Citadel (Undercity) [Unused].ogg"), Spectral = GetFile("1 - 107 - Human Citadel (Undercity) [Spectral - Unused].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.SarafanTomb")
                {
                    yield return new()
                    {
                        Name = "Sarafan Tomb",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 111 - Sarafan Tomb.ogg"), Spectral = GetFile("1 - 123 - Sarafan Tomb [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 112 - Sarafan Tomb [Suspense].ogg"), Spectral = GetFile("1 - 124 - Sarafan Tomb [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 113 - Sarafan Tomb [Puzzle].ogg"), Spectral = GetFile("1 - 125 - Sarafan Tomb [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 114 - Sarafan Tomb [Danger].ogg"), Spectral = GetFile("1 - 126 - Sarafan Tomb [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 115 - Sarafan Tomb [Combat].ogg"), Spectral = GetFile("1 - 127 - Sarafan Tomb [Spectral - Combat].ogg") },
                            new() { Name = "Suspense (Exterior)", Material = GetFile("1 - 116 - Sarafan Tomb (Exterior) [Suspense].ogg"), Spectral = GetFile("1 - 128 - Sarafan Tomb (Exterior) [Spectral - Suspense].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 117 - Sarafan Tomb (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 129 - Sarafan Tomb (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Exterior)", Material = GetFile("1 - 118 - Sarafan Tomb (Exterior) [Combat].ogg"), Spectral = GetFile("1 - 130 - Sarafan Tomb (Exterior) [Spectral - Combat].ogg") },
                            new() { Name = "Tomb Guardian Encounter", Material = GetFile("1 - 132 - Tomb Guardian Encounter.ogg"), Spectral = GetFile("1 - 133 - Tomb Guardian Encounter [Spectral].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.DrownedAbbey")
                {
                    yield return new()
                    {
                        Name = "Drowned Abbey",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 134 - Drowned Abbey.ogg"), Spectral = GetFile("1 - 144 - Drowned Abbey [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 135 - Drowned Abbey [Suspense].ogg"), Spectral = GetFile("1 - 145 - Drowned Abbey [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 136 - Drowned Abbey [Puzzle].ogg"), Spectral = GetFile("1 - 146 - Drowned Abbey [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 137 - Drowned Abbey [Danger].ogg"), Spectral = GetFile("1 - 147 - Drowned Abbey [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 138 - Drowned Abbey [Combat].ogg"), Spectral = GetFile("1 - 148 - Drowned Abbey [Spectral - Combat].ogg") },
                            new() { Name = "Suspense (Exterior)", Material = GetFile("1 - 139 - Drowned Abbey (Exterior) [Suspense].ogg"), Spectral = GetFile("1 - 149 - Drowned Abbey (Exterior) [Spectral - Suspense].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 140 - Drowned Abbey (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 150 - Drowned Abbey (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Unused (Underwater)", Material = GetFile("1 - 141 - Drowned Abbey (Underwater).ogg"), Spectral = GetFile("1 - 151 - Drowned Abbey (Underwater) [Spectral - Unused].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.RuinedCity")
                {
                    yield return new()
                    {
                        Name = "Ruined City",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 156 - Ruined City.ogg"), Spectral = GetFile("1 - 163 - Ruined City [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 157 - Ruined City [Suspense].ogg"), Spectral = GetFile("1 - 164 - Ruined City [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 158 - Ruined City [Puzzle].ogg"), Spectral = GetFile("1 - 165 - Ruined City [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 159 - Ruined City [Danger].ogg"), Spectral = GetFile("1 - 166 - Ruined City [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 160 - Ruined City [Combat].ogg"), Spectral = GetFile("1 - 167 - Ruined City [Spectral - Combat].ogg") }
                        ]
                    };
                }
                else if (pair.Key.Id.Name == "Sungaila.SoundReaver.Music.TheLighthouse")
                {
                    yield return new()
                    {
                        Name = "The Lighthouse",
                        Logo = logoBitmap,
                        Tracks = [
                            new() { Name = "Normal", Material = GetFile("1 - 169 - The Lighthouse.ogg"), Spectral = GetFile("1 - 181 - The Lighthouse [Spectral].ogg") },
                            new() { Name = "Suspense", Material = GetFile("1 - 170 - The Lighthouse [Suspense].ogg"), Spectral = GetFile("1 - 182 - The Lighthouse [Spectral - Suspense].ogg") },
                            new() { Name = "Puzzle", Material = GetFile("1 - 171 - The Lighthouse [Puzzle].ogg"), Spectral = GetFile("1 - 183 - The Lighthouse [Spectral - Puzzle].ogg") },
                            new() { Name = "Danger", Material = GetFile("1 - 172 - The Lighthouse [Danger].ogg"), Spectral = GetFile("1 - 184 - The Lighthouse [Spectral - Danger].ogg") },
                            new() { Name = "Combat", Material = GetFile("1 - 173 - The Lighthouse [Combat].ogg"), Spectral = GetFile("1 - 185 - The Lighthouse [Spectral - Combat].ogg") },
                            new() { Name = "Suspense (Exterior)", Material = GetFile("1 - 175 - The Lighthouse (Exterior) [Suspense].ogg"), Spectral = GetFile("1 - 186 - The Lighthouse (Exterior) [Spectral - Suspense].ogg") },
                            new() { Name = "Danger (Exterior)", Material = GetFile("1 - 176 - The Lighthouse (Exterior) [Danger].ogg"), Spectral = GetFile("1 - 187 - The Lighthouse (Exterior) [Spectral - Danger].ogg") },
                            new() { Name = "Combat (Exterior)", Material = GetFile("1 - 177 - The Lighthouse (Exterior) [Combat].ogg"), Spectral = GetFile("1 - 188 - The Lighthouse (Exterior) [Spectral - Combat].ogg") }
                        ]
                    };
                }
            }
        }

        private static async Task<WriteableBitmap?> CreateBitmapFromLogoAsync(StorageFile? logo)
        {
            if (logo == null)
                return null;

            using var stream = await logo.OpenAsync(FileAccessMode.Read);

            var decoder = await BitmapDecoder.CreateAsync(stream);
            var pixelData = await decoder.GetPixelDataAsync();

            byte[] pixels = pixelData.DetachPixelData();
            uint width = decoder.PixelWidth;
            uint height = decoder.PixelHeight;

            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte b = pixels[i + 0];
                byte g = pixels[i + 1];
                byte r = pixels[i + 2];

                if (r <= 0 && g == 0 && b == 0)
                {
                    pixels[i + 3] = 0;
                }
            }

            var bitmap = new WriteableBitmap((int)width, (int)height);
            using var wbStream = bitmap.PixelBuffer.AsStream();
            await wbStream.WriteAsync(pixels);

            return bitmap;
        }
    }
}
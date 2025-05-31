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
        private static readonly List<StorageFile> _files = [];

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

                var packages = new List<Package> { Package.Current };

                packages.AddRange([.. Package.Current.Dependencies.Where(d => d.IsOptional && d.Id.Name.StartsWith(Package.Current.Id.Name + ".Music."))]);

                foreach (var pack in packages)
                {
                    try
                    {
                        var folder = await pack.InstalledLocation.GetFolderAsync("Tracks");
                        var files = await folder.GetFilesAsync(CommonFileQuery.OrderByName);
                        _files.AddRange(files.Where(f => f.FileType.Equals(".ogg", StringComparison.OrdinalIgnoreCase)));
                    }
                    catch { }
                }
            }
            finally
            {
                _initialized = true;
            }
        }

        public static async Task<List<CategoryViewModel>> GenerateViewModelsAsync()
        {
            await EnsureInitializedAsync();

            var deduplicatedFiles = _files.DistinctBy(f => f.Name);

            var availableTracks = FindAvailableTracks(deduplicatedFiles);
            var categories = CreateViewModels(availableTracks);

            var logoFolder = await Package.Current.InstalledLocation.TryGetItemAsync("Logos") as StorageFolder;

            foreach (var category in categories)
            {
                category.Logo = logoFolder != null ? await CreateBitmapFromLogoAsync(await logoFolder.TryGetItemAsync($"{category.Id}.png") as StorageFile) : null;
            }

            return categories;
        }

        private static List<CategoryViewModel> CreateViewModels(IEnumerable<FoundTrack> foundTracks)
        {
            var categories = new List<CategoryViewModel>();

            CategoryViewModel GetOrAddCategory(string categoryId, string categoryName)
            {
                if (categories.FirstOrDefault(c => c.Id == categoryId) is CategoryViewModel category)
                    return category;

                var newCategory = new CategoryViewModel { Id = categoryId, Name = categoryName };
                categories.Add(newCategory);
                return newCategory;
            }

            foreach (var track in foundTracks)
            {
                var category = GetOrAddCategory(track.Track.CategoryId, track.Track.CategoryName);
                category.Tracks.Add(new()
                {
                    Name = track.Track.Name,
                    Material = track.Material,
                    Spectral = track.Spectral
                });
            }

            return categories;
        }

        private readonly record struct FoundTrack(KnownTrack Track, StorageFile Material, StorageFile Spectral);

        private static IEnumerable<FoundTrack> FindAvailableTracks(IEnumerable<StorageFile> storageFiles)
        {
            foreach (var track in _tracks)
            {
                if (storageFiles.FirstOrDefault(f => f.Name == track.Material) is not StorageFile materialFile ||
                    storageFiles.FirstOrDefault(f => f.Name == track.Spectral) is not StorageFile spectralFile)
                    continue;

                yield return new()
                {
                    Track = track,
                    Material = materialFile,
                    Spectral = spectralFile
                };
            }
        }

        private readonly record struct KnownTrack(string CategoryId, string CategoryName, string Name, string Material, string Spectral);

        private static readonly KnownTrack[] _tracks = [
            new("Underworld", "Underworld", "Normal", "1 - 002 - Underworld.ogg", "1 - 013 - Underworld [Spectral].ogg"),
            new("Underworld", "Underworld", "Suspense", "1 - 003 - Underworld [Suspense].ogg", "1 - 014 - Underworld [Spectral - Suspense].ogg"),
            new("Underworld", "Underworld", "Abyss", "1 - 004 - Underworld [Abyss].ogg", "1 - 015 - Underworld [Spectral - Abyss].ogg"),
            new("Underworld", "Underworld", "Puzzle", "1 - 005 - Underworld [Puzzle].ogg", "1 - 016 - Underworld [Spectral - Puzzle].ogg"),
            new("Underworld", "Underworld", "Danger", "1 - 006 - Underworld [Danger].ogg", "1 - 017 - Underworld [Spectral - Danger].ogg"),
            new("Underworld", "Underworld", "Combat", "1 - 007 - Underworld [Combat].ogg", "1 - 018 - Underworld [Spectral - Combat].ogg"),
            new("Underworld", "Underworld", "Normal (Exterior)", "1 - 008 - Underworld (Exterior).ogg", "1 - 019 - Underworld (Exterior) [Spectral].ogg"),
            new("Underworld", "Underworld", "Danger (Exterior)", "1 - 009 - Underworld (Exterior) [Danger].ogg", "1 - 020 - Underworld (Exterior) [Spectral - Danger].ogg"),
            new("Underworld", "Underworld", "Combat (Exterior)", "1 - 010 - Underworld (Exterior) [Combat].ogg", "1 - 021 - Underworld (Exterior) [Spectral - Combat].ogg"),
            new("Underworld", "Underworld", "Kain Encounter (Anticipation)", "1 - 061 - Kain Encounter (Anticipation).ogg", "1 - 062 - Kain Encounter (Anticipation) [Spectral].ogg"),
            new("Underworld", "Underworld", "Kain Encounter", "1 - 063 - Kain Encounter.ogg", "1 - 064 - Kain Encounter [Spectral].ogg"),

            new("RazielsStronghold", "Raziel’s Stronghold", "Normal", "1 - 025 - Raziel's Stronghold.ogg", "1 - 032 - Raziel's Stronghold [Spectral].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Suspense", "1 - 026 - Raziel's Stronghold [Suspense].ogg", "1 - 033 - Raziel's Stronghold [Spectral - Suspense].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Danger", "1 - 027 - Raziel's Stronghold [Danger].ogg", "1 - 034 - Raziel's Stronghold [Spectral - Danger].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Combat", "1 - 028 - Raziel's Stronghold [Combat].ogg", "1 - 035 - Raziel's Stronghold [Spectral - Combat].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Suspense (Exterior)", "1 - 029 - Raziel's Stronghold (Exterior) [Suspense].ogg", "1 - 036 - Raziel's Stronghold (Exterior) [Spectral - Suspense].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Danger (Exterior)", "1 - 030 - Raziel's Stronghold (Exterior) [Danger].ogg", "1 - 037 - Raziel's Stronghold (Exterior) [Spectral - Danger].ogg"),
            new("RazielsStronghold", "Raziel’s Stronghold", "Combat (Exterior)", "1 - 031 - Raziel's Stronghold (Exterior) [Combat].ogg", "1 - 038 - Raziel's Stronghold (Exterior) [Spectral - Combat].ogg"),

            new("Necropolis", "Necropolis", "Normal", "1 - 039 - Necropolis.ogg", "1 - 052 - Necropolis [Spectral].ogg"),
            new("Necropolis", "Necropolis", "Suspense", "1 - 040 - Necropolis [Suspense].ogg", "1 - 053 - Necropolis [Spectral - Suspense].ogg"),
            new("Necropolis", "Necropolis", "Puzzle", "1 - 041 - Necropolis [Puzzle].ogg", "1 - 054 - Necropolis [Spectral - Puzzle].ogg"),
            new("Necropolis", "Necropolis", "Danger", "1 - 042 - Necropolis [Danger].ogg", "1 - 055 - Necropolis [Spectral - Danger].ogg"),
            new("Necropolis", "Necropolis", "Combat", "1 - 044 - Necropolis [Combat].ogg", "1 - 056 - Necropolis [Spectral - Combat].ogg"),

            new("SilencedCathedral", "Silenced Cathedral", "Normal", "1 - 065 - Silenced Cathedral.ogg", "1 - 076 - Silenced Cathedral [Spectral].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Suspense", "1 - 066 - Silenced Cathedral [Suspense].ogg", "1 - 077 - Silenced Cathedral [Spectral - Suspense].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Puzzle", "1 - 067 - Silenced Cathedral [Puzzle].ogg", "1 - 078 - Silenced Cathedral [Spectral - Puzzle].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Danger", "1 - 068 - Silenced Cathedral [Danger].ogg", "1 - 079 - Silenced Cathedral [Spectral - Danger].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Combat", "1 - 069 - Silenced Cathedral [Combat].ogg", "1 - 080 - Silenced Cathedral [Spectral - Combat].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Suspense (Exterior)", "1 - 070 - Silenced Cathedral (Exterior) [Suspense].ogg", "1 - 081 - Silenced Cathedral (Exterior) [Spectral - Suspense].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Danger (Exterior)", "1 - 071 - Silenced Cathedral (Exterior) [Danger].ogg", "1 - 082 - Silenced Cathedral (Exterior) [Spectral - Danger].ogg"),
            new("SilencedCathedral", "Silenced Cathedral", "Combat (Exterior)", "1 - 072 - Silenced Cathedral (Exterior) [Combat].ogg", "1 - 083 - Silenced Cathedral (Exterior) [Spectral - Combat].ogg"),

            new("HumanCitadel", "Human Citadel", "Normal", "1 - 087 - Human Citadel.ogg", "1 - 099 - Human Citadel [Spectral].ogg"),
            new("HumanCitadel", "Human Citadel", "Suspense", "1 - 088 - Human Citadel [Suspense].ogg", "1 - 100 - Human Citadel [Spectral - Suspense].ogg"),
            new("HumanCitadel", "Human Citadel", "Puzzle", "1 - 089 - Human Citadel [Puzzle].ogg", "1 - 101 - Human Citadel [Spectral - Puzzle].ogg"),
            new("HumanCitadel", "Human Citadel", "Danger", "1 - 090 - Human Citadel [Danger].ogg", "1 - 102 - Human Citadel [Spectral - Danger].ogg"),
            new("HumanCitadel", "Human Citadel", "Combat", "1 - 091 - Human Citadel [Combat].ogg", "1 - 103 - Human Citadel [Spectral - Combat].ogg"),
            new("HumanCitadel", "Human Citadel", "Normal (Water Tower)", "1 - 092 - Human Citadel (Water Tower).ogg", "1 - 104 - Human Citadel (Water Tower) [Spectral].ogg"),
            new("HumanCitadel", "Human Citadel", "Danger (Water Tower)", "1 - 093 - Human Citadel (Water Tower) [Danger].ogg", "1 - 105 - Human Citadel (Water Tower) [Spectral - Danger].ogg"),
            new("HumanCitadel", "Human Citadel", "Combat (Water Tower)", "1 - 094 - Human Citadel (Water Tower) [Combat].ogg", "1 - 106 - Human Citadel (Water Tower) [Spectral - Combat].ogg"),
            new("HumanCitadel", "Human Citadel", "Unused (Undercity)", "1 - 095 - Human Citadel (Undercity) [Unused].ogg", "1 - 107 - Human Citadel (Undercity) [Spectral - Unused].ogg"),

            new("SarafanTomb", "Sarafan Tomb", "Normal", "1 - 111 - Sarafan Tomb.ogg", "1 - 123 - Sarafan Tomb [Spectral].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Suspense", "1 - 112 - Sarafan Tomb [Suspense].ogg", "1 - 124 - Sarafan Tomb [Spectral - Suspense].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Puzzle", "1 - 113 - Sarafan Tomb [Puzzle].ogg", "1 - 125 - Sarafan Tomb [Spectral - Puzzle].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Danger", "1 - 114 - Sarafan Tomb [Danger].ogg", "1 - 126 - Sarafan Tomb [Spectral - Danger].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Combat", "1 - 115 - Sarafan Tomb [Combat].ogg", "1 - 127 - Sarafan Tomb [Spectral - Combat].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Suspense (Exterior)", "1 - 116 - Sarafan Tomb (Exterior) [Suspense].ogg", "1 - 128 - Sarafan Tomb (Exterior) [Spectral - Suspense].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Danger (Exterior)", "1 - 117 - Sarafan Tomb (Exterior) [Danger].ogg", "1 - 129 - Sarafan Tomb (Exterior) [Spectral - Danger].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Combat (Exterior)", "1 - 118 - Sarafan Tomb (Exterior) [Combat].ogg", "1 - 130 - Sarafan Tomb (Exterior) [Spectral - Combat].ogg"),
            new("SarafanTomb", "Sarafan Tomb", "Tomb Guardian Encounter", "1 - 132 - Tomb Guardian Encounter.ogg", "1 - 133 - Tomb Guardian Encounter [Spectral].ogg"),

            new("DrownedAbbey", "Drowned Abbey", "Normal", "1 - 134 - Drowned Abbey.ogg", "1 - 144 - Drowned Abbey [Spectral].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Suspense", "1 - 135 - Drowned Abbey [Suspense].ogg", "1 - 145 - Drowned Abbey [Spectral - Suspense].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Puzzle", "1 - 136 - Drowned Abbey [Puzzle].ogg", "1 - 146 - Drowned Abbey [Spectral - Puzzle].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Danger", "1 - 137 - Drowned Abbey [Danger].ogg", "1 - 147 - Drowned Abbey [Spectral - Danger].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Combat", "1 - 138 - Drowned Abbey [Combat].ogg", "1 - 148 - Drowned Abbey [Spectral - Combat].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Suspense (Exterior)", "1 - 139 - Drowned Abbey (Exterior) [Suspense].ogg", "1 - 149 - Drowned Abbey (Exterior) [Spectral - Suspense].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Danger (Exterior)", "1 - 140 - Drowned Abbey (Exterior) [Danger].ogg", "1 - 150 - Drowned Abbey (Exterior) [Spectral - Danger].ogg"),
            new("DrownedAbbey", "Drowned Abbey", "Unused (Underwater)", "1 - 141 - Drowned Abbey (Underwater).ogg", "1 - 151 - Drowned Abbey (Underwater) [Spectral - Unused].ogg"),

            new("RuinedCity", "Ruined City", "Normal", "1 - 156 - Ruined City.ogg", "1 - 163 - Ruined City [Spectral].ogg"),
            new("RuinedCity", "Ruined City", "Suspense", "1 - 157 - Ruined City [Suspense].ogg", "1 - 164 - Ruined City [Spectral - Suspense].ogg"),
            new("RuinedCity", "Ruined City", "Puzzle", "1 - 158 - Ruined City [Puzzle].ogg", "1 - 165 - Ruined City [Spectral - Puzzle].ogg"),
            new("RuinedCity", "Ruined City", "Danger", "1 - 159 - Ruined City [Danger].ogg", "1 - 166 - Ruined City [Spectral - Danger].ogg"),
            new("RuinedCity", "Ruined City", "Combat", "1 - 160 - Ruined City [Combat].ogg", "1 - 167 - Ruined City [Spectral - Combat].ogg"),

            new("TheLighthouse", "The Lighthouse", "Normal", "1 - 169 - The Lighthouse.ogg", "1 - 181 - The Lighthouse [Spectral].ogg"),
            new("TheLighthouse", "The Lighthouse", "Suspense", "1 - 170 - The Lighthouse [Suspense].ogg", "1 - 182 - The Lighthouse [Spectral - Suspense].ogg"),
            new("TheLighthouse", "The Lighthouse", "Puzzle", "1 - 171 - The Lighthouse [Puzzle].ogg", "1 - 183 - The Lighthouse [Spectral - Puzzle].ogg"),
            new("TheLighthouse", "The Lighthouse", "Danger", "1 - 172 - The Lighthouse [Danger].ogg", "1 - 184 - The Lighthouse [Spectral - Danger].ogg"),
            new("TheLighthouse", "The Lighthouse", "Combat", "1 - 173 - The Lighthouse [Combat].ogg", "1 - 185 - The Lighthouse [Spectral - Combat].ogg"),
            new("TheLighthouse", "The Lighthouse", "Suspense (Exterior)", "1 - 175 - The Lighthouse (Exterior) [Suspense].ogg", "1 - 186 - The Lighthouse (Exterior) [Spectral - Suspense].ogg"),
            new("TheLighthouse", "The Lighthouse", "Danger (Exterior)", "1 - 176 - The Lighthouse (Exterior) [Danger].ogg", "1 - 187 - The Lighthouse (Exterior) [Spectral - Danger].ogg"),
            new("TheLighthouse", "The Lighthouse", "Combat (Exterior)", "1 - 177 - The Lighthouse (Exterior) [Combat].ogg", "1 - 188 - The Lighthouse (Exterior) [Spectral - Combat].ogg"),
        ];

        private static async Task<BitmapImage?> CreateBitmapFromLogoAsync(StorageFile? logo)
        {
            if (logo == null)
                return null;

            using var stream = await logo.OpenAsync(FileAccessMode.Read);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(stream);
            return bitmap;
        }
    }
}
using Microsoft.UI.Xaml;
using Sungaila.SoundReaver.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sungaila.SoundReaver
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            RootFrame.DataContext = CreateDataContext();
            RootFrame.Navigate(typeof(MainPage));
        }

        private static AppViewModel CreateDataContext() => new()
        {
            Categories = [
                new CategoryViewModel {
                    Name = "Misc",
                    IsExpanded = true,
                    Tracks = [
                        new TrackViewModel { Name = "Kain Encounter", Spectral = "1-50 - Kain Encounter - spectral.mp3", Material = "1-49 - Kain Encounter.mp3" },
                        new TrackViewModel { Name = "Anticipation", Spectral = "1-51 - Anticipation - spectral.mp3", Material = "1-48 - Anticipation.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Underworld",
                    IsExpanded = true,
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-07 - Underworld - spectral.mp3", Material = "1-01 - Underworld.mp3" },
                        new TrackViewModel { Name = "Suspense", Spectral = "1-08 - Underworld - spectral - sus.mp3", Material = "1-02 - Underworld - sus.mp3" },
                        new TrackViewModel { Name = "Suspense v2", Spectral = "1-09 - Underworld - spectral - sus v2.mp3", Material = "1-03 - Underworld - sus v2.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "1-10 - Underworld - spectral - puzzle.mp3", Material = "1-04 - Underworld - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-11 - Underworld - spectral - dng.mp3", Material = "1-05 - Underworld - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-12 - Underworld - spectral - cmb.mp3", Material = "1-06 - Underworld - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Underworld - Wilderness",
                    IsExpanded = true,
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-16 - Underworld - Wilderness - spectral.mp3", Material = "1-13 - Underworld - Wilderness.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-17 - Underworld - Wilderness - spectral - dng.mp3", Material = "1-14 - Underworld - Wilderness - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-18 - Underworld - Wilderness - spectral - cmb.mp3", Material = "1-15 - Underworld - Wilderness - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Raziel's Stronghold",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-25 - Raziel's Stronghold - spectral.mp3", Material = "1-19 - Raziel's Stronghold.mp3" },
                        new TrackViewModel { Name = "Suspense", Spectral = "1-26 - Raziel's Stronghold - spectral - sus.mp3", Material = "1-20 - Raziel's Stronghold - sus.mp3" },
                        new TrackViewModel { Name = "Suspense v2", Spectral = "1-27 - Raziel's Stronghold - spectral - sus v2.mp3", Material = "1-21 - Raziel's Stronghold - sus v2.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-28 - Raziel's Stronghold - spectral - dng.mp3", Material = "1-22 - Raziel's Stronghold - dng.mp3" },
                        new TrackViewModel { Name = "Danger v2", Spectral = "1-29 - Raziel's Stronghold - spectral - dng v2.mp3", Material = "1-23 - Raziel's Stronghold - dng v2.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-30 - Raziel's Stronghold - spectral - cmb.mp3", Material = "1-24 - Raziel's Stronghold - cmb.mp3" },
                        new TrackViewModel { Name = "Combat v2", Spectral = "1-31 - Raziel's Stronghold - spectral - cmb v2.mp3", Material = "1-24 - Raziel's Stronghold - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Necropolis",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-39 - Necropolis - spectral.mp3", Material = "1-33 - Necropolis.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "1-41 - Necropolis - spectral - puzzle.mp3", Material = "1-36 - Necropolis - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-42 - Necropolis - spectral - dng.mp3", Material = "1-37 - Necropolis - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-43 - Necropolis - spectral - cmb.mp3", Material = "1-38 - Necropolis - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Cathedral",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-60 - Cathedral - spectral.mp3", Material = "1-52 - Cathedral.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "1-63 - Cathedral - spectral - puzzle.mp3", Material = "1-55 - Cathedral - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-64 - Cathedral - spectral - dng.mp3", Material = "1-56 - Cathedral - dng.mp3" },
                        new TrackViewModel { Name = "Danger v2", Spectral = "1-65 - Cathedral - spectral - dng v2.mp3", Material = "1-57 - Cathedral - dng v2.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-66 - Cathedral - spectral - cmb.mp3", Material = "1-58 - Cathedral - cmb.mp3" },
                        new TrackViewModel { Name = "Combat v2", Spectral = "1-67 - Cathedral - spectral - cmb v2.mp3", Material = "1-59 - Cathedral - cmb v2.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "The City",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-77 - The City - spectral.mp3", Material = "1-72 - The City.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "1-79 - The City - spectral - puzzle.mp3", Material = "1-74 - The City - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-80 - The City - spectral - dng.mp3", Material = "1-75 - The City - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-81 - The City - spectral - cmb.mp3", Material = "1-76 - The City - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "The City - Water Tower",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-85 - The City - Water Tower - spectral.mp3", Material = "1-82 - The City - Water Tower.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-86 - The City - Water Tower - spectral - dng.mp3", Material = "1-83 - The City - Water Tower - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-87 - The City - Water Tower - spectral - cmb.mp3", Material = "1-84 - The City - Water Tower - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "The Undercity",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "1-91 - The Undercity - spectral.mp3", Material = "1-88 - The Undercity.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "1-92 - The Undercity - spectral - dng.mp3", Material = "1-89 - The Undercity - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "1-93 - The Undercity - spectral - cmb.mp3", Material = "1-90 - The Undercity - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Pillars-Tomb",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "2-09 - Pillars-Tomb - spectral.mp3", Material = "2-01 - Pillars-Tomb.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "2-12 - Pillars-Tomb - spectral - puzzle.mp3", Material = "2-04 - Pillars-Tomb - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "2-13 - Pillars-Tomb - spectral - dng.mp3", Material = "2-05 - Pillars-Tomb - dng.mp3" },
                        new TrackViewModel { Name = "Danger v2", Spectral = "2-14 - Pillars-Tomb - spectral - dng v2.mp3", Material = "2-06 - Pillars-Tomb - dng v2.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "2-15 - Pillars-Tomb - spectral - cmb.mp3", Material = "2-07 - Pillars-Tomb - cmb.mp3" },
                        new TrackViewModel { Name = "Combat v2", Spectral = "2-16 - Pillars-Tomb - spectral - cmb v2.mp3", Material = "2-08 - Pillars-Tomb - cmb v2.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Drowned Abbey",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "2-28 - Drowned Abbey - spectral.mp3", Material = "2-22 - Drowned Abbey.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "2-31 - Drowned Abbey - spectral - puzzle.mp3", Material = "2-25 - Drowned Abbey - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "2-32 - Drowned Abbey - spectral - dng.mp3", Material = "2-26 - Drowned Abbey - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "2-33 - Drowned Abbey - spectral - cmb.mp3", Material = "2-27 - Drowned Abbey - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "Ruined City",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "2-44 - Ruined City - spectral.mp3", Material = "2-39 - Ruined City.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "2-46 - Ruined City - spectral - puzzle.mp3", Material = "2-41 - Ruined City - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "2-47 - Ruined City - spectral - dng.mp3", Material = "2-42 - Ruined City - dng.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "2-48 - Ruined City - spectral - cmb.mp3", Material = "2-43 - Ruined City - cmb.mp3" },
                    ]
                },
                new CategoryViewModel {
                    Name = "The Lighthouse",
                    Tracks = [
                        new TrackViewModel { Name = "Normal", Spectral = "2-59 - The Lighthouse - spectral.mp3", Material = "2-51 - The Lighthouse.mp3" },
                        new TrackViewModel { Name = "Puzzle", Spectral = "2-62 - The Lighthouse - spectral - puzzle.mp3", Material = "2-54 - The Lighthouse - puzzle.mp3" },
                        new TrackViewModel { Name = "Danger", Spectral = "2-63 - The Lighthouse - spectral - dng.mp3", Material = "2-55 - The Lighthouse - dng.mp3" },
                        new TrackViewModel { Name = "Danger v2", Spectral = "2-64 - The Lighthouse - spectral - dng v2.mp3", Material = "2-56 - The Lighthouse - dng v2.mp3" },
                        new TrackViewModel { Name = "Combat", Spectral = "2-65 - The Lighthouse - spectral - cmb.mp3", Material = "2-57 - The  Lighthouse - cmb.mp3" },
                        new TrackViewModel { Name = "Combat v2", Spectral = "2-66 - The Lighthouse - spectral - cmb v2.mp3", Material = "2-58 - The Lighthouse - cmb v2.mp3" },
                    ]
                }
            ]
        };
    }
}

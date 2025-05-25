using CommunityToolkit.Mvvm.ComponentModel;
using Sungaila.SoundReaver.Manager;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class AppViewModel : ViewModel
    {
        public SettingsViewModel Settings { get; } = new();

        [ObservableProperty]
        public partial bool IsMaterial { get; set; }

        public bool IsRepeating
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                PlaybackManager.SetIsLoopingEnabled(value);
            }
        } = true;

        public double Volume
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                PlaybackManager.SetVolume(value);
            }
        } = 100.0d;

        public ObservableCollection<CategoryViewModel> Categories { get; set; } = [];

        public TrackViewModel? CurrentTrack
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                foreach (var track in Categories.SelectMany(c => c.Tracks))
                {
                    track.OnIsSelectedChanged();
                }
            }
        }
    }
}
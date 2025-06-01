using Sungaila.SoundReaver.Manager;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class AppViewModel : ViewModel
    {
        public SettingsViewModel Settings { get; } = new();

        public bool IsMaterial
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                ApplicationData.Current.RoamingSettings.Values[nameof(IsMaterial)] = value;
                PlaybackManager.IsMaterial = value;
            }
        } = false;

        public bool IsRepeating
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                ApplicationData.Current.RoamingSettings.Values[nameof(IsRepeating)] = value;
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

                ApplicationData.Current.RoamingSettings.Values[nameof(Volume)] = value;
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
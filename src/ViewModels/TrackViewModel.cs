using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class TrackViewModel : ViewModel
    {
        [ObservableProperty]
        public required partial string Name { get; set; }

        [ObservableProperty]
        public required partial StorageFile? Spectral { get; set; }

        [ObservableProperty]
        public required partial StorageFile? Material { get; set; }

        public bool IsSelected
        {
            get => App.MainWindow?.Data is AppViewModel viewModel && viewModel.CurrentTrack == this;
        }

        public void OnIsSelectedChanged() => OnPropertyChanged(nameof(IsSelected));
    }
}
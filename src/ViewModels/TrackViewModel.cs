using Windows.Storage;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class TrackViewModel : ViewModel
    {
        public required string Name { get; init; }

        public StorageFile? Thumbnail { get; set; }

        public required StorageFile? Spectral { get; init; }

        public required StorageFile? Material { get; init; }

        public bool IsSelected
        {
            get => App.MainWindow?.Data is AppViewModel viewModel && viewModel.CurrentTrack == this;
        }

        public void OnIsSelectedChanged() => OnPropertyChanged(nameof(IsSelected));
    }
}
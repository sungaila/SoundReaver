using CommunityToolkit.Mvvm.ComponentModel;

namespace Sungaila.SoundReaver.ViewModels
{
    internal partial class TrackViewModel : ViewModel
    {
        [ObservableProperty]
        public required partial string Name { get; set; }

        [ObservableProperty]
        public required partial string Spectral { get; set; }

        [ObservableProperty]
        public required partial string Material { get; set; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }
    }
}

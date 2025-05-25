using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class CategoryViewModel : ViewModel
    {
        [ObservableProperty]
        public required partial string Name { get; set; }

        [ObservableProperty]
        public partial bool IsExpanded { get; set; }

        public ObservableCollection<TrackViewModel> Tracks { get; set; } = [];
    }
}
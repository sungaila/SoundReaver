using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;

namespace Sungaila.SoundReaver.ViewModels
{
    public partial class CategoryViewModel : ViewModel
    {
        public required string Id { get; init; }

        public required string Name { get; init; }

        public ImageSource? Logo { get; set; }

        [ObservableProperty]
        public partial bool IsExpanded { get; set; }

        public ObservableCollection<TrackViewModel> Tracks { get; set; } = [];
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Sungaila.SoundReaver.ViewModels
{
    internal partial class AppViewModel : ViewModel
    {
        [ObservableProperty]
        public partial bool IsPlaying { get; set; }

        [ObservableProperty]
        public partial bool IsRepeating { get; set; } = true;

        [ObservableProperty]
        public partial bool IsMaterial { get; set; }

        public ObservableCollection<CategoryViewModel> Categories { get; set; } = [];
    }
}
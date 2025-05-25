using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.Globalization;
using WinRT;

namespace Sungaila.SoundReaver.ViewModels
{
    [GeneratedBindableCustomProperty]
    public partial class SettingsViewModel : ViewModel
    {
        private LanguageViewModel _selectedLanguage = new CultureInfo(ApplicationLanguages.PrimaryLanguageOverride);

        public LanguageViewModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    ApplicationLanguages.PrimaryLanguageOverride = value.IetfLanguageTag;
                }
            }
        }

        public ObservableCollection<LanguageViewModel> AvailableLanguages { get; } = [.. ApplicationLanguages.ManifestLanguages.Select(l => (LanguageViewModel)new CultureInfo(l))];

        [ObservableProperty]
        public partial bool IsShiftSoundEnabled { get; set; } = true;
    }
}
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Windows.Globalization;
using Windows.Storage;

namespace Sungaila.SoundReaver.ViewModels
{
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

        public bool IsShiftSoundEnabled
        {
            get => field;
            set
            {
                if (!SetProperty(ref field, value))
                    return;

                ApplicationData.Current.RoamingSettings.Values[nameof(IsShiftSoundEnabled)] = value;
            }
        } = true;
    }
}
using Microsoft.UI.Xaml;
using Microsoft.Windows.Globalization;
using System.Linq;
using Windows.Storage;

namespace Sungaila.SoundReaver
{
    public partial class App : Application
    {
        public static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        internal static MainWindow? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
            HandlePrimaryLanguageOverride();
        }

        private static void HandlePrimaryLanguageOverride()
        {
            if (!string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
                return;

            var supportedLanguage = ApplicationLanguages.Languages.FirstOrDefault(l => ApplicationLanguages.ManifestLanguages.Contains(l));

            ApplicationLanguages.PrimaryLanguageOverride = supportedLanguage ?? "en-US";
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
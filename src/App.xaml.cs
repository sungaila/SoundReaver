using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Sungaila.SoundReaver
{
    public partial class App : Application
    {
        public static readonly ResourceLoader ResourceLoader = new();

        public static readonly ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        internal static MainWindow? MainWindow { get; private set; }

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }
    }
}
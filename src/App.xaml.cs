using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
            UnhandledException += Application_UnhandledException;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            MainWindow = new MainWindow();
            MainWindow.Activate();
        }

        private static void Application_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (MainWindow == null)
                return;

            MainWindow.ShowInfoBar(e.Message, InfoBarSeverity.Error);
            e.Handled = true;
        }
    }
}
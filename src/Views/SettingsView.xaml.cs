using CommunityToolkit.WinUI.Controls;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.System;

namespace Sungaila.SoundReaver.Views
{
    public sealed partial class SettingsView : Page
    {
        private AppViewModel? Data => DataContext as AppViewModel;

        public SettingsView()
        {
            InitializeComponent();

            string nameAndVersion;
            string author;

            try
            {
                nameAndVersion = $"{Package.Current.DisplayName} {Package.Current.Id.Version.ToFormattedString()}";
                author = Package.Current.PublisherDisplayName;
            }
            catch
            {
                var assemblyName = typeof(App).Assembly.GetName();
                nameAndVersion = $"{assemblyName.Name} {assemblyName.Version}";
                author = typeof(App).Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? string.Empty;
            }

            AppNameTextBlock.Text = nameAndVersion;
            AuthorTextBlock.Text = author;
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
                comboBox.SelectionChanged += (_, _) => App.MainWindow?.ShowInfoBar(App.ResourceLoader.GetString("SettingsRestartRequired"), InfoBarSeverity.Warning);
        }

        internal static readonly Uri WindowsAppSettingsUri = new("ms-settings:appsfeatures-app");

        private async void WindowsAppSettingsCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(WindowsAppSettingsUri);
        }

        private async void SettingsCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not SettingsCard settingsCard)
                return;

            if (!Uri.TryCreate(settingsCard.ActionIconToolTip, UriKind.Absolute, out var uri))
                return;

            await Launcher.LaunchUriAsync(uri);
        }
    }
}
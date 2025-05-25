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

        private static readonly Uri _githubIssuesUri = new("https://github.com/sungaila/SoundReaver/issues");

        private async void SettingsCard_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(_githubIssuesUri);
        }
    }
}
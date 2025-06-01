using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.ViewModels;
using Windows.Storage;
using Windows.UI;

namespace Sungaila.SoundReaver
{
    public sealed partial class MainWindow : Window
    {
        internal NavigationView? NavigationView => Content.FindDescendant<NavigationView>();

        internal AppViewModel Data => (AppViewModel)((FrameworkElement)Content).DataContext;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            if (Content is FrameworkElement frameworkElement)
            {
                var viewModel = new AppViewModel();
                LoadAndApplySettings(viewModel);
                frameworkElement.DataContext = viewModel;
            }

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 650;
                presenter.PreferredMinimumHeight = 600;
            }
        }

        private void LoadAndApplySettings(AppViewModel viewModel)
        {
            if (ApplicationData.Current.RoamingSettings.Values[nameof(SettingsViewModel.IsShiftSoundEnabled)] is bool isShiftSoundEnabled)
                viewModel.Settings.IsShiftSoundEnabled = isShiftSoundEnabled;

            if (ApplicationData.Current.RoamingSettings.Values[nameof(AppViewModel.IsMaterial)] is bool isMaterial)
                viewModel.IsMaterial = isMaterial;

            if (ApplicationData.Current.RoamingSettings.Values[nameof(AppViewModel.IsRepeating)] is bool isRepeating)
                viewModel.IsRepeating = isRepeating;

            if (ApplicationData.Current.RoamingSettings.Values[nameof(AppViewModel.Volume)] is double volume)
                viewModel.Volume = volume;

            if (SystemBackdrop is MicaTintedBackdrop backdrop &&
                Application.Current.Resources.TryGetValue(viewModel.IsMaterial ? "AccentColorMaterial" : "AccentColorSpectral", out var resource) &&
                resource is Color color)
            {
                backdrop.TintColor = color;
            }
        }

        public void SetSubTitle(string? title)
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.High, () =>
            {
                Title = title != null
                ? $"Sound Reaver – {title}"
                : "Sound Reaver";
                AppTitleBar.Title = Title;
            });
        }
    }
}
using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.BadgeNotifications;
using Sungaila.SoundReaver.Manager;
using Sungaila.SoundReaver.ViewModels;
using Sungaila.SoundReaver.Views;
using System;
using System.Drawing;
using System.Linq;

namespace Sungaila.SoundReaver
{
    public sealed partial class MainWindow : Window
    {
        internal NavigationView NavigationView => NavView;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            if (Content is FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = new AppViewModel();
            }

            var icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!)!;
            AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(icon.Handle));

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 650;
                presenter.PreferredMinimumHeight = 600;
            }

            BadgeNotificationManager.Current.ClearBadge();
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (RootFrame.CanGoBack)
                RootFrame.GoBack();
        }

        private async void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not NavigationView navigationView)
                return;

            await DispatcherQueue.EnqueueAsync(async () =>
            {
                if (Content is not FrameworkElement frameworkElement ||frameworkElement.DataContext is not AppViewModel viewModel)
                    return;

                await foreach (var category in TrackManager.GenerateViewModelsAsync())
                {
                    viewModel.Categories.Add(category);
                }

                var categoryViewModel = viewModel.Categories.FirstOrDefault(c => c.Name == "Underworld")
                    ?? viewModel.Categories.FirstOrDefault();
                categoryViewModel?.IsExpanded = true;
                viewModel.CurrentTrack = categoryViewModel?.Tracks.FirstOrDefault();

                await PlaybackManager.EnsureInitializedAsync();

                navigationView.SelectedItem = navigationView.MenuItems.First();
            }, DispatcherQueuePriority.Low);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type? pageType = null;

            if (args.IsSettingsSelected)
            {
                pageType = typeof(SettingsView);
            }
            else if (args.SelectedItemContainer?.Tag is Type type)
            {
                pageType = type;
            }

            RootFrame.Navigate(
                pageType,
                null,
                args.RecommendedNavigationTransitionInfo);
        }
    }
}

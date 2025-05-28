using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.Manager;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Linq;

namespace Sungaila.SoundReaver.Views
{
    public sealed partial class MainView : Page
    {
        public MainView()
        {
            InitializeComponent();
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
                if (DataContext is not AppViewModel viewModel)
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

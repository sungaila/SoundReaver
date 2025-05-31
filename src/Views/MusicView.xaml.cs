using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Sungaila.SoundReaver.Manager;
using Sungaila.SoundReaver.ViewModels;
using System;
using Windows.System;
using Windows.UI;

namespace Sungaila.SoundReaver.Views
{
    public sealed partial class MusicView : Page
    {
        private readonly DispatcherTimer _positionTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };

        public MusicView()
        {
            InitializeComponent();
            _positionTimer.Tick += PositionTimer_Tick;
        }

        private void PositionTimer_Tick(object? sender, object e)
        {
            UpdateTrackPositionControls();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTrackPositionControls();

            if (PlaybackManager.IsPlaying)
                _positionTimer.Start();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _positionTimer.Stop();
            _positionTimer.Tick -= PositionTimer_Tick;
        }

        internal AppViewModel Data => (AppViewModel)DataContext;

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || sender is not ToggleSwitch toggleSwitch)
                return;

            if (Application.Current is not App app || App.MainWindow?.SystemBackdrop is not MicaTintedBackdrop backdrop)
                return;

            if (!app.Resources.TryGetValue(toggleSwitch.IsOn ? "AccentColorMaterial" : "AccentColorSpectral", out var resource) || resource is not Color color)
                return;

            backdrop.TintColor = color;

            await PlaybackManager.SwitchTrackVariant();

            if (!Data.Settings.IsShiftSoundEnabled)
                return;

            await PlaybackManager.PlayShiftSoundEffect(toggleSwitch.IsOn);
        }

        private async void TracksTreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
        {
            if (!IsLoaded || args.AddedItems.Count != 1 || args.AddedItems[0] is not TrackViewModel track || Data.CurrentTrack == track)
                return;

            Data.CurrentTrack = track;
            UpdateTrackPositionControls(false);

            if (!PlaybackManager.IsPlaying)
                return;

            _positionTimer.Stop();
            await PlaybackManager.PlayTrack(track, Data.IsMaterial);
            UpdateTrackPositionControls(false);
            _positionTimer.Start();
        }

        private async void PlayButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || _isUpdatingTrackPositionControls || PlaybackManager.IsPlaying || Data.CurrentTrack is not TrackViewModel track)
                return;

            _positionTimer.Stop();
            await PlaybackManager.PlayTrack(track, Data.IsMaterial);
            UpdateTrackPositionControls(false);
            _positionTimer.Start();
        }

        private async void PlayButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || _isUpdatingTrackPositionControls || !PlaybackManager.IsPlaying)
                return;

            await PlaybackManager.PauseTrack();
            _positionTimer.Stop();
            UpdateTrackPositionControls(false);
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            await PlaybackManager.StopTrack();
            _positionTimer.Stop();
            UpdateTrackPositionControls();
            PlayButton.IsChecked = false;
        }

        private async void RefreshHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not HyperlinkButton hyperlinkButton)
                return;

            await DispatcherQueue.EnqueueAsync(async () =>
            {
                try
                {
                    hyperlinkButton.IsEnabled = false;

                    if (Content is not FrameworkElement frameworkElement || frameworkElement.DataContext is not AppViewModel viewModel)
                        return;

                    await TrackManager.InitializeAsync();
                    viewModel.Categories.Clear();

                    await foreach (var category in TrackManager.GenerateViewModelsAsync())
                    {
                        viewModel.Categories.Add(category);
                    }
                }
                finally
                {
                    hyperlinkButton.IsEnabled = true;
                }
            }, Microsoft.UI.Dispatching.DispatcherQueuePriority.Low);
        }

        private async void SettingsHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(SettingsView.WindowsAppSettingsUri);
        }

        private bool _isUpdatingTrackPositionControls = false;

        private void UpdateTrackPositionControls(bool isTimerUpdate = true)
        {
            try
            {
                _isUpdatingTrackPositionControls = true;

                PlayButton.IsEnabled = Data.CurrentTrack != null;

                if (isTimerUpdate)
                    PlayButton.IsChecked = PlaybackManager.IsPlaying;

                PositionSlider.Maximum = PlaybackManager.Duration.TotalMilliseconds;
                PositionSlider.Value = PlaybackManager.Position.TotalMilliseconds;
            }
            finally
            {
                _isUpdatingTrackPositionControls = false;
            }
        }

        private void PositionSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_isUpdatingTrackPositionControls)
                return;

            PlaybackManager.Position = TimeSpan.FromMilliseconds(e.NewValue);
        }

        private void PositionSlider_PreviewKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Left && e.Key != VirtualKey.Right)
                return;

            PositionSlider.Value = e.Key == VirtualKey.Left
                ? Math.Max(PositionSlider.Minimum, PositionSlider.Value - PositionSlider.StepFrequency * 5)
                : Math.Min(PositionSlider.Maximum, PositionSlider.Value + PositionSlider.StepFrequency * 5);

            e.Handled = true;
        }

        private void TreeViewItem_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (sender is not TreeViewItem treeViewItem || treeViewItem.DataContext is not CategoryViewModel category)
                return;

            category.IsExpanded = !category.IsExpanded;
        }

        private void TreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not TreeViewItem item)
                return;

            if (item.FindDescendant<TextBlock>(b => b.Name == "CollapsedGlyph") is TextBlock collapsed &&
                item.FindDescendant<TextBlock>(b => b.Name == "ExpandedGlyph") is TextBlock expanded)
            {
                collapsed.Width = 18;
                collapsed.Height = 18;

                expanded.Width = collapsed.Width;
                expanded.Height = collapsed.Height;
            }

            if (item.FindDescendant<Rectangle>(b => b.Name == "SelectionIndicator") is Rectangle indicator)
            {
                indicator.Height = 48;
            }
        }

        private void VolumeButton_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (sender is not Button button || button.Flyout is null || args.Key != VirtualKey.V)
                return;

            VolumeSlider.Focus(FocusState.Keyboard);
            args.Handled = true;
        }
    }
}
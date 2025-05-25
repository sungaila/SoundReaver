using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.Windows.BadgeNotifications;
using NAudio.Wave;
using Sungaila.SoundReaver.Extensions;
using Sungaila.SoundReaver.Helpers;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using static System.Net.WebRequestMethods;

namespace Sungaila.SoundReaver
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _pollingTimer.Tick += PollingTimer_Tick;
        }

        internal AppViewModel Context => (AppViewModel)DataContext;

        private WasapiOut? _shiftWavePlayer;
        private readonly AudioFileReader _shiftAudioFile = new(AssetsHelper.GetFullPath(@"Sounds\raziel-0062.wav"));
        private readonly AudioFileReader _backAudioFile = new(AssetsHelper.GetFullPath(@"Sounds\raziel-0026.wav"));

        private async void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleSwitch toggleSwitch)
                return;

            if (Application.Current is not App app || app.MainWindow is not MainWindow mainWindow || mainWindow.SystemBackdrop is not MicaTintedBackdrop backdrop)
                return;

            app.Resources.TryGetValue(toggleSwitch.IsOn ? "AccentColorMaterial" : "AccentColorSpectral", out object? resource);

            if (resource is not Color color)
                return;

            backdrop.TintColor = color;

            _shiftWavePlayer?.Stop();
            _shiftWavePlayer?.Dispose();

            _shiftWavePlayer = new WasapiOut();

            _shiftAudioFile.Position = 0;
            _backAudioFile.Position = 0;

            if (toggleSwitch.IsOn)
            {
                _shiftWavePlayer.Init(_shiftAudioFile);
            }
            else
            {
                _shiftWavePlayer.Init(_backAudioFile);
            }

            _shiftWavePlayer.Play();

            await CrossFade(toggleSwitch.IsOn);
        }

        private WasapiOut? _spectralWavePlayer;
        private WasapiOut? _materialWavePlayer;
        private AudioFileReader? _spectralAudioFile;
        private AudioFileReader? _materialAudioFile;
        private bool _isPlaybackChanging = false;

        private async void TreeView_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
        {
            if (!Context.IsPlaying || args.AddedItems.Count != 1 || args.AddedItems[0] is not TrackViewModel track)
                return;

            _lastPosition = _spectralAudioFile?.Position ?? 0;
            await StartPlayback(track);
        }

        private async void PlayButton_Checked(object sender, RoutedEventArgs e)
        {
            if (TracksTreeView.SelectedItem is not TrackViewModel track)
                return;

            if (_isPlaybackChanging)
                return;

            try
            {
                _isPlaybackChanging = true;

                await StartPlayback(track);
            }
            finally
            {
                _isPlaybackChanging = false;
            }
        }

        private async void PlayButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isPlaybackChanging)
                return;

            try
            {
                _isPlaybackChanging = true;

                await PausePlayback();
            }
            finally
            {
                _isPlaybackChanging = false;
            }
        }

        private long _lastPosition = 0;
        private TrackViewModel? _lastTrack = null;
        private readonly DispatcherTimer _pollingTimer = new() { Interval = TimeSpan.FromMilliseconds(10) };
        private CancellationTokenSource _fadeCancellation = new();

        private async Task PausePlayback()
        {
            _pollingTimer.Stop();
            _spectralWavePlayer?.Pause();
            _materialWavePlayer?.Pause();

            _lastPosition = _spectralAudioFile?.Position ?? 0;

            Context.IsPlaying = false;

            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Paused);
        }

        private async Task StopPlayback()
        {
            try
            {
                await _fadeCancellation.CancelAsync();
            }
            catch (OperationCanceledException) { }

            _fadeCancellation = new();

            _pollingTimer.Stop();
            _spectralWavePlayer?.PlaybackStopped -= WavePlayer_PlaybackStopped;
            _spectralWavePlayer?.Stop();
            _materialWavePlayer?.Stop();
            _spectralWavePlayer?.Dispose();
            _materialWavePlayer?.Dispose();

            BadgeNotificationManager.Current.ClearBadge();
        }

        private async Task StartPlayback(TrackViewModel track)
        {
            var wasPlaying = Context.IsPlaying;
            var differentTrack = _lastTrack != track;

            if (_spectralWavePlayer?.PlaybackState != PlaybackState.Paused)
            {
                await StopPlayback();

                _lastTrack = track;

                _spectralAudioFile = TrackHelper.Files[track.Spectral];
                _materialAudioFile = TrackHelper.Files[track.Material];

                if (_lastPosition < _spectralAudioFile.Length && _lastPosition < _materialAudioFile.Length)
                {
                    _spectralAudioFile.Position = _lastPosition;
                    _materialAudioFile.Position = _lastPosition;
                }

                _spectralWavePlayer = new WasapiOut();
                _spectralWavePlayer.PlaybackStopped += WavePlayer_PlaybackStopped;
                _materialWavePlayer = new WasapiOut();

                _spectralWavePlayer.Init(_spectralAudioFile);
                _materialWavePlayer.Init(_materialAudioFile);
            }

            Context.IsPlaying = true;

            TimeSlider.Maximum = _spectralAudioFile.Length;
            TotalTimeTextBlock.Text = _spectralAudioFile.TotalTime.ToString(@"hh\:mm\:ss");

            CurrentTimeTextBlock.Text = _spectralAudioFile.CurrentTime.ToString(@"hh\:mm\:ss");
            TimeSlider.Value = _spectralAudioFile.Position;

            _pollingTimer.Start();

            _spectralWavePlayer.Play();
            _materialWavePlayer.Play();

            if (wasPlaying && !differentTrack)
            {
                await CrossFade(Context.IsMaterial);
            }
            else
            {
                if (Context.IsMaterial)
                {
                    _spectralAudioFile.Volume = 0.0f;
                    _materialAudioFile.Volume = (float)(VolumeSlider.Value / 100.0d);
                }
                else
                {
                    _spectralAudioFile.Volume = (float)(VolumeSlider.Value / 100.0d);
                    _materialAudioFile.Volume = 0.0f;
                }
            }

            BadgeNotificationManager.Current.SetBadgeAsGlyph(BadgeNotificationGlyph.Playing);
        }

        private void PollingTimer_Tick(object? sender, object e)
        {
            if (!Context.IsPlaying || _spectralAudioFile == null)
                return;

            CurrentTimeTextBlock.Text = _spectralAudioFile.CurrentTime.ToString(@"hh\:mm\:ss");

            try
            {
                _isPlaybackChanging = true;
                TimeSlider.Value = _spectralAudioFile.Position;
            }
            finally
            {
                _isPlaybackChanging = false;
            }
        }

        private async Task CrossFade(bool isMaterial)
        {
            if (!Context.IsPlaying || _spectralAudioFile == null || _materialAudioFile == null)
                return;

            try
            {
                if (isMaterial)
                {
                    await Task.WhenAll(
                        _spectralAudioFile.FadeOutAsync(_shiftAudioFile.TotalTime, cancellationToken: _fadeCancellation.Token),
                        _materialAudioFile.FadeInAsync(_shiftAudioFile.TotalTime, (float)(VolumeSlider.Value / 100.0d), cancellationToken: _fadeCancellation.Token)
                        );
                }
                else
                {
                    await Task.WhenAll(
                        _materialAudioFile.FadeOutAsync(_backAudioFile.TotalTime, cancellationToken: _fadeCancellation.Token),
                        _spectralAudioFile.FadeInAsync(_backAudioFile.TotalTime, (float)(VolumeSlider.Value / 100.0d), cancellationToken: _fadeCancellation.Token)
                        );
                }
            }
            catch (OperationCanceledException) { }
        }

        private void WavePlayer_PlaybackStopped(object? sender, StoppedEventArgs e)
        {
            try
            {
                if (_spectralAudioFile != null && _spectralAudioFile.Position >= _spectralAudioFile.Length - 50000)
                {
                    _lastPosition = 0;
                    _spectralAudioFile.Position = 0;
                    _materialAudioFile.Position = 0;

                    CurrentTimeTextBlock.Text = _spectralAudioFile.TotalTime.ToString(@"hh\:mm\:ss");

                    _isPlaybackChanging = true;
                    TimeSlider.Value = TimeSlider.Maximum;

                    if (Context.IsRepeating)
                    {
                        _spectralWavePlayer.Play();
                        _materialWavePlayer.Play();
                        return;
                    }
                }

                Context.IsPlaying = false;
            }
            finally
            {
                _isPlaybackChanging = false;
            }
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaybackChanging)
                return;

            try
            {
                _isPlaybackChanging = true;

                await StopPlayback();

                _lastPosition = 0;

                CurrentTimeTextBlock.Text = "00:00:00";
                TimeSlider.Value = 0;

                Context.IsPlaying = false;

                _isPlaybackChanging = true;
            }
            finally
            {
                _isPlaybackChanging = false;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (Context == null)
                return;

            if (Context.IsMaterial)
            {
                _spectralAudioFile?.Volume = 0.0f;
                _materialAudioFile?.Volume = (float)(e.NewValue / 100.0d);
            }
            else
            {
                _spectralAudioFile?.Volume = (float)(e.NewValue / 100.0d);
                _materialAudioFile?.Volume = 0.0f;
            }
        }

        private void TimeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_isPlaybackChanging)
                return;

            _spectralAudioFile?.Position = (long)e.NewValue;
            _materialAudioFile?.Position = (long)e.NewValue;
        }

        private void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Context.Categories.FirstOrDefault(c => c.Name == "Misc")?.Tracks.FirstOrDefault(t => t.Name == "Anticipation") is not TrackViewModel track)
                return;

            TracksTreeView.SelectedItem = track;
        }
    }
}

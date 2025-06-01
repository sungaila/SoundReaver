using Sungaila.SoundReaver.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Sungaila.SoundReaver.Manager
{
    public static class PlaybackManager
    {
        private static bool _initialized = false;

        private static readonly MediaPlayer _toggleMediaPlayer = new()
        {
            CommandManager = { IsEnabled = false },
            AudioCategory = MediaPlayerAudioCategory.SoundEffects,
            SystemMediaTransportControls = { IsEnabled = false }
        };
        private static StorageFile? _shiftToMaterialFile = null;
        private static StorageFile? _shiftToSpectralFile = null;

        private static readonly MediaPlayer _spectralMediaPlayer = new()
        {
            CommandManager = { IsEnabled = false },
            AudioCategory = MediaPlayerAudioCategory.GameMedia,
            IsLoopingEnabled = true,
            IsMuted = true,
            SystemMediaTransportControls =
            {
                IsEnabled = false,
                AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track,
                IsChannelDownEnabled = false,
                IsChannelUpEnabled = false,
                IsFastForwardEnabled = false,
                IsNextEnabled = false,
                IsPauseEnabled = true,
                IsPlayEnabled = true,
                IsPreviousEnabled = false,
                IsRecordEnabled = false,
                IsRewindEnabled = false,
                IsStopEnabled = true,
                ShuffleEnabled = false
            }
        };

        private static readonly MediaPlayer _materialMediaPlayer = new()
        {
            CommandManager = { IsEnabled = false },
            AudioCategory = MediaPlayerAudioCategory.GameMedia,
            IsLoopingEnabled = true,
            IsMuted = true,
            SystemMediaTransportControls =
            {
                IsEnabled = false,
                AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track,
                IsChannelDownEnabled = false,
                IsChannelUpEnabled = false,
                IsFastForwardEnabled = false,
                IsNextEnabled = false,
                IsPauseEnabled = true,
                IsPlayEnabled = true,
                IsPreviousEnabled = false,
                IsRecordEnabled = false,
                IsRewindEnabled = false,
                IsStopEnabled = true,
                ShuffleEnabled = false
            }
        };

        public static async Task EnsureInitializedAsync()
        {
            if (_initialized)
                return;

            await InitializeAsync();
        }

        public static async Task InitializeAsync()
        {
            try
            {
                _shiftToMaterialFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Sounds/raziel-0062.wav"));
                _shiftToSpectralFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Sounds/raziel-0026.wav"));

                _spectralMediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
                _materialMediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;

                _spectralMediaPlayer.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
                _materialMediaPlayer.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            }
            finally
            {
                _initialized = true;
            }
        }

        public static event EventHandler<MediaPlaybackState>? PlaybackStateChanged;

        private static void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            PlaybackStateChanged?.Invoke(sender, sender.PlaybackState);
        }

        private static async void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button == SystemMediaTransportControlsButton.Pause)
            {
                await PauseTrack();
            }
            else if (args.Button == SystemMediaTransportControlsButton.Stop)
            {
                await StopTrack();
            }
            else if (args.Button == SystemMediaTransportControlsButton.Play && _lastTrack != null)
            {
                await PlayTrack(_lastTrack, _lastIsMaterial, false);
            }
        }

        public static async Task PlayShiftSoundEffect(bool toMaterial)
        {
            await EnsureInitializedAsync();

            _toggleMediaPlayer.Pause();
            _toggleMediaPlayer.Position = TimeSpan.Zero;

            _toggleMediaPlayer.Source = MediaSource.CreateFromStorageFile(
                toMaterial
                    ? _shiftToMaterialFile
                    : _shiftToSpectralFile
            );

            _toggleMediaPlayer.Play();
        }

        public static async Task StopTrack()
        {
            await EnsureInitializedAsync();

            _spectralMediaPlayer.Pause();
            _materialMediaPlayer.Pause();

            _spectralMediaPlayer.Position = TimeSpan.Zero;
            _materialMediaPlayer.Position = TimeSpan.Zero;

            _lastPosition = _spectralMediaPlayer.Position;
            App.MainWindow?.SetSubTitle(null);
        }

        public static async Task PauseTrack()
        {
            await EnsureInitializedAsync();

            _spectralMediaPlayer.Pause();
            _materialMediaPlayer.Pause();

            _spectralMediaPlayer.Position = _materialMediaPlayer.Position;

            _lastPosition = _spectralMediaPlayer.Position;
        }

        private static TimeSpan _lastPosition = TimeSpan.Zero;
        private static TrackViewModel? _lastTrack = null;
        private static bool _lastIsMaterial = false;

        public static async Task SwitchTrackVariant()
        {
            await EnsureInitializedAsync();

            (_spectralMediaPlayer.IsMuted, _materialMediaPlayer.IsMuted) = (_materialMediaPlayer.IsMuted, _spectralMediaPlayer.IsMuted);
            (_spectralMediaPlayer.CommandManager.IsEnabled, _materialMediaPlayer.CommandManager.IsEnabled) = (_materialMediaPlayer.CommandManager.IsEnabled, _spectralMediaPlayer.CommandManager.IsEnabled);

            if (!IsPlaying)
                return;

            if (_spectralMediaPlayer.SystemMediaTransportControls.IsEnabled)
            {
                App.MainWindow?.SetSubTitle(_spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.MusicProperties.Title);
            }
            else if (_materialMediaPlayer.SystemMediaTransportControls.IsEnabled)
            {
                App.MainWindow?.SetSubTitle(_materialMediaPlayer.SystemMediaTransportControls.DisplayUpdater.MusicProperties.Title);
            }
        }

        public static async Task PlayTrack(TrackViewModel trackViewModel, bool isMaterial, bool isTrackSwitching)
        {
            await EnsureInitializedAsync();

            if (trackViewModel == null || trackViewModel.Spectral == null || trackViewModel.Material == null)
                return;

            if (IsPlaying)
                _lastPosition = _spectralMediaPlayer.Position;

            bool wasPlaying = IsPlaying;

            _spectralMediaPlayer.Pause();
            _materialMediaPlayer.Pause();

            if (_lastTrack != trackViewModel)
            {
                _spectralMediaPlayer.Source = MediaSource.CreateFromStorageFile(trackViewModel.Spectral);
                _materialMediaPlayer.Source = MediaSource.CreateFromStorageFile(trackViewModel.Material);

                var spectralOpenTcs = new TaskCompletionSource();
                var materialOpenTcs = new TaskCompletionSource();

                void HandleMediaOpened(MediaPlayer sender, object args)
                {
                    sender.MediaOpened -= HandleMediaOpened;

                    if (sender == _spectralMediaPlayer)
                        spectralOpenTcs.TrySetResult();
                    else if (sender == _materialMediaPlayer)
                        materialOpenTcs.TrySetResult();
                }

                _spectralMediaPlayer.MediaOpened += HandleMediaOpened;
                _materialMediaPlayer.MediaOpened += HandleMediaOpened;

                await Task.WhenAll(spectralOpenTcs.Task, materialOpenTcs.Task);

                if (_lastPosition < _spectralMediaPlayer.NaturalDuration && _lastPosition < _materialMediaPlayer.NaturalDuration)
                {
                    _spectralMediaPlayer.Position = _lastPosition;
                    _materialMediaPlayer.Position = _lastPosition;
                }
                else
                {
                    _spectralMediaPlayer.Position = TimeSpan.Zero;
                    _materialMediaPlayer.Position = TimeSpan.Zero;
                }

                await _spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, trackViewModel.Spectral);
                await _materialMediaPlayer.SystemMediaTransportControls.DisplayUpdater.CopyFromFileAsync(MediaPlaybackType.Music, trackViewModel.Material);

                _spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail = trackViewModel.Thumbnail != null ? RandomAccessStreamReference.CreateFromFile(trackViewModel.Thumbnail) : null;
                _materialMediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail = _spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.Thumbnail;

                _spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.Update();
                _materialMediaPlayer.SystemMediaTransportControls.DisplayUpdater.Update();
            }

            _spectralMediaPlayer.IsMuted = isMaterial;
            _materialMediaPlayer.IsMuted = !isMaterial;

            _spectralMediaPlayer.CommandManager.IsEnabled = !isMaterial;
            _materialMediaPlayer.CommandManager.IsEnabled = isMaterial;

            App.MainWindow?.SetSubTitle(!isMaterial
                ? _spectralMediaPlayer.SystemMediaTransportControls.DisplayUpdater.MusicProperties.Title
                : _materialMediaPlayer.SystemMediaTransportControls.DisplayUpdater.MusicProperties.Title
            );

            _lastTrack = trackViewModel;
            _lastIsMaterial = isMaterial;

            if (!isTrackSwitching || wasPlaying)
            {
                _spectralMediaPlayer.Play();
                _materialMediaPlayer.Play();
            }
        }

        public static void SetIsLoopingEnabled(bool value)
        {
            _spectralMediaPlayer.IsLoopingEnabled = value;
            _materialMediaPlayer.IsLoopingEnabled = value;
        }

        public static void SetVolume(double value)
        {
            _spectralMediaPlayer.Volume = value / 100.0d;
            _materialMediaPlayer.Volume = value / 100.0d;
        }

        public static bool IsPlaying => _spectralMediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;

        public static TimeSpan Position
        {
            get => _spectralMediaPlayer.Position;
            set
            {
                _spectralMediaPlayer.Position = value;
                _materialMediaPlayer.Position = value;
            }
        }

        public static TimeSpan Duration => _spectralMediaPlayer.NaturalDuration;
    }
}
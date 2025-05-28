using Sungaila.SoundReaver.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace Sungaila.SoundReaver.Manager
{
    public static class PlaybackManager
    {
        private static bool _initialized = false;

        private static readonly MediaPlayer _toggleMediaPlayer = new() { CommandManager = { IsEnabled = false }, AudioCategory = MediaPlayerAudioCategory.SoundEffects };
        private static StorageFile? _shiftToMaterialFile = null;
        private static StorageFile? _shiftToSpectralFile = null;

        private static readonly MediaPlayer _spectralMediaPlayer = new() { CommandManager = { IsEnabled = false }, AudioCategory = MediaPlayerAudioCategory.GameMedia, IsLoopingEnabled = true };
        private static readonly MediaPlayer _materialMediaPlayer = new() { CommandManager = { IsEnabled = false }, AudioCategory = MediaPlayerAudioCategory.GameMedia, IsLoopingEnabled = true };

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
            }
            finally
            {
                _initialized = true;
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
        }

        public static async Task PauseTrack()
        {
            await EnsureInitializedAsync();

            _spectralMediaPlayer.Pause();
            _materialMediaPlayer.Pause();

            _lastPosition = _spectralMediaPlayer.Position;
        }

        private static TimeSpan _lastPosition = TimeSpan.Zero;

        public static async Task SwitchTrackVariant()
        {
            await EnsureInitializedAsync();

            (_spectralMediaPlayer.IsMuted, _materialMediaPlayer.IsMuted) = (_materialMediaPlayer.IsMuted, _spectralMediaPlayer.IsMuted);
        }

        public static async Task PlayTrack(TrackViewModel trackViewModel, bool isMaterial)
        {
            await EnsureInitializedAsync();

            if (trackViewModel == null || trackViewModel.Spectral == null || trackViewModel.Material == null)
                return;

            if (IsPlaying)
                _lastPosition = _spectralMediaPlayer.Position;

            _spectralMediaPlayer.Pause();
            _materialMediaPlayer.Pause();

            var spectralStream = await trackViewModel.Spectral.OpenReadAsync();
            var materialStream = await trackViewModel.Material.OpenReadAsync();
            _spectralMediaPlayer.Source = MediaSource.CreateFromStream(spectralStream, trackViewModel.Spectral.ContentType);
            _materialMediaPlayer.Source = MediaSource.CreateFromStream(materialStream, trackViewModel.Material.ContentType);

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

            _spectralMediaPlayer.CurrentStateChanged += DisposeStream;
            _materialMediaPlayer.CurrentStateChanged += DisposeStream;

            void DisposeStream(MediaPlayer sender, object args)
            {
                if (_spectralMediaPlayer.CurrentState == MediaPlayerState.Closed || _spectralMediaPlayer.CurrentState == MediaPlayerState.Paused || _spectralMediaPlayer.CurrentState == MediaPlayerState.Stopped)
                {
                    spectralStream?.Dispose();
                    _spectralMediaPlayer.CurrentStateChanged -= DisposeStream;
                }

                if (_materialMediaPlayer.CurrentState == MediaPlayerState.Closed || _materialMediaPlayer.CurrentState == MediaPlayerState.Paused || _materialMediaPlayer.CurrentState == MediaPlayerState.Stopped)
                {
                    materialStream?.Dispose();
                    _materialMediaPlayer.CurrentStateChanged -= DisposeStream;
                }
            }

            _spectralMediaPlayer.IsMuted = isMaterial;
            _materialMediaPlayer.IsMuted = !isMaterial;

            _spectralMediaPlayer.Play();
            _materialMediaPlayer.Play();
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

        public static bool IsPlaying => _spectralMediaPlayer.CurrentState == MediaPlayerState.Playing || _materialMediaPlayer.CurrentState == MediaPlayerState.Playing;

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
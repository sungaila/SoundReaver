using CommunityToolkit.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace Sungaila.SoundReaver
{
    public sealed partial class MainWindow : Window
    {
        internal NavigationView? NavigationView => Content.FindDescendant<NavigationView>();

        internal CanvasControl? CanvasControl => PatternCanvasVertical;

        internal AppViewModel Data => (AppViewModel)((FrameworkElement)Content).DataContext;

        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;

            if (Content is FrameworkElement frameworkElement)
            {
                frameworkElement.FlowDirection = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight;

                var viewModel = new AppViewModel();
                LoadAndApplySettings(viewModel);
                frameworkElement.DataContext = viewModel;
            }

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 650;
                presenter.PreferredMinimumHeight = 600;
            }

            AppWindow.SetIcon(@"Assets\App.ico");

            UpdatePatternCanvasVisibility();
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

        public void ShowInfoBar(string message, InfoBarSeverity severity)
        {
            if (MainView.InfoBar == null)
                return;

            MainView.InfoBar.Severity = severity;
            MainView.InfoBar.Message = message;
            MainView.InfoBar.IsOpen = true;
        }

        CanvasBitmap? _canvasBitmap;

        private void PatternCanvas_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            static async Task CreateResources(MainWindow @this, CanvasControl sender)
            {
                var uri = new Uri("ms-appx:///Assets/Background.png");

                if (await StorageFile.GetFileFromApplicationUriAsync(uri) is not StorageFile file)
                    return;

                using var pattern = await file.OpenStreamForReadAsync();
                using var ms = new MemoryStream((int)pattern.Length);
                pattern.CopyTo(ms);

                using var stream = new InMemoryRandomAccessStream();
                await stream.WriteAsync(ms.ToArray().AsBuffer());
                stream.Seek(0);

                @this._canvasBitmap = await CanvasBitmap.LoadAsync(sender, stream);
            }

            args.TrackAsyncAction(CreateResources(this, sender).AsAsyncAction());
        }

        private void PatternCanvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (_canvasBitmap == null || !GetPatternCanvasVisible())
                return;

            using var list = new CanvasCommandList(sender);
            using var session = list.CreateDrawingSession();
            session.DrawImage(_canvasBitmap, 0, 0, _canvasBitmap.Bounds, 1f, CanvasImageInterpolation.NearestNeighbor);

            const float scale = 1f;
            var halfRect = new Windows.Foundation.Rect(
                0, 0,
                _canvasBitmap.Bounds.Width * scale,
                _canvasBitmap.Bounds.Height * scale);

            using var preScale = new Transform2DEffect
            {
                Source = list,
                CacheOutput = true,
                TransformMatrix = Matrix3x2.CreateScale(scale),
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor
            };

            using var tile = new TileEffect
            {
                Source = preScale,
                CacheOutput = true,
                SourceRectangle = halfRect
            };

            using var rotate = new Transform2DEffect
            {
                Source = tile,
                CacheOutput = true,
                TransformMatrix = Matrix3x2.CreateTranslation(Vector2.Zero) * Matrix3x2.CreateRotation(-0.0872665f, Vector2.Zero),
                InterpolationMode = CanvasImageInterpolation.NearestNeighbor
            };

            if (sender == PatternCanvasVertical)
            {
                using var move = new Transform2DEffect
                {
                    Source = rotate,
                    CacheOutput = true,
                    TransformMatrix = Matrix3x2.CreateTranslation(Vector2.Zero) * Matrix3x2.CreateTranslation(0, (float)-PatternCanvasHorizontal.ActualHeight),
                    InterpolationMode = CanvasImageInterpolation.NearestNeighbor
                };

                args.DrawingSession.DrawImage(move);
            }
            else
            {
                args.DrawingSession.DrawImage(rotate);
            }
        }

        public static bool GetPatternCanvasVisible() => App.LocalSettings.Values["RenderBackgroundPattern"] is bool render && render;

        public static void SetPatternCanvasVisible(bool value)
        {
            App.LocalSettings.Values["RenderBackgroundPattern"] = value;
            App.MainWindow?.UpdatePatternCanvasVisibility();
        }

        private void UpdatePatternCanvasVisibility()
        {
            PatternCanvasHorizontal.Visibility = GetPatternCanvasVisible()
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}
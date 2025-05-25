using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Sungaila.SoundReaver
{
    internal sealed partial class MicaTintedBackdrop : SystemBackdrop
    {
        private readonly MicaController _micaController = new();
        private int _alpha = 0xFF;

        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);

            var defaultConfig = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
            _micaController.SetSystemBackdropConfiguration(defaultConfig);
            _micaController.AddSystemBackdropTarget(connectedTarget);
        }

        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);

            _micaController.RemoveSystemBackdropTarget(disconnectedTarget);
            _micaController?.Dispose();
        }

        public Color TintColor
        {
            get => _micaController.TintColor;
            set
            {
                _micaController.TintColor = value;
                _micaController.FallbackColor = Color.FromArgb((byte)_alpha, TintColor.R, TintColor.G, TintColor.B);
            }
        }

        public MicaKind Kind
        {
            get => _micaController.Kind;
            set => _micaController.Kind = value;
        }

        public int Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;

                _micaController.TintOpacity = _alpha / 255.0f;
                _micaController.FallbackColor = Color.FromArgb((byte)_alpha, TintColor.R, TintColor.G, TintColor.B);
            }
        }
    }
}
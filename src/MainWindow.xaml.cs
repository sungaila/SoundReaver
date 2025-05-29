using CommunityToolkit.WinUI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sungaila.SoundReaver.ViewModels;

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
                frameworkElement.DataContext = new AppViewModel();
            }

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 650;
                presenter.PreferredMinimumHeight = 600;
            }
        }
    }
}
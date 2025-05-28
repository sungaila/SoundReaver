using CommunityToolkit.WinUI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.BadgeNotifications;
using Sungaila.SoundReaver.ViewModels;
using System;
using System.Drawing;

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

            var icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!)!;
            AppWindow.SetIcon(Win32Interop.GetIconIdFromIcon(icon.Handle));

            if (AppWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.PreferredMinimumWidth = 650;
                presenter.PreferredMinimumHeight = 600;
            }

            BadgeNotificationManager.Current.ClearBadge();
        }
    }
}
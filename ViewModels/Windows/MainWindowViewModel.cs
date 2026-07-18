using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace Uperclaw.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public static MainWindowViewModel Instance { get; private set; }
        public MainWindowViewModel()
        {
            Instance = this;
            _menuItems[0].SetResourceReference(NavigationViewItem.ContentProperty, "Menu");
            _menuItems[1].SetResourceReference(NavigationViewItem.ContentProperty, "Control");
            _menuItems[2].SetResourceReference(NavigationViewItem.ContentProperty, "Learn");
            _menuItems[3].SetResourceReference(NavigationViewItem.ContentProperty, "Console");
            _footerMenuItems[0].SetResourceReference(NavigationViewItem.ContentProperty, "Settings");
        }
        [ObservableProperty]
        private string _applicationTitle = "UpanClaw";

        [ObservableProperty]
        private ObservableCollection<NavigationViewItem> _menuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "主页",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage),
            },
            new NavigationViewItem()
            {
                Content = "管理",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.DataPage)
            },new NavigationViewItem()
            {
                Content = "教程",
                Icon = new SymbolIcon{ Symbol = SymbolRegular.QuestionCircle24},
                TargetPageType = typeof(Views.Pages.Learn)
            },
            new NavigationViewItem()
            {
                Content = "终端",
                Icon = new SymbolIcon{Symbol = SymbolRegular.TextAlignCenter24},
                TargetPageType= typeof(Views.Pages.Console)
            }
        };

        [ObservableProperty]
        private ObservableCollection<NavigationViewItem> _footerMenuItems = new()
        {
            new NavigationViewItem()
            {
                Content = "设置",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };
    }
}

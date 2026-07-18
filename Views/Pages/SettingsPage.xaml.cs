using System.Globalization;
using System.Windows.Controls;
using Uperclaw.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Uperclaw.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            RegionBox.ItemsSource = new List<string>() { $"{RegionInfo.CurrentRegion.DisplayName} | {RegionInfo.CurrentRegion.EnglishName}" };
            RegionBox.SelectedIndex = 0;
        }

        private void RegionBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
        bool isLoading = false;
        private void LanguageBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (isLoading)
            {
                isLoading = false;
                return;
            }
            string requestedCulture;
            var target = (ComboBox)sender;
            if(target.SelectedIndex == 0)
            {//zh-CN
                requestedCulture = "/Language/zh-CN.xaml";
            }
            else
            {//en-US
                requestedCulture = "/Language/en-US.xaml";
            }
            var resourceDictionary = Application.Current.Resources.MergedDictionaries.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));

            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}

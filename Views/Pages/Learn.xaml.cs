using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Uperclaw.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace Uperclaw.Views.Pages
{
    /// <summary>
    /// Learn.xaml 的交互逻辑
    /// </summary>
    public partial class Learn : INavigableView<STDViewModel>
    {
        public STDViewModel ViewModel { get; set; }
        public Learn(STDViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}

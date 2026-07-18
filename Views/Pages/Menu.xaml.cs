using System.Diagnostics.SymbolStore;
using System.IO;
using System.Management;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Uperclaw.ViewModels.Pages;
using Uperclaw.ViewModels.Windows;
using Uperclaw.Views.Windows;
using Wpf.Ui.Abstractions.Controls;

namespace Uperclaw.Views.Pages
{
    public partial class DashboardPage : INavigableView<STDViewModel>
    {
        public static bool AutoRunOnce = false;
        public STDViewModel ViewModel { get; }

        public DashboardPage(STDViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        public static string TotalRAM;
        public static DriveInfo TheDrive;
        public double AsTransByte2GB = 1024 * 1024 * 1024;
        bool inited = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!inited)
            {
                inited = true;
                var png = this.Icontion.Source;
                string path = (Path.GetFullPath(Environment.ProcessPath) ?? "C:\\").Replace("\\", "/");
                path = path.Split('/')[0];
                TheDrive = new DriveInfo(path);
                this.Icontion.Source = png;
                TotalRAM = SystemUtil.GetSysMemory();
                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                foreach (var item in searcher.Get())
                {
                    CPUModel.Text = (string)item["Name"];
                }
                new Thread(() =>
                {
                    string tmp1,tmp2;
                    while (true)
                    {
                        tmp1 = $"{SystemUtil.GetUsedMemory().ToString()}/{TotalRAM}";
                        tmp2 = $"{(TheDrive.TotalFreeSpace/AsTransByte2GB).ToString("F2")} GB";
                        this.Dispatcher.Invoke(() =>
                        {
                            RAMSize.Text = tmp1;
                            CanUseStorge.Text = tmp2;
                        });
                        Thread.Sleep(10000);
                    }
                })
                { Priority = ThreadPriority.Lowest, IsBackground = true }.Start();
            }
        }

        private void StartShabi_Click(object sender, RoutedEventArgs e)
        {
            AutoRunOnce = true;
            MainWindow.Instance.Navigate(typeof(Console));
            Task.Run((async () =>
            {
                while (Console.loaded == false)
                    await Task.Delay(1000);
                Console.Instance.Dispatcher.Invoke(() =>
                {
                    ButtonAutomationPeer peer = new ButtonAutomationPeer(Console.Instance.StartButton);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv?.Invoke();
                });
            }));
        }
    }
}

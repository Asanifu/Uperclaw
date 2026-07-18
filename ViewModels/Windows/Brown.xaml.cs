using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Uperclaw.ViewModels.Windows
{
    public partial class Brown : Window
    {
        public static Brown Instance { get; set; } = new Brown();

        public Brown()
        {
            InitializeComponent();
        }
        public static bool isLoaded = false;
        private string GetDashboardUrlFromCli()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string nodePath = System.IO.Path.Combine(baseDir, "nodejs", "node.exe");
                string openClawPath = System.IO.Path.Combine(baseDir, "nodejs", "node_modules", "openclaw", "openclaw.mjs");

                if (!System.IO.File.Exists(nodePath) || !System.IO.File.Exists(openClawPath))
                {
                    return "";
                }

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = nodePath,
                    Arguments = $"\"{openClawPath}\" dashboard",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    StandardOutputEncoding = System.Text.Encoding.UTF8,
                    StandardErrorEncoding = System.Text.Encoding.UTF8
                };

                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit(3000);

                    Match match = Regex.Match(output, @"(http[s]?://[^\s]+token=[a-zA-Z0-9\-_]+)");
                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch { }
            return "";
        }

        public async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
            try
            {
                LoadingProgress.Visibility = Visibility.Visible;
                LoadingProgress.Value = 10;

                await Brownser.EnsureCoreWebView2Async(null);

                LoadingProgress.Value = 20;

                Brownser.CoreWebView2.NavigationStarting += (s, args) =>
                {
                    LoadingProgress.Visibility = Visibility.Visible;
                    LoadingProgress.Value = 10;
                };

                Brownser.CoreWebView2.ContentLoading += (s, args) =>
                {
                    LoadingProgress.Value = 50;
                };

                Brownser.CoreWebView2.NavigationCompleted += (s, args) =>
                {
                    LoadingProgress.Value = 100;
                    LoadingProgress.Visibility = Visibility.Collapsed;
                };

                Brownser.CoreWebView2.NewWindowRequested += (s, args) =>
                {
                    args.Handled = true;
                    var newWindow = new Brown();
                    newWindow.Brownser.CoreWebView2.Navigate(args.Uri);
                    newWindow.Show();
                };

                string dashboardUrl = GetDashboardUrlFromCli();

                if (!string.IsNullOrEmpty(dashboardUrl))
                {
                    Brownser.CoreWebView2.Navigate(dashboardUrl);
                }
                else
                {
                    Brownser.CoreWebView2.Navigate("http://127.0.0.1:18789/control-ui");
                }

                LoadingProgress.Value = 100;
                await Task.Delay(300);
                LoadingProgress.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingProgress.Visibility = Visibility.Collapsed;
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Brownser?.CoreWebView2 != null)
                {
                    LoadingProgress.Visibility = Visibility.Visible;
                    LoadingProgress.Value = 0;

                    string dashboardUrl = GetDashboardUrlFromCli();
                    if (!string.IsNullOrEmpty(dashboardUrl))
                    {
                        Brownser.CoreWebView2.Navigate(dashboardUrl);
                    }
                    else
                    {
                        Brownser.CoreWebView2.Reload();
                    }
                }
                else
                {
                    await Brownser.EnsureCoreWebView2Async(null);
                    Brownser.CoreWebView2.Navigate("http://127.0.0.1:18789/control-ui");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"刷新失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingProgress.Visibility = Visibility.Collapsed;
            }
        }
    }
}
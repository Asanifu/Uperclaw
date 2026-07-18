using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
using Uperclaw.ViewModels.Windows;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using Wpf.Ui.Terminal;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace Uperclaw.Views.Pages
{
    public partial class Console : INavigableView<STDViewModel>
    {
        public static Console Instance { get; set; }
        public STDViewModel ViewModel { get; set; }
        public Console(STDViewModel model)
        {
            ViewModel = model;
            Instance = this;
            InitializeComponent();
        }
        public static bool needSende = false;

        private bool IsPortOpen(int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect("127.0.0.1", port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(500, false);
                    if (!success)
                    {
                        return false;
                    }
                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var console = this.PowershellBox.ConPTYTerm;
            if (console == null) return;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string nodePath = Path.Combine(baseDir, "nodejs", "node.exe");
            string openClawPath = Path.Combine(baseDir, "nodejs", "node_modules", "openclaw", "openclaw.mjs");
            string setedConfigFile = Path.Combine(baseDir, "nodejs", "node_modules", "openclaw", "openclaw.json");
            if (!File.Exists(nodePath))
            {
                console.WriteToTerm($"echo 错误：找不到 node.exe，路径：{nodePath}\r\n");
                return;
            }
            if (!File.Exists(openClawPath))
            {
                console.WriteToTerm($"echo 错误：找不到 openclaw.mjs，路径：{openClawPath}\r\n");
                return;
            }
            string openClawDir = Path.GetDirectoryName(openClawPath);
            console.WriteToTerm($"cls\r\n");
            console.WriteToTerm($"cd \"{openClawDir}\"\r\n");
            console.WriteToTerm($"$env:OPENCLAW_CONFIG_PATH=\"{setedConfigFile}\"\r\n");

            var openaiProvider = ClawConfiger.GetModelProvider("openai");
            if (openaiProvider != null && !string.IsNullOrEmpty(openaiProvider.ApiKey))
            {
                console.WriteToTerm($"$env:OPENAI_API_KEY=\"{openaiProvider.ApiKey}\"\r\n");
                console.WriteToTerm($"$env:OPENAI_BASE_URL=\"{openaiProvider.ApiUri}\"\r\n");
            }

            string command = $"& \"{nodePath}\" \"{openClawPath}\" gateway --port 18789\r\n";
            console.WriteToTerm(command);
            needSende = true;
            task = new(async () =>
            {
                while (true)
                {
                    if (needSende)
                    {
                        Thread.Sleep(3000);
                        if (IsPortOpen(18789))
                        {
                            await Task.Run(() =>
                            {
                                string dashboardUrl = "";
                                try
                                {
                                    string baseDir2 = AppDomain.CurrentDomain.BaseDirectory;
                                    string nodePath2 = Path.Combine(baseDir2, "nodejs", "node.exe");
                                    string openClawPath2 = Path.Combine(baseDir2, "nodejs", "node_modules", "openclaw", "openclaw.mjs");

                                    if (File.Exists(nodePath2) && File.Exists(openClawPath2))
                                    {
                                        ProcessStartInfo startInfo = new ProcessStartInfo
                                        {
                                            FileName = nodePath2,
                                            Arguments = $"\"{openClawPath2}\" dashboard",
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
                                                dashboardUrl = match.Groups[1].Value;
                                            }
                                        }
                                    }
                                }
                                catch { }

                                string finalUrl = string.IsNullOrEmpty(dashboardUrl) ? "http://127.0.0.1:18789/control-ui" : dashboardUrl;

                                this.Dispatcher.Invoke(() =>
                                {
                                    try
                                    {
                                        Process.Start(new ProcessStartInfo
                                        {
                                            FileName = finalUrl,
                                            UseShellExecute = true
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"打开浏览器失败：{ex.Message}");
                                    }
                                });
                            });

                            needSende = false;
                            break;
                        }
                    }
                    else
                    {
                        needSende = false;
                        return;
                    }
                }
            })
            { IsBackground = true };
            task.Start();
        }
        public Thread task = null;
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            needSende = false;
            var console = this.PowershellBox.ConPTYTerm;
            if (console == null) return;
            console.WriteToTerm("\x03 \r\ncls\r\necho Process ended.\r\n");
        }
        public static bool loaded = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }
    }
}
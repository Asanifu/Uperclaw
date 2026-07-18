using System.Windows;
using System.Windows.Controls;

namespace Uperclaw.Views.Pages
{
    public partial class DataPage : Page
    {
        private bool isLoaded = false;

        public DataPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var provider = ClawConfiger.GetModelProvider("deepseek");
            if (provider != null)
            {
                DeepSeek_ApiKeyTextBox.Text = provider.ApiKey;
                DeepSeek_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    DeepSeek_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("doubao");
            if (provider != null)
            {
                Doubao_ApiKeyTextBox.Text = provider.ApiKey;
                Doubao_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    Doubao_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("openai");
            if (provider != null)
            {
                OpenAI_ApiKeyTextBox.Text = provider.ApiKey;
                OpenAI_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    OpenAI_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("groq");
            if (provider != null)
            {
                Groq_ApiKeyTextBox.Text = provider.ApiKey;
                Groq_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    Groq_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("zhipu");
            if (provider != null)
            {
                Zhipu_ApiKeyTextBox.Text = provider.ApiKey;
                Zhipu_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    Zhipu_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("minimax");
            if (provider != null)
            {
                MiniMax_ApiKeyTextBox.Text = provider.ApiKey;
                MiniMax_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    MiniMax_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("qwen");
            if (provider != null)
            {
                Qwen_ApiKeyTextBox.Text = provider.ApiKey;
                Qwen_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    Qwen_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            provider = ClawConfiger.GetModelProvider("kimi");
            if (provider != null)
            {
                Kimi_ApiKeyTextBox.Text = provider.ApiKey;
                Kimi_UrlTextBox.Text = provider.ApiUri;
                if (provider.Models != null && provider.Models.Length > 0)
                {
                    Kimi_ModelTextBox.Text = string.Join(", ", provider.Models);
                }
            }

            isLoaded = true;
        }

        private void ApiKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isLoaded) return;

            var textBox = sender as TextBox;
            if (textBox == null) return;

            switch (textBox.Name)
            {
                case "OpenAI_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("openai", textBox.Text);
                    break;
                case "OpenAI_UrlTextBox":
                    ClawConfiger.SetModelApiUri("openai", textBox.Text);
                    break;
                case "OpenAI_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("openai", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("openai", Array.Empty<string>());
                    }
                    break;

                case "Groq_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("groq", textBox.Text);
                    break;
                case "Groq_UrlTextBox":
                    ClawConfiger.SetModelApiUri("groq", textBox.Text);
                    break;
                case "Groq_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("groq", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("groq", Array.Empty<string>());
                    }
                    break;

                case "Doubao_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("doubao", textBox.Text);
                    break;
                case "Doubao_UrlTextBox":
                    ClawConfiger.SetModelApiUri("doubao", textBox.Text);
                    break;
                case "Doubao_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("doubao", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("doubao", Array.Empty<string>());
                    }
                    break;

                case "Zhipu_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("zhipu", textBox.Text);
                    break;
                case "Zhipu_UrlTextBox":
                    ClawConfiger.SetModelApiUri("zhipu", textBox.Text);
                    break;
                case "Zhipu_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("zhipu", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("zhipu", Array.Empty<string>());
                    }
                    break;

                case "MiniMax_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("minimax", textBox.Text);
                    break;
                case "MiniMax_UrlTextBox":
                    ClawConfiger.SetModelApiUri("minimax", textBox.Text);
                    break;
                case "MiniMax_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("minimax", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("minimax", Array.Empty<string>());
                    }
                    break;

                case "DeepSeek_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("deepseek", textBox.Text);
                    break;
                case "DeepSeek_UrlTextBox":
                    ClawConfiger.SetModelApiUri("deepseek", textBox.Text);
                    break;
                case "DeepSeek_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("deepseek", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("deepseek", Array.Empty<string>());
                    }
                    break;

                case "Qwen_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("qwen", textBox.Text);
                    break;
                case "Qwen_UrlTextBox":
                    ClawConfiger.SetModelApiUri("qwen", textBox.Text);
                    break;
                case "Qwen_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("qwen", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("qwen", Array.Empty<string>());
                    }
                    break;

                case "Kimi_ApiKeyTextBox":
                    ClawConfiger.SetModelApiKey("kimi", textBox.Text);
                    break;
                case "Kimi_UrlTextBox":
                    ClawConfiger.SetModelApiUri("kimi", textBox.Text);
                    break;
                case "Kimi_ModelTextBox":
                    if (!string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        var models = textBox.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        ClawConfiger.SetModelModels("kimi", models);
                    }
                    else
                    {
                        ClawConfiger.SetModelModels("kimi", Array.Empty<string>());
                    }
                    break;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch
            {

            }
            e.Handled = true;
        }
    }
}
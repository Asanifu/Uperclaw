using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Uperclaw
{
    public class ClawConfiger
    {
        public class ModelProvision
        {
            public string ApiUri { get; set; }
            public string ApiKey { get; set; }
            public string[] Models { get; set; }

            public ModelProvision(string uri = "", string key = "", string[] models = null)
            {
                ApiUri = uri ?? "";
                ApiKey = key ?? "";
                Models = models ?? Array.Empty<string>();
            }
        }

        public class ConfigLoadResult
        {
            public List<string> PluginPaths { get; set; } = new();
            public Dictionary<string, ModelProvision> ModelProviders { get; set; } = new();
            public Dictionary<string, bool> PluginEntries { get; set; } = new();
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; }
        }

        public static ConfigLoadResult CurrentConfig { get; private set; } = new();
        public static string TargetPluginPath { get; private set; } = "";

        public static string ConfigFilePath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nodejs", "node_modules", "openclaw", "openclaw.json");
            }
        }

        public static void INIT()
        {
            CurrentConfig = new ConfigLoadResult();
            string configPath = ConfigFilePath;
            TargetPluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nodejs", "node_modules", "@tencent-weixin").Replace("\\", "/");

            if (!File.Exists(configPath))
            {
                CurrentConfig.ErrorMessage = $"配置文件不存在: {configPath}";
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] {CurrentConfig.ErrorMessage}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(configPath, Encoding.UTF8);
                using var document = JsonDocument.Parse(jsonContent);
                var root = document.RootElement;

                if (root.TryGetProperty("plugins", out JsonElement pluginsElement))
                {
                    if (pluginsElement.TryGetProperty("load", out JsonElement loadElement))
                    {
                        if (loadElement.TryGetProperty("paths", out JsonElement pathsElement))
                        {
                            if (pathsElement.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var pathElement in pathsElement.EnumerateArray())
                                {
                                    string path = pathElement.GetString();
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        string normalizedPath = path.Replace('\\', '/');
                                        CurrentConfig.PluginPaths.Add(normalizedPath);
                                        System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 插件路径: {normalizedPath}");
                                    }
                                }
                            }
                        }
                    }

                    if (CurrentConfig.PluginPaths.Count == 0)
                    {
                        CurrentConfig.PluginPaths.Add(TargetPluginPath);
                        System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 未找到 paths 节点，使用默认插件路径: {TargetPluginPath}");
                    }

                    if (pluginsElement.TryGetProperty("entries", out JsonElement entriesElement))
                    {
                        foreach (var entry in entriesElement.EnumerateObject())
                        {
                            string pluginName = entry.Name;
                            if (entry.Value.TryGetProperty("enabled", out JsonElement enabledElement))
                            {
                                bool isEnabled = enabledElement.GetBoolean();
                                CurrentConfig.PluginEntries[pluginName] = isEnabled;
                                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 插件 {pluginName} -> 启用: {isEnabled}");
                            }
                        }
                    }
                }
                else
                {
                    CurrentConfig.PluginPaths.Add(TargetPluginPath);
                    System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 未找到 plugins 节点，使用默认插件路径: {TargetPluginPath}");
                }

                if (root.TryGetProperty("models", out JsonElement modelsElement))
                {
                    if (modelsElement.TryGetProperty("providers", out JsonElement providersElement))
                    {
                        foreach (var providerProperty in providersElement.EnumerateObject())
                        {
                            string modelName = providerProperty.Name;
                            var modelConfig = providerProperty.Value;

                            string apiUri = "";
                            string apiKey = "";
                            string[] models = Array.Empty<string>();

                            if (modelConfig.TryGetProperty("baseUrl", out JsonElement baseUriElement))
                            {
                                apiUri = baseUriElement.GetString() ?? "";
                            }
                            else if (modelConfig.TryGetProperty("apiUri", out JsonElement apiUriElement))
                            {
                                apiUri = apiUriElement.GetString() ?? "";
                            }
                            else if (modelConfig.TryGetProperty("apiUrl", out JsonElement apiUrlElement))
                            {
                                apiUri = apiUrlElement.GetString() ?? "";
                            }

                            if (modelConfig.TryGetProperty("apiKey", out JsonElement keyElement))
                            {
                                apiKey = keyElement.GetString() ?? "";
                            }
                            else if (modelConfig.TryGetProperty("token", out JsonElement tokenElement))
                            {
                                apiKey = tokenElement.GetString() ?? "";
                            }

                            if (modelConfig.TryGetProperty("models", out JsonElement modelsElement2))
                            {
                                if (modelsElement2.ValueKind == JsonValueKind.Array)
                                {
                                    var modelList = new List<string>();
                                    foreach (var modelItem in modelsElement2.EnumerateArray())
                                    {
                                        if (modelItem.ValueKind == JsonValueKind.String)
                                        {
                                            string model = modelItem.GetString();
                                            if (!string.IsNullOrEmpty(model))
                                            {
                                                modelList.Add(model);
                                            }
                                        }
                                        else if (modelItem.ValueKind == JsonValueKind.Object)
                                        {
                                            if (modelItem.TryGetProperty("name", out JsonElement nameElement))
                                            {
                                                string model = nameElement.GetString();
                                                if (!string.IsNullOrEmpty(model))
                                                {
                                                    modelList.Add(model);
                                                }
                                            }
                                            else if (modelItem.TryGetProperty("id", out JsonElement idElement))
                                            {
                                                string model = idElement.GetString();
                                                if (!string.IsNullOrEmpty(model))
                                                {
                                                    modelList.Add(model);
                                                }
                                            }
                                        }
                                    }
                                    models = modelList.ToArray();
                                }
                            }

                            if (!string.IsNullOrEmpty(apiUri))
                            {
                                CurrentConfig.ModelProviders[modelName] = new ModelProvision(apiUri, apiKey, models);
                                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 加载模型: {modelName} -> {apiUri} (模型数: {models.Length})");
                            }
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 配置文件中未找到 models.providers 节点");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 配置文件中未找到 models 节点");
                }

                CurrentConfig.IsValid = true;
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 配置加载完成: 插件路径 {CurrentConfig.PluginPaths.Count} 个, 模型 {CurrentConfig.ModelProviders.Count} 个");

                if (!CurrentConfig.PluginPaths.Contains(TargetPluginPath))
                {
                    CurrentConfig.PluginPaths.Add(TargetPluginPath);
                    System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 添加目标插件路径: {TargetPluginPath}");
                    SaveConfig();
                }
            }
            catch (JsonException ex)
            {
                CurrentConfig.ErrorMessage = $"JSON 解析失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] {CurrentConfig.ErrorMessage}");
            }
            catch (Exception ex)
            {
                CurrentConfig.ErrorMessage = $"读取配置文件失败: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] {CurrentConfig.ErrorMessage}");
            }
        }

        public static void SetModelApiKey(string modelName, string apiKey)
        {
            if (CurrentConfig.ModelProviders.TryGetValue(modelName, out var provider))
            {
                provider.ApiKey = apiKey;
                SaveConfig();
            }
        }

        public static void SetModelApiUri(string modelName, string apiUri)
        {
            if (CurrentConfig.ModelProviders.TryGetValue(modelName, out var provider))
            {
                provider.ApiUri = apiUri;
                SaveConfig();
            }
        }

        public static void SetModelModels(string modelName, string[] models)
        {
            if (CurrentConfig.ModelProviders.TryGetValue(modelName, out var provider))
            {
                provider.Models = models ?? Array.Empty<string>();
                SaveConfig();
            }
        }

        public static void AddModel(string modelName, string apiUri, string apiKey, string[] models = null)
        {
            if (!CurrentConfig.ModelProviders.ContainsKey(modelName))
            {
                CurrentConfig.ModelProviders[modelName] = new ModelProvision(apiUri, apiKey, models);
                SaveConfig();
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 添加模型: {modelName}");
            }
        }

        public static void RemoveModel(string modelName)
        {
            if (CurrentConfig.ModelProviders.Remove(modelName))
            {
                SaveConfig();
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 移除模型: {modelName}");
            }
        }

        public static void SaveConfig()
        {
            try
            {
                string configPath = ConfigFilePath;

                var entriesDict = new Dictionary<string, object>();
                if (CurrentConfig.PluginEntries.Count > 0)
                {
                    foreach (var entry in CurrentConfig.PluginEntries)
                    {
                        entriesDict[entry.Key] = new { enabled = entry.Value };
                    }
                }
                else
                {
                    entriesDict["openclaw_weixin"] = new { enabled = true };
                }

                var providersDict = new Dictionary<string, object>();
                foreach (var kvp in CurrentConfig.ModelProviders)
                {
                    var modelsList = new List<object>();
                    if (kvp.Value.Models != null)
                    {
                        foreach (var model in kvp.Value.Models)
                        {
                            if (!string.IsNullOrEmpty(model))
                            {
                                modelsList.Add(new { id = model, name = model });
                            }
                        }
                    }

                    providersDict[kvp.Key] = new
                    {
                        baseUrl = kvp.Value.ApiUri,
                        apiKey = kvp.Value.ApiKey,
                        models = modelsList
                    };
                }

                var configObject = new
                {
                    gateway = new
                    {
                        mode = "local",
                        auth = new
                        {
                            mode = "token",
                            token = "https://github.com/uxiaohan/vh-claw"
                        }
                    },
                    meta = new
                    {
                        lastTouchedVersion = "2026.4.2",
                        lastTouchedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    },
                    plugins = new
                    {
                        load = new
                        {
                            paths = CurrentConfig.PluginPaths.Distinct().ToArray()
                        },
                        entries = entriesDict
                    },
                    models = new
                    {
                        providers = providersDict
                    }
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(configObject, options);
                File.WriteAllText(configPath, json, Encoding.UTF8);
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 配置已保存: {configPath}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 保存配置失败: {ex.Message}");
            }
        }

        public static void AddPluginPath(string path)
        {
            string normalizedPath = path.Replace('\\', '/');
            if (!CurrentConfig.PluginPaths.Contains(normalizedPath))
            {
                CurrentConfig.PluginPaths.Add(normalizedPath);
                SaveConfig();
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 添加插件路径: {normalizedPath}");
            }
        }

        public static void RemovePluginPath(string path)
        {
            string normalizedPath = path.Replace('\\', '/');
            if (CurrentConfig.PluginPaths.Remove(normalizedPath))
            {
                SaveConfig();
                System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 移除插件路径: {normalizedPath}");
            }
        }

        public static void ClearPluginPaths()
        {
            CurrentConfig.PluginPaths.Clear();
            CurrentConfig.PluginPaths.Add(TargetPluginPath);
            SaveConfig();
            System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 已清空插件路径，保留目标路径");
        }

        public static List<string> GetPluginPaths()
        {
            return CurrentConfig.PluginPaths;
        }

        public static ModelProvision GetModelProvider(string name)
        {
            CurrentConfig.ModelProviders.TryGetValue(name, out var provider);
            return provider;
        }

        public static Dictionary<string, ModelProvision> GetAllModelProviders()
        {
            return CurrentConfig.ModelProviders;
        }

        public static void SetPluginEnabled(string pluginName, bool enabled)
        {
            CurrentConfig.PluginEntries[pluginName] = enabled;
            SaveConfig();
            System.Diagnostics.Debug.WriteLine($"[ClawConfiger] 插件 {pluginName} -> 启用: {enabled}");
        }

        public static bool IsPluginEnabled(string pluginName)
        {
            return CurrentConfig.PluginEntries.TryGetValue(pluginName, out bool enabled) && enabled;
        }

        public static bool IsConfigValid()
        {
            return CurrentConfig.IsValid;
        }

        public static string GetErrorMessage()
        {
            return CurrentConfig.ErrorMessage;
        }

        public static string GetTargetPluginPath()
        {
            return TargetPluginPath;
        }
    }
}
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using AppEndCommon;
using AppEndServer;

namespace Zzz
{
    public static class Ai
    {
        // Method: Zzz.Ai.Generate
        // Inputs: { prompt:string, model:string, provider?:string }
        public static object? Generate(string prompt, string model, string? provider, AppEndUser? Actor)
        {
            try
            {
                var task = AiServices.GenerateTextAsync(prompt, model, provider);
                task.Wait();
                var text = task.Result ?? "";
                if (string.IsNullOrWhiteSpace(text)) return new { Result = new { Error = "AiNotConfiguredOrEmptyResponse", Text = text } };
                return new { Result = new { Text = text } };
            }
            catch (Exception ex)
            {
                return new { Result = new { Error = ex.Message } };
            }
        }

        // Method: Zzz.Ai.ConfigStatus
        // Returns providers and models dynamically (always include all providers, even without ApiKey)
        public static object? ConfigStatus()
        {
            try
            {
                var root = AppEndSettings.AppSettings;
                var appEnd = root?[AppEndSettings.ConfigSectionName]?.AsObject();
                var ai = appEnd?["Ai"]?.AsObject();
                var google = ai?["Google"]?.AsObject();
                var github = ai?["GitHub"]?.AsObject();

                string? googleKey = google?["ApiKey"]?.ToString();
                string? githubKey = github?["ApiKey"]?.ToString();
                bool hasGoogle = !string.IsNullOrWhiteSpace(googleKey);
                bool hasGitHub = !string.IsNullOrWhiteSpace(githubKey);
                bool hasAnyApiKey = hasGoogle || hasGitHub;

                static string[] ReadModels(JsonObject? section)
                {
                    var modelsNode = section?["Models"];
                    if (modelsNode is JsonArray arr)
                    {
                        var list = new System.Collections.Generic.List<string>();
                        foreach (var n in arr)
                        {
                            var s = n?.ToString();
                            if (!string.IsNullOrWhiteSpace(s)) list.Add(s);
                        }
                        return list.ToArray();
                    }
                    try
                    {
                        if (modelsNode != null)
                        {
                            var json = modelsNode.ToJsonString();
                            var des = JsonSerializer.Deserialize<string[]>(json);
                            return des ?? Array.Empty<string>();
                        }
                    }
                    catch { }
                    return Array.Empty<string>();
                }

                var providers = new System.Collections.Generic.List<object>();
                if (google != null) providers.Add(new { Name = "Google", HasApiKey = hasGoogle, Models = ReadModels(google) });
                if (github != null) providers.Add(new { Name = "GitHub", HasApiKey = hasGitHub, Models = ReadModels(github) });

                return new { Result = new { HasApiKey = hasAnyApiKey, Providers = providers } };
            }
            catch (Exception ex)
            {
                return new { Result = new { HasApiKey = false, Providers = Array.Empty<object>(), Message = ex.Message } };
            }
        }
    }
}

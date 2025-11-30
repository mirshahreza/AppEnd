using System;
using System.Text.Json;
using AppEndCommon;
using AppEndServer;

namespace Zzz
{
    public static class Ai
    {
        // Method: Zzz.Ai.Generate
        // Inputs: ClientQueryJE.Params: [{ Name: "Prompt", Value: "..." }, { Name: "Model", Value: "gpt-4o-mini" }]
        public static object? Generate(string prompt, string model, AppEndUser? Actor)
        {
            try
            {
                var task = AiServices.GenerateTextAsync(prompt, model);
                task.Wait();
                var text = task.Result ?? "";

                if (string.IsNullOrWhiteSpace(text))
                {
                    // Likely missing key or invalid model
                    return new { Result = new { Error = "AiNotConfiguredOrEmptyResponse", Text = text } };
                }

                return new { Result = new { Text = text } };
            }
            catch (Exception ex)
            {
                return new { Result = new { Error = ex.Message } };
            }
        }

        // Method: Zzz.Ai.ConfigStatus
        // Returns whether AppEnd.Ai.GitHub.ApiKey exists in appsettings.
        public static object? ConfigStatus()
        {
            try
            {
                string? apiKey = GetApiKeyInternal();
                bool hasKey = !string.IsNullOrWhiteSpace(apiKey);
                string message = hasKey ? "Configured" : "GitHub AI key missing. Set AppEnd.Ai.GitHub.ApiKey in appsettings (Settings menu).";
                return new { Result = new { HasApiKey = hasKey, Message = message } };
            }
            catch (Exception ex)
            {
                return new { Result = new { HasApiKey = false, Message = ex.Message } };
            }
        }

        private static string? GetApiKeyInternal()
        {
            var root = AppEndSettings.AppSettings;
            var appEnd = root?[AppEndSettings.ConfigSectionName]?.AsObject();
            var ai = appEnd?["Ai"]?.AsObject();
            var gh = ai?["GitHub"]?.AsObject();
            return gh?["ApiKey"]?.ToString();
        }
    }
}

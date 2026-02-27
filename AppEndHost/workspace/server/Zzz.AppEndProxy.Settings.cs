namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region Settings
        public static object? GetAppEndSettings(AppEndUser? Actor)
        {
            try
            {
                // Read directly from AppSettings.AppSettings which is the root JsonNode
                var appEndNode = AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName];
                if (appEndNode == null)
                {
                    return new Dictionary<string, object>();
                }

                // Convert JsonNode to a clean Dictionary structure with proper types
                return ParseJsonNode(appEndNode);
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object> { { "error", ex.Message }, { "stack", ex.StackTrace ?? "" } };
            }
        }

        private static object? ParseJsonNode(JsonNode? node)
        {
            if (node == null) return null;

            if (node is JsonValue jval)
            {
                // Extract primitive types properly
                if (jval.TryGetValue<string>(out var s)) return s;
                if (jval.TryGetValue<int>(out var i)) return i;
                if (jval.TryGetValue<long>(out var l)) return l;
                if (jval.TryGetValue<bool>(out var b)) return b;
                if (jval.TryGetValue<double>(out var d)) return d;
                return jval.ToString();
            }

            if (node is JsonArray jarr)
            {
                var list = new List<object?>();
                foreach (var item in jarr)
                {
                    list.Add(ParseJsonNode(item));
                }
                return list;
            }

            if (node is JsonObject jobj)
            {
                var dict = new Dictionary<string, object?>();
                foreach (var prop in jobj)
                {
                    dict[prop.Key] = ParseJsonNode(prop.Value);
                }
                return dict;
            }

            return node.ToString();
        }

        public static object? SaveAppEndSettings(AppEndUser? Actor, JsonElement AppEnd)
        {
            try
            {
                // Overwrite only the AppEnd section and persist
                var node = JsonNode.Parse(AppEnd.GetRawText());
                if (node is null) return false;
                AppEndSettings.AppSettings[AppEndSettings.ConfigSectionName] = node;
                AppEndSettings.Save();
                // refresh in-memory cache
                AppEndSettings.RefereshSettings();
                
                // Reload scheduler tasks if settings were saved successfully
                try
                {
                    var manager = GetSchedulerManager();
                    manager.ReloadTasksFromSettings();
                }
                catch (Exception ex)
                {
                    // Log but don't fail the save operation
                    LogMan.LogWarning($"Settings saved but failed to reload scheduler tasks: {ex.Message}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}

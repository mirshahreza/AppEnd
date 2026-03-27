namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region Ai
        public static object? Generate(AppEndUser? Actor, string prompt, string model)
        {
			return AiServices.GenerateFromAppSettingsAsync(prompt, model).GetAwaiter().GetResult();
        }
        public static object? GetAiProvidersWithModels(AppEndUser? actor)
        {
            return AiServices.GetAiProvidersWithModels();
        }
        public static object? RunAiEnrichment(string ConnectionName, JsonElement StructureIds, string? Model)
        {
            var ids = new System.Collections.Generic.List<string>();
            if (StructureIds.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in StructureIds.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String)
                        ids.Add(item.GetString() ?? "");
                }
            }
            return AiEnrichmentServices.EnrichStructuresAsync(ConnectionName, ids, Model).GetAwaiter().GetResult();
        }
        #endregion
    }
}

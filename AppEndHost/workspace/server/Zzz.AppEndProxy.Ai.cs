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
        #endregion
    }
}

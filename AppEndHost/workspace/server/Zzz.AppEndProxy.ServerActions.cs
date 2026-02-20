namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region ServerActions
        public static object? RebuildProject()
        {
            return BuildServices.RebuildProject();
        }
        public static object? RestartApp()
        {
            
            return true;
        }
        #endregion
    }
}

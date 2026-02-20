namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region ActivityLog
        public static object? FlushActivityLogs(AppEndUser? Actor)
        {
            try
            {
                LogMan.Flush();
                return new { Success = true, Message = "Activity logs flushed successfully" };
            }
            catch (Exception ex)
            {
                return new { Success = false, Message = ex.Message };
            }
        }
        #endregion
    }
}

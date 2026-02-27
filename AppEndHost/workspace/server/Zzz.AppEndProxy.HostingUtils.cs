namespace Zzz
{
    public static partial class AppEndProxy
    {
        #region HostingUtils
        public static object? GetAppEndSummary()
		{
			return HostingUtils.GetAppEndSummary();
		}
        public static object? TestMe(string s)
        {
            LogMan.LogConsole($"TestMe called with : {s}");
            return "TestMe was ok :)";
        }
        public static object? PingMe()
        {
            LogMan.LogConsole("PingMe called");
            return "I am at your service :)";
        }

        public static object? LongRunningDemo(int Seconds, CancellationToken ct)
        {
            int total = Seconds * 10;
            for (int i = 0; i < total; i++)
            {
                ct.ThrowIfCancellationRequested();
                Thread.Sleep(100);
            }
            return new { Message = "LongRunningDemo completed successfully", Duration = Seconds, CompletedAt = DateTime.UtcNow };
        }

        #endregion
    }
}

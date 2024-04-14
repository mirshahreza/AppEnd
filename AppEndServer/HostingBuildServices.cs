using AppEndDbIO;
using AppEndDynaCode;

namespace AppEndServer
{
	public static class HostingBuildServices
	{
		public static bool RebuildProject()
		{
			DynaCode.Refresh();
			return true;
		}

	}

	public class BuildInfo(DbDialog dbDialog, ClientUI clientUI)
	{
		public DbDialog DbDialog { set; get; } = dbDialog;
		public ClientUI ClientUI { set; get; } = clientUI;
		public Dictionary<string, object> Parameters { set; get; } = [];
	}

}

using AppEndDbIO;

namespace AppEndServer
{
    public class BuildInfo(DbDialog dbDialog, ClientUI clientUI)
	{
		public DbDialog DbDialog { set; get; } = dbDialog;
		public ClientUI ClientUI { set; get; } = clientUI;
		public Dictionary<string, object> Parameters { set; get; } = [];
	}
}

﻿using AppEndDbIO;
using AppEndDynaCode;

namespace AppEndServer
{
	public static class BuildServices
	{
		public static bool RebuildProject()
		{
			DynaCode.ReBuild();
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

using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace AppEndServer
{
	public static class HostingDeployServices
	{


		public static object? StartDeployToNode(AppEndBackgroundWorkerQueue backgroundWorker, bool considerLastTime, int ind)
		{
			backgroundWorker.QueueBackgroundWorkItem(async token =>
			{
				await ExecDeployToNode(considerLastTime, ind);
			});
			return true;
		}
		private static Task ExecDeployToNode(bool considerLastTime, int ind)
		{
			string logFile = GetDeployLogFileName(ind, considerLastTime);
			if (File.Exists(logFile))
			{
				JArray jArrFilesToDo = GetNodeToDoItems(considerLastTime, ind, false);
				foreach (JObject joFile in jArrFilesToDo)
				{
					if ((bool)joFile["Done"] == false)
					{
						Thread.Sleep(100);
						joFile["Done"] = true;
						File.WriteAllText(logFile, jArrFilesToDo.ToJsonStringByNewtonsoft());
					}
				}
				UpdateNodeLastDeployToNow(ind);
				File.Delete(logFile);
			}
			return Task.CompletedTask;
		}
		public static JArray GetNodeToDoItems(bool considerLastTime, int ind, bool overrideExistingCalc)
		{
			string logFile = GetDeployLogFileName(ind, considerLastTime);
			if (!File.Exists(logFile) || overrideExistingCalc == true)
			{
				JArray nodes = GetNodes();

				JObject? jn = (JObject)nodes[ind.ToIntSafe()];
				JArray arr = HostingUtils.GetHostRootDirectory().GetFilesRecursiveWithInfo();
				if (considerLastTime)
				{
					JArray list = [];
					JObject n = (JObject)nodes[ind];
					string dtStr = n["LastDeploy"].ToStringEmpty();
					if (dtStr.Trim() == "") dtStr = DateTime.Now.AddYears(-2).ToString();
					foreach (var item in arr)
					{
						if (!dtStr.IsNullOrEmpty() && Convert.ToDateTime(item["LastWrite"].ToStringEmpty()) > Convert.ToDateTime(dtStr))
						{
							string fp = item["FilePath"].ToStringEmpty().Replace(HostingUtils.GetHostRootDirectory().FullName, "").Replace("\\", "/");
							if (!IsDirtyToDeploy(fp))
							{
								item["FilePath"] = fp;
								item["Done"] = false;
								list.Add(item);
							}
						}
					}
					arr = list;
				}
				if (arr.Count > 0)
				{
					JArray sorted = new JArray(arr.OrderBy(obj => (DateTime)obj["LastWrite"]).Reverse());
					File.WriteAllText(logFile, sorted.ToJsonStringByNewtonsoft());
				}
			}

			if (!File.Exists(logFile)) return [];
			return File.ReadAllText(logFile).ToJArrayByNewtonsoft();
		}
		public static JArray GetNodes()
		{
			if (!File.Exists(DeployNodesFileName)) return [];
			return File.ReadAllText(DeployNodesFileName).ToJArrayByNewtonsoft();
		}
		public static void RemoveNode(string ind)
		{
			var nodes = GetNodes();
			JObject? jn = (JObject)nodes[ind.ToIntSafe()];
			if (jn != null) nodes.Remove(jn);
			HostingUtils.GetHostRootDirectory().Delete("deploy_" + ind + "_*");
			File.WriteAllText(DeployNodesFileName, nodes.ToJsonStringByNewtonsoft());
		}
		public static void CreateUpdateNode(int ind, string ip, string port, string name, string userName, string password)
		{
			JArray nodes = GetNodes();
			if (ind == -1)
			{
				JObject jn = new();
				jn["Name"] = name;
				jn["Ip"] = ip;
				jn["Port"] = port;
				jn["UserName"] = userName;
				jn["Password"] = password;
				nodes.Add(JsonNode.Parse(jn.ToJsonStringByNewtonsoft()));
			}
			else
			{
				nodes[ind]["Name"] = name;
				nodes[ind]["Ip"] = ip;
				nodes[ind]["Port"] = port;
				nodes[ind]["UserName"] = userName;
				nodes[ind]["Password"] = password;
			}
			File.WriteAllText(DeployNodesFileName, nodes.ToJsonStringByNewtonsoft());
		}
		public static void UpdateNodeLastDeployToNow(int ind)
		{
			JArray nodes = GetNodes();
			Object? jn = nodes[ind.ToIntSafe()];
			if (jn != null)
			{
				((JObject)nodes[ind.ToIntSafe()])["LastDeploy"] = DateTime.Now.ToString();
			}
			File.WriteAllText(DeployNodesFileName, nodes.ToJsonStringByNewtonsoft());
		}

		public static bool IsDirtyToDeploy(string fp)
		{
			if (fp.StartsWithIgnoreCase("/bin/")) return true;
			if (fp.StartsWithIgnoreCase("/obj/")) return true;
			if (fp.StartsWithIgnoreCase("/deploy_")) return true;
			if (fp.StartsWithIgnoreCase("/DynaAsm")) return true;
			if (fp.ContainsIgnoreCase(".csproj")) return true;
			if (fp.ContainsIgnoreCase("program.cs")) return true;
			return false;
		}


		private static string GetDeployLogFileName(int nodeIndex, bool considerLastTime)
		{
			return $"deploy_{nodeIndex}_{considerLastTime.ToString().ToLower()}.json";
		}
		private static string DeployNodesFileName
		{
			get 
			{
				return "deploy_nodes.json";
			}
		}


	}
}

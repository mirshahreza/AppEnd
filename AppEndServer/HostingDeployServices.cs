using AppEndCommon;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Nodes;
using WinSCP;

namespace AppEndServer
{
	public static class HostingDeployServices
	{


		public static object? StartDeployToNode(AppEndBackgroundWorkerQueue backgroundWorker, int nodeIndex)
		{
			JObject joNode = GetNode(nodeIndex);
			if(joNode["InProgress"].ToBooleanSafe() == false)
			{
				UpdateNodeInProgress(nodeIndex, true);
				backgroundWorker.QueueBackgroundWorkItem(async token =>
				{
					await ExecDeployToNode(nodeIndex);
				});
			}
			return true;
		}
		private static Task ExecDeployToNode(int nodeIndex)
		{
			try
			{
				JObject joNode = GetNode(nodeIndex);
				string remotePath = joNode["RemotePath"].ToStringEmpty();
				Session session = CreateSftpSession(joNode);
				TransferOptions transferOptions = new() { FileMask = "| deploy*; DynaAsm*" };
				var r = session.SynchronizeDirectories(SynchronizationMode.Remote, HostingUtils.GetHostRootDirectory().FullName, remotePath, false, options: transferOptions);

				UpdateNodeLastDeployToNow(nodeIndex);
			}
			catch (Exception ex)
			{
				StaticMethods.LogImmed(ex.Message, "log", "", "deploy_");
			}
			UpdateNodeInProgress(nodeIndex, false);
			return Task.CompletedTask;
		}

		private static string CalculateFileMask(JArray jArrayFilesToDo)
		{
			string res = "";
			string sep = "";
			foreach(JObject joF in jArrayFilesToDo)
			{
				res += sep + new FileInfo(joF["FilePath"].ToStringEmpty()).Name;
				sep = "; ";
			}
			return res;
		}

		private static Session CreateSftpSession(JObject joNode)
		{
			string host = joNode["Ip"].ToStringEmpty();
			string userName = joNode["UserName"].ToStringEmpty();
			int port = joNode["Port"].ToIntSafe();
			string password = joNode["Password"].ToStringEmpty();

			WinSCP.SessionOptions sessionOptions = new()
			{
				Protocol = WinSCP.Protocol.Sftp,
				HostName = host,
				PortNumber = port,
				UserName = userName,
				Password = password
			};

			WinSCP.Session session = new();
			string fingerPrint = session.ScanFingerprint(sessionOptions, "SHA-256");
			sessionOptions.SshHostKeyFingerprint = fingerPrint;
			session.Open(sessionOptions);

			return session;
		}

		public static JArray GetNodeToDoItems(int ind)
		{
			JArray nodes = GetNodes();
			JObject? jn = (JObject)nodes[ind.ToIntSafe()];
			JArray arr = HostingUtils.GetHostRootDirectory().GetFilesRecursiveWithInfo();
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
			return new JArray(list.OrderBy(obj => (DateTime)obj["LastWrite"]).Reverse());
		}
		public static JObject GetNode(int ind)
		{
			return (JObject)GetNodes()[ind];
		}
		public static JArray GetNodes()
		{
			if (!File.Exists(DeployNodesFileName)) return [];
			JArray arr = File.ReadAllText(DeployNodesFileName).ToJArrayByNewtonsoft();
			return arr;
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
			Thread.Sleep(1000);
		}
		public static void UpdateNodeInProgress(int ind,bool inProgress)
		{
			JArray nodes = GetNodes();
			Object? jn = nodes[ind.ToIntSafe()];
			if (jn != null)
			{
				((JObject)nodes[ind.ToIntSafe()])["InProgress"] = inProgress;
			}
			File.WriteAllText(DeployNodesFileName, nodes.ToJsonStringByNewtonsoft());
			Thread.Sleep(1000);
		}
		public static bool IsDirtyToDeploy(string fp)
		{
			if (fp.StartsWithIgnoreCase("/bin/")) return true;
			if (fp.StartsWithIgnoreCase("/obj/")) return true;
			if (fp.StartsWithIgnoreCase("/DynaAsm")) return true;
			if (fp.StartsWithIgnoreCase("/log/")) return true;
			if (fp.ContainsIgnoreCase(".csproj")) return true;
			if (fp.ContainsIgnoreCase("deploy_nodes.json")) return true;
			if (fp.ContainsIgnoreCase("program.cs")) return true;
			return false;
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

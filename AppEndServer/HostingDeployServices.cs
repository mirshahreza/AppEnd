using AngleSharp.Dom;
using AppEndCommon;
using AppEndDynaCode;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
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
			if (!SV.SharedMemoryCache.TryGetValue(GenItemKey(joNode), out object? result))
			{
				JObject joInfo = (JObject)joNode.DeepClone();

				joInfo.TryRemoveProperty("Password");
				joInfo.TryRemoveProperty("FilesToDo");
				joInfo.TryRemoveProperty("ProgressState");

				backgroundWorker.QueueBackgroundWorkItem(GenItemKey(joNode), joInfo, async token =>
				{
					await ExecDeployToNode(backgroundWorker, nodeIndex, joNode);
				});
			}
			return true;
		}
		private static Task ExecDeployToNode(AppEndBackgroundWorkerQueue backgroundWorker, int nodeIndex, JObject joNode)
		{
			try
			{
				string remotePath = joNode["RemotePath"].ToStringEmpty();
				Session session = CreateSftpSession(joNode);
				TransferOptions transferOptions = new() { FileMask = "| deploy*; DynaAsm*" };
				var r = session.SynchronizeDirectories(SynchronizationMode.Remote, HostingUtils.GetHostRootDirectory().FullName, remotePath, false, options: transferOptions);
			}
			catch (Exception ex)
			{
				StaticMethods.LogImmed(ex.Message, "log", "", "deploy_");
			}
			finally
			{
				ChangeNodeLastDeployToNow(nodeIndex);
				AppEndBackgroundWorkerQueue.UnRegisterTask();
			}
			return Task.CompletedTask;
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

		public static JArray GetNodeToDoItems(JObject jn)
		{
			JArray arr = HostingUtils.GetHostRootDirectory().GetFilesRecursiveWithInfo();
			JArray list = [];
			string dtStr = jn["LastDeploy"].ToStringEmpty();
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
			int ind = 0;
			foreach(var node in arr)
			{
				node["FilesToDo"] = GetNodeToDoItems((JObject)node);
				node["ProgressState"] = AppEndBackgroundWorkerQueue.QueueState(GenItemKey((JObject)node)); 
				ind++;
			}
			return arr;
		}
		public static void RemoveNode(string ind)
		{
			var nodes = GetNodes();
			JObject? jn = (JObject)nodes[ind.ToIntSafe()];
			if (jn != null) nodes.Remove(jn);
			HostingUtils.GetHostRootDirectory().Delete("deploy_" + ind + "_*");
			WriteNodes(nodes);
		}
		public static void CreateAlterNode(int ind, string name, string ip, string port, string remotePath, string userName, string password)
		{
			JArray nodes = GetNodes();
			if (ind == -1)
			{
				JObject jn = new()
				{
					["Name"] = name,
					["Ip"] = ip,
					["Port"] = port,
					["RemotePath"] = remotePath,
					["UserName"] = userName,
					["Password"] = password
				};
				nodes.Add(jn);
			}
			else
			{
				nodes[ind]["Name"] = name;
				nodes[ind]["Ip"] = ip;
				nodes[ind]["Port"] = port;
				nodes[ind]["RemotePath"] = remotePath;
				nodes[ind]["UserName"] = userName;
				nodes[ind]["Password"] = password;
			}
			WriteNodes(nodes);
		}
		public static void ChangeNodeLastDeployToNow(int ind)
		{
			Thread.Sleep(20);
			JArray nodes = GetNodes();
			if (nodes[ind.ToIntSafe()] != null)
			{
				((JObject)nodes[ind.ToIntSafe()])["LastDeploy"] = DateTime.Now.ToString();
				WriteNodes(nodes);
			}
			Thread.Sleep(20);
		}

		private static void WriteNodes(JArray nodes)
		{
			foreach (var node in nodes)
			{
				((JObject)node).TryRemoveProperty("ProgressState");
			}
			File.WriteAllText(DeployNodesFileName, nodes.ToJsonStringByNewtonsoft());
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
			if (fp.ContainsIgnoreCase("appsettings")) return true;
			return false;
		}
		private static string DeployNodesFileName
		{
			get 
			{
				return HostingUtils.GetHostRootDirectory().FullName + "/deploy_nodes.json";
			}
		}
		private static string GenItemKey(JObject joNode)
		{
			return $"DeployTo[{joNode["Name"].ToStringEmpty()}]";
		}
	}
}

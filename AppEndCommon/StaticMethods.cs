using System.Net.Sockets;
using System.Net;

namespace AppEndCommon
{
	public static class StaticMethods
    {
        
        public static string GetUniqueName(string prefix = "param")
        {
            return prefix + Guid.NewGuid().ToString().Replace("-", "");
        }
        public static string GetRandomName(string prefix = "param")
        {
            return $"{prefix}{(new Random()).Next(100)}";
        }

		public static void LogImmed(string content, string logFolder = "log", string subFolder = "", string filePreFix = "")
		{
			string fn = $"{filePreFix}{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}-{DateTime.Now.Millisecond}-{+(new Random()).Next(100)}.txt";
			File.WriteAllText(Path.Combine($"{logFolder}{(subFolder == "" ? "" : $"/{subFolder}")}", fn), content);
		}

		public static string GetServerIP()
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

			foreach (IPAddress address in ipHostInfo.AddressList)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
					return address.ToString();
			}

			return string.Empty;
		}

	}
}
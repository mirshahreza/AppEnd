using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Text;

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
			string fn = $"{filePreFix}{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}-{DateTime.Now.Millisecond}-{DateTime.Now.Nanosecond}-{+(new Random()).Next(100)}.txt";
			File.WriteAllText(Path.Combine($"{logFolder}{(subFolder == "" ? "" : $"/{subFolder}")}", fn), content);
		}

		public static string GetServerIP()
		{
			IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

			foreach (IPAddress address in ipHostInfo.AddressList)
				if (address.AddressFamily == AddressFamily.InterNetwork)
					return address.ToString();

			return string.Empty;
		}

		public static void RESTPostSync(string Url, string Content, Hashtable Headers, out string StatusCode, out string ResponseContent)
		{
			using HttpClient client = new HttpClient();
			var content = new StringContent(Content, Encoding.UTF8, "application/json");
			if (Headers != null && Headers.Count > 0)
				foreach (string k in Headers.Keys)
					client.DefaultRequestHeaders.TryAddWithoutValidation(k, Headers[k]?.ToString());
			var result = client.PostAsync(Url, content).Result;
			StatusCode = result.StatusCode.ToString();
			ResponseContent = result.Content.ReadAsStringAsync().Result;
		}

		public static async Task<Tuple<string, string>> RESTPostAsync(string Url, string Content, Hashtable Headers)
		{
			using HttpClient client = new HttpClient();
			var content = new StringContent(Content, Encoding.UTF8, "application/json");
			if (Headers != null && Headers.Count > 0)
				foreach (string k in Headers.Keys) 
					client.DefaultRequestHeaders.TryAddWithoutValidation(k, Headers[k]?.ToString());
			var result = await client.PostAsync(Url, content);
			return new Tuple<string, string>(result.StatusCode.ToString(), result.Content.ReadAsStringAsync().Result);
		}



		public static Tuple<string?, string, string> MethodPartsNames(string methodFullPath)
		{
			if (methodFullPath.Trim() == "") throw new AppEndException("MethodFullPathCanNotBeEmpty", System.Reflection.MethodBase.GetCurrentMethod())
							.GetEx();
			string[] parts = methodFullPath.Trim().Split('.');
			if (parts.Length < 2 || parts.Length > 3) throw new AppEndException($"MethodMustContainsAtLeast2PartsSeparatedByDot", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("MethodFullPath", methodFullPath)
					.GetEx();
			return parts.Length == 3 ? new(parts[0], parts[1], parts[2]) : new(null, parts[0], parts[1]);
		}


	}
}
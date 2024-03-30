using AppEndCommon;
using AppEndDynaCode;
using Newtonsoft.Json.Linq;

namespace AppEndServer
{
	public static class AppEndEventLogger
	{
		private static List<JObject> _events = [];

		public static void Add(JObject eventContent)
		{
			_events.Add(eventContent);
			if (_events.Count > AppEndSettings.LogWriterQueueCap) SatrtWriting();
		}

		public static void SatrtWriting()
		{
			DynaCode.InvokeByJsonInputs("Zzz.AppEndProxy.AppEndStartWritingLogItems");
		}

		public static List<JObject> GetAndCleanupEvents()
		{
			List<JObject> events = _events;
			_events = [];
			return events;
		}

	}
}

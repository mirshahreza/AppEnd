using AppEndCommon;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AppEndDbIO
{
    public class DbConf
    {
        public string Name { set; get; }
        public ServerType ServerType { set; get; }
        public string ConnectionString { set; get; }

        public DbConf(string dbConfName, ServerType serverType, string connectionString)
        {
            Name = dbConfName;
            ServerType = serverType;
            ConnectionString = connectionString;
        }

        [JsonConstructor]
        public DbConf() { }
        public void Save()
        {
            JsonNode? jsonNode = AppEndSettings.DbServers.FirstOrDefault(i => i?[nameof(Name)]?.ToString() == this.Name);
            if (jsonNode == null)
            {
                string dbInfoText = JsonSerializer.Serialize(this, options: new()
                {
                    WriteIndented = true
                });
                jsonNode = JsonNode.Parse(dbInfoText);
            }
            else
            {
                int i = AppEndSettings.DbServers.IndexOf(jsonNode);
                AppEndSettings.DbServers.RemoveAt(i);
                string dbInfoText = JsonSerializer.Serialize(this, options: new()
                {
                    WriteIndented = true
                });
                jsonNode = JsonNode.Parse(dbInfoText);
            }
            AppEndSettings.DbServers.Add(jsonNode);
            string appSettingsText = JsonSerializer.Serialize(AppEndSettings.DbServers.Parent?.Parent, options: new()
            {
                WriteIndented = true
            });
            File.WriteAllText("appsettings.json", appSettingsText);
            AppEndSettings.RefereshSettings();
        }
        public static void Remove(string DbServerName)
        {
            JsonNode? jsonNode = AppEndSettings.DbServers.FirstOrDefault(i => i?[nameof(Name)]?.ToString() == DbServerName);
            if (jsonNode != null)
            {
                int i = AppEndSettings.DbServers.IndexOf(jsonNode);
                AppEndSettings.DbServers.RemoveAt(i);
                string appSettingsText = JsonSerializer.Serialize(AppEndSettings.DbServers.Parent?.Parent, options: new()
                {
                    WriteIndented = true
                });
                File.WriteAllText("appsettings.json", appSettingsText);
                AppEndSettings.RefereshSettings();
            }
        }
        public static DbConf FromSettings(string dbConfName)
        {
            JsonNode? jsonNode = AppEndSettings.DbServers.FirstOrDefault(i => i?[nameof(Name)].ToStringEmpty() == dbConfName) ?? throw new AppEndException("DbConfNameIsNotExist")
                    .AddParam("DbConfName", dbConfName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			DbConf? dbInfo = jsonNode.Deserialize<DbConf>();
			return dbInfo ?? throw new AppEndException("DeserializationError")
					.AddParam("DbConfName", dbConfName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
		}
	}
}

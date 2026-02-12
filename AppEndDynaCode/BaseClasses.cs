using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppEndDynaCode
{
    // Base DTOs, enums and records used across DynaCode partials
    // Organized by concern without changing public surface
    public class MethodSettings
    {
        public AccessRules AccessRules = new([], [], []);
        public CachePolicy CachePolicy = new() { };
        public LogPolicy LogPolicy = LogPolicy.TrimInputs;
        public LongRunningPolicy? LongRunningPolicy;
        public string Serialize()
        {
            return JsonSerializer.Serialize(this, options: new()
            {
                IncludeFields = true,
                WriteIndented = false,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            });
        }
    }

    // Logging
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LogPolicy
    {
        IgnoreLogging,
        TrimInputs,
        Full
    }

    // Caching
    public record CachePolicy
    {
        public CacheLevel CacheLevel = CacheLevel.None;
        public int AbsoluteExpirationSeconds;
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CacheLevel
    {
        None,
        PerUser,
        AllUsers
    }

    // Long-Running
    public record LongRunningPolicy
    {
        public int TimeoutSeconds = 300;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum LongRunningTaskStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled
    }

    public class LongRunningTaskInfo
    {
        public string TaskToken { get; set; } = Guid.NewGuid().ToString("N");
        public string MethodFullName { get; set; } = "";
        public LongRunningTaskStatus Status { get; set; } = LongRunningTaskStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public object? Result { get; set; }
        public string? Error { get; set; }
        public long DurationMs { get; set; }
        public bool FromCache { get; set; }
        public string CreatedBy { get; set; } = "";
    }

    // Access rules
    public record AccessRules(string[] AllowedRoles, string[] AllowedUsers, string[] DeniedUsers)
    {
        public string[] AllowedRoles { set; get; } = AllowedRoles;
        public string[] AllowedUsers { set; get; } = AllowedUsers;
        public string[] DeniedUsers { set; get; } = DeniedUsers;
    }

    // Mapping
    public record CodeMap(string MethodFullName, string FilePath)
    {
        public string MethodFullName { get; init; } = MethodFullName;
        public string FilePath { get; init; } = FilePath;
    }

    public record SourceCode(string FilePath, string RawCode)
    {
        public string FilePath { get; init; } = FilePath;
        public string RawCode { get; init; } = RawCode;
    }

    // Invocation
    public record CodeInvokeResult
    {
        public long Duration { get; init; }
        public object? Result { get; init; }
        public bool IsSucceeded { get; init; } = false;
        public bool FromCache { get; init; } = false;
        public string? TaskToken { get; init; }
        public bool IsLongRunning { get; init; } = false;
    }

    public record CodeInvokeOptions(string StartPath)
    {
        public string StartPath { get; init; } = StartPath;
        public bool CompiledIn { get; init; } = false;
        public bool IsDevelopment { get; init; } = false;
        public string PublicKeyUser { get; init; } = "";
        public string PublicKeyRole { get; init; } = "";
        public string[]? PublicMethods { get; init; }
        public string AlternativeMethodFullName { get; init; } = "";
        public string ReferencesPath { get; init; } = "";
    }

    // Introspection outputs
    public record DynaClass(string Name, List<DynaMethod> DynaMethods)
    {
        public string? Namespace { set; get; }
        public string Name { set; get; } = Name;
        public List<DynaMethod> DynaMethods { set; get; } = DynaMethods;
    }

    public record DynaMethod(string Name, MethodSettings MethodSettings)
    {
        public string Name { set; get; } = Name;
        public MethodSettings MethodSettings { set; get; } = MethodSettings;
    }
}

using System.Reflection;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

namespace AppEndCommon
{
    public static class ExtensionsForJson
    {
        public static T? TryDeserializeTo<T>(string jsonString, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, options: jsonSerializerOptions);
            }
            catch
            {
                return default;
            }
        }
        public static T? TryDeserializeTo<T>(JsonElement jsonElement, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonElement, options: jsonSerializerOptions);
            }
            catch
            {
                return default;
            }
        }
        public static T? TryDeserializeTo<T>(JsonObject jsonObject)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(jsonObject);
            }
            catch
            {
                return default;
            }
        }
        public static string ToStringEmpty(this JToken? jToken)
        {
            if (jToken == null) return "";
            return jToken.ToString();
        }
        public static string ToStringEmpty(this JValue? jValue)
        {
            if (jValue == null) return "";
            return jValue.ToString();
        }

       
        public static JArray ToJArray(this JToken? jToken)
        {
            if (jToken is null) return [];
            if (jToken is not JArray) new AppEndException("InputParameterIsNotJArray", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Input", jToken)
                    .GetEx();
            return (JArray)jToken;
        }

        public static JObject ToJObject(this JToken? jToken)
        {
            if (jToken == null) return [];
            if (jToken is not JObject) new AppEndException("InputParameterIsNotJObject", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Input", jToken)
                    .GetEx();
            return (JObject)jToken;
        }

        public static string FixNullToString(this JToken? jToken, string ifNull)
        {
            if (jToken == null) return ifNull;
            if (jToken is JValue value && value.Value == null) return ifNull;
            return ((JValue)jToken).Value.ToStringEmpty();
        }
        public static string? FixNullToNullableString(this JToken? jToken, string? ifNull)
        {
            if (jToken == null) return ifNull;
            if (jToken is JValue value && value.Value == null) return ifNull;
            return ((JValue)jToken).Value.ToStringEmpty();
        }
        public static int? FixNullToNullableInt(this JToken? jToken, int? ifNull)
        {
            if (jToken == null) return ifNull;
            if (jToken is JValue value && value.Value == null) return ifNull;
            return int.Parse(((JValue)jToken).Value.ToStringEmpty());
        }
        public static int FixNullToInt(this JToken? jToken)
        {
            if (jToken is null) return -1;
            if (jToken is JValue value && value.Value == null) return -1;
            return jToken.ToIntSafe();
        }

        public static bool FixNullToBool(this JToken? jToken, bool ifNull)
        {
            if (jToken == null) return ifNull;
            if (jToken is JValue value && value.Value == null) return ifNull;
            return bool.Parse(((JValue)jToken).Value.ToStringEmpty());
        }

		public static string ToJsonStringByNewtonsoft(this object? o, bool indented = true)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(o, indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
		}
		public static JObject ToJObjectByNewtonsoft(this string s)
        {
            return JObject.Parse(s);
        }
        public static JArray ToJArrayByNewtonsoft(this string s)
        {
            return JArray.Parse(s);
        }
        public static string ToJsonStringByBuiltIn(this object? o, bool indented = true, bool includeFields = true, JsonIgnoreCondition ignorePolicy = JsonIgnoreCondition.WhenWritingDefault)
        {
            return JsonSerializer.Serialize(o, options: new()
            {
                IncludeFields = includeFields,
                WriteIndented = indented,
                DefaultIgnoreCondition = ignorePolicy,
                IgnoreReadOnlyProperties = true
            });
        }
        public static string ToJsonStringByBuiltInAllDefaults(this object? o)
        {
            return JsonSerializer.Serialize(o);
        }
		public static JsonElement ToJsonElementByNewton(this object o)
		{
			return JsonSerializer.Deserialize<JsonElement>(o.ToJsonStringByNewtonsoft());
		}
		public static JsonElement ToJsonElementByBuiltIn(this object o)
        {
            return JsonSerializer.Deserialize<JsonElement>(o.ToJsonStringByBuiltIn(false));
        }

        public static string[]? DeserializeAsStringArray(this string? o)
        {
            if (o is null) return null;
            return JsonSerializer.Deserialize<string[]>(o);
        }

        public static object ToOrigType(this JsonElement s, ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType == typeof(int)) return int.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(Int16)) return Int16.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(Int32)) return Int32.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(Int64)) return Int64.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(bool)) return bool.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(DateTime)) return DateTime.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(Guid)) return Guid.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(float)) return float.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(Decimal)) return Decimal.Parse(s.ToString());
            if (parameterInfo.ParameterType == typeof(string)) return s.ToString();
            if (parameterInfo.ParameterType == typeof(byte[])) return Encoding.UTF8.GetBytes(s.ToString());
            if (parameterInfo.ParameterType == typeof(List<string>)) return s.Deserialize<List<string>>().FixNull(new List<string>());


            return s;
        }

        public static void TryRemoveProperty(this JObject jo,string propertyName)
        {
			var p = jo.Properties().FirstOrDefault(i => i.Name == propertyName);
			p?.Remove();
		}


        public static JsonElement ShrinkJsonElement(this JsonElement element, int maxLen = 128)
        {
            // Write a modified JSON into a memory stream using Utf8JsonWriter, then parse back to JsonElement.
            using var ms = new MemoryStream();
            using (var writer = new Utf8JsonWriter(ms))
            {
                WriteShrunk(element, writer, maxLen);
                writer.Flush();
            }
            ms.Position = 0;
            using var doc = JsonDocument.Parse(ms);
            // Clone the root element so it isn't tied to the JsonDocument's lifetime
            return doc.RootElement.Clone();
        }

        public static void WriteShrunk(JsonElement el, Utf8JsonWriter writer, int maxLen)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    writer.WriteStartObject();
                    foreach (var prop in el.EnumerateObject())
                    {
                        writer.WritePropertyName(prop.Name);
                        WriteShrunk(prop.Value, writer, maxLen);
                    }
                    writer.WriteEndObject();
                    break;

                case JsonValueKind.Array:
                    writer.WriteStartArray();
                    foreach (var item in el.EnumerateArray())
                    {
                        WriteShrunk(item, writer, maxLen);
                    }
                    writer.WriteEndArray();
                    break;

                case JsonValueKind.String:
                    {
                        var s = el.GetString() ?? string.Empty;
                        if (s.Length > maxLen)
                            writer.WriteStringValue(": large...");
                        else
                            writer.WriteStringValue(s);
                    }
                    break;

                case JsonValueKind.Number:
                    // Preserve numeric formatting by writing raw text
                    writer.WriteRawValue(el.GetRawText());
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    writer.WriteBooleanValue(el.GetBoolean());
                    break;

                case JsonValueKind.Null:
                    writer.WriteNullValue();
                    break;

                default:
                    // Fallback: write raw text
                    writer.WriteRawValue(el.GetRawText());
                    break;
            }
        }
    }
}
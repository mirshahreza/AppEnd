using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace AppEndCommon
{
    public static partial class ExtensionsForString
    {
        private static readonly ConcurrentDictionary<string, List<string>> _sqlParamsCache = new();

        public static byte[] ToByteArray(this string fileString)
        {
			return Convert.FromBase64String(fileString);
		}

		public static string TruncateTo(this string value, int maxLength)
        {
            if(value == null) throw new ArgumentNullException("value");
            if(value.Length <= maxLength) return value;
            return value.Substring(maxLength);
        }

        public static bool PathIsManagable(this string path)
        {
            if (path.StartsWithIgnoreCase(".")) return false;
            if (path.StartsWithIgnoreCase("/properties")) return false;
            if (path.StartsWithIgnoreCase("/bin")) return false;
            if (path.StartsWithIgnoreCase("/obj")) return false;
            if (path.StartsWithIgnoreCase("/.config")) return false;
            if (path.ContainsIgnoreCase("DynaAsm")) return false;
            if (path.ContainsIgnoreCase(".csproj")) return false;
            if (path.ContainsIgnoreCase(".Development.")) return false;
            return true;
        }

        public static string NormalizeAsHostPath(this string path, bool removeBasePath = true)
        {
            string s = path.StartsWith("/") ? path[1..] : path;
            s = s.Replace("\\", "/");
            s = s.Replace("//", "/");
            s = s.Replace("//", "/");
            s = removeBasePath == true ? s.Replace(AppEndSettings.ProjectRoot.FullName.NormalizeAsHostPath(false), "") : s;
            s = s.StartsWith("/") ? s[1..] : s;
            return s;
        }

        public static Tuple<int, int> ToRangeMinValue(this string? s)
        {
            if (s is null || s.Trim() == "") return new Tuple<int, int>(1, 100);
            string[] parts = s.Split("(");
            if (parts.Length < 2) return new Tuple<int, int>(1, 100);
            parts = parts[1].Replace(")", "").Split(",");
            int min = parts[0].ToIntSafe(1);
            int max = parts.Length > 1 ? parts[1].ToIntSafe(1) : 100;
            return Tuple.Create(min, max);
		}
		public static bool StartsWithIgnoreCase(this string? s, string? testString)
		{
			if (s is null || testString is null) return false;
			if (s is null || s == "" || testString is null || testString == "") return false;
			return s.StartsWith(testString, StringComparison.CurrentCultureIgnoreCase);
		}
        public static bool EndsWithIgnoreCase(this string? s, string? testString)
        {
            if (s is null || testString is null) return false;
            if (s is null || s == "" || testString is null || testString == "") return false;
            return s.EndsWith(testString, StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool EndsWithIgnoreCase(this string? s, List<string> testStringList)
        {
            if (s is null || s == "" || testStringList is null || testStringList.Count == 0) return false;
            foreach (var item in testStringList)
            {
                if (s.EndsWith(item, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }
        public static bool EqualsIgnoreCase(this string? s, string? testString)
		{
			if (s is null || testString is null) return false;
			if (s is null || s == "" || testString is null || testString == "") return false;
            return s.Equals(testString, StringComparison.CurrentCultureIgnoreCase);
		}

		public static bool ContainsIgnoreCase(this string? s, string? testString)
		{
			if (s is null || s == "" || testString is null || testString == "") return false;
			if (s.Contains(testString, StringComparison.CurrentCultureIgnoreCase)) return true;
			return false;
		}
		public static bool ContainsIgnoreCase(this string? s, List<string> testStringList)
		{
			if (s is null || s == "" || testStringList is null || testStringList.Count == 0) return false;
			foreach (var item in testStringList)
			{
				if (s.Contains(item, StringComparison.CurrentCultureIgnoreCase)) return true;
			}
			return false;
		}

		public static string ReplaceSafe(this string? s, string? v1, string v2)
        {
            if (s is null || s == "") return "";
            if (v1 is null || v1 == "") return s;
            return s.Replace(v1, v2);
        }
        public static string BeginingCommonPart(this string? s1, string s2)
        {
            if (s1 is null || s2 is null) return "";
            char[] c1 = s1.ToCharArray();
            char[] c2 = s2.ToCharArray();
            List<char> result = [];

            int minLen = c1.Length < c2.Length ? c1.Length : c2.Length;

            int ind = 0;
            while (ind < minLen)
            {
                if (c1[ind] == c2[ind]) result.Add(c1[ind]);
                else break;
                ind++;
            }

            return string.Join("", result.ToArray());
        }
        public static string FixNull(this string? s, string alternate)
        {
            if (s == null) return alternate;
            return s;
        }
        public static bool IsNullOrEmpty(this string? s)
        {
            if (s == null || s.Trim() == "") return true;
            return false;
        }
        public static string FixNullOrEmpty(this string? s, string alternate)
        {
            if (s == null || s.Trim() == "") return alternate;
            return s;
        }
        public static string RepeatN(this string s, int n)
        {
            string temp = "";
            for (int i = 0; i < n; i++)
            {
                temp += s;
            }
            return temp;
        }
        public static string ReplaceLastOccurrence(this string s, string find, string replace)
        {
            int place = s.LastIndexOf(find);
            if (place == -1) return s;
            string result = s.Remove(place, find.Length).Insert(place, replace);
            return result;
        }

        public static string RemoveUnNecessaryEmptyLines(this string s)
        {
            s = s.Replace(RepeatN(SV.NL, 4), SV.NL);
            s = s.Replace(RepeatN(SV.NL, 3), SV.NL);
            s = s.Replace(RepeatN(SV.NL, 2), SV.NL);
            return s;
        }

        public static void ValidateStringNotNullOrEmpty(this string s, string paramName)
        {
            if (s == null || s.Trim() == "") new AppEndException($"CanNotBeNullOrEmpty", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ParamName", paramName)
                    .GetEx();
        }

        public static string RemoveWhitelines(this string s)
        {
            return WhitelinesRegex().Replace(s, string.Empty);
        }


		public static List<string> ExtractSqlParameters(this string sql)
		{
            if (string.IsNullOrEmpty(sql)) return [];
            if (_sqlParamsCache.TryGetValue(sql, out var cached)) return cached;
            var list = SqlParamsRegex().Matches(sql).Select(i => i.Value.Replace("@", "")).ToList().Distinct().ToList();
            _sqlParamsCache[sql] = list;
            return list;
		}

        public static string BeautifySql(this string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return "";

            // Normalize whitespace
            string s = Regex.Replace(sql, @"\r\n|\r|\n", " ");
            s = Regex.Replace(s, @"\s+", " ");
            s = s.Trim();

            // Uppercase common keywords
            string[] keywords = new[]
            {
                "select","from","where","group by","order by","inner join","left join","right join","full join","join","on","and","or",
                "insert into","values","update","set","delete","delete from","having","union","union all","exists","not exists",
                "top","distinct","case","when","then","else","end","in","like","between","is null","is not null"
            };
            foreach (var kw in keywords.OrderByDescending(k => k.Length))
            {
                // word-boundary-ish replacement, case-insensitive
                s = Regex.Replace(s, $"(?i)(?<![A-Z_]){Regex.Escape(kw)}(?![A-Z_])", kw.ToUpper());
            }

            // Line breaks before primary clauses
            string[] breakBefore = new[] { " SELECT ", " FROM ", " WHERE ", " GROUP BY ", " ORDER BY ", " HAVING ", " UNION ", " UNION ALL ", " INSERT INTO ", " UPDATE ", " DELETE FROM " };
            foreach (var bb in breakBefore)
            {
                s = s.Replace(bb, SV.NL + bb.Trim() + " ");
            }

            // Line breaks for JOIN and ON
            s = Regex.Replace(s, " (?i)JOIN ", SV.NL + "JOIN ");
            s = Regex.Replace(s, " (?i)INNER JOIN ", SV.NL + "INNER JOIN ");
            s = Regex.Replace(s, " (?i)LEFT JOIN ", SV.NL + "LEFT JOIN ");
            s = Regex.Replace(s, " (?i)RIGHT JOIN ", SV.NL + "RIGHT JOIN ");
            s = Regex.Replace(s, " (?i)FULL JOIN ", SV.NL + "FULL JOIN ");
            s = Regex.Replace(s, " (?i) ON ", SV.NL + "ON ");

            // Newline after commas in select lists and SET clauses
            s = Regex.Replace(s, @"\s*,\s*", "," + SV.NL + "    ");

            // Indent AND/OR in WHERE
            s = Regex.Replace(s, $"{SV.NL}(?i)AND ", SV.NL + "    AND ");
            s = Regex.Replace(s, $"{SV.NL}(?i)OR ", SV.NL + "    OR ");

            // Collapse extra newlines
            s = s.Replace(RepeatN(SV.NL, 3), SV.NL);
            s = s.Replace(RepeatN(SV.NL, 2), SV.NL);
            // Remove whitespace-only lines
            s = s.RemoveWhitelines();

            s = s.Trim();

            return s;
        }


		[GeneratedRegex(@"^\s+$[\r\n]*", RegexOptions.Multiline)]
		public static partial Regex WhitelinesRegex();

		[GeneratedRegex(@"shared.translate\(.*?\)", RegexOptions.Multiline)]
		public static partial Regex JsTranslationRegex();

		[GeneratedRegex(@"(\?|\@\w+)", RegexOptions.Multiline)]
		public static partial Regex SqlParamsRegex();

	}
}
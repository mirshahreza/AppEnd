namespace AppEndCommon
{
	public static class ExtensionsForList
    {
		public static void AddIfNotContains(this List<string> list, string name)
		{
			if (!list.ContainsIgnoreCase(name)) list.Add(name);
		}
		public static void AddSafe(this List<string> list, string? name)
		{
			if (name is not null) list.Add(name);
		}

		public static bool ContainsIgnoreCase(this List<string>? list, string? testString)
		{
			if (list is null) return false;
			if (testString is null || testString == "") return false;
			foreach (string str in list) if (str.ToLower() == testString.ToLower()) return true;
			return false;
		}

		public static bool HasIntersect(this List<string> list, string[]? testArr)
		{
			if (testArr is null || testArr.Length == 0) return false;
			foreach (string str in list) if (testArr.ContainsIgnoreCase(str)) return true;
			return false;
		}

	}
}
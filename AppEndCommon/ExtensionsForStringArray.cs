namespace AppEndCommon
{
	public static class ExtensionsForStringArray
	{
		public static bool ContainsIgnoreCase(this string[]? arr, string? testString)
		{
			if (arr is null || arr.Length == 0 || testString is null || testString == "") return false;
			foreach (string str in arr)
			{
				if (str.ToLower() == testString.ToLower()) return true;
			}
			return false;
		}

		public static bool HasIntersect(this string[] arr, string[]? testArr)
		{
			if (testArr is null || testArr.Length == 0) return false;
			foreach (string str in arr)
			{
				if (testArr.ContainsIgnoreCase(str)) return true;
			}
			return false;
		}

	}
}
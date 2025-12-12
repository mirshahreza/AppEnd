namespace AppEndCommon
{
    public static class ExtensionsForObject
    {
        public static string ToStringEmpty(this object? o)
        {
            if (o is null) return "";
            return o.ToString();
        }
        public static int ToIntSafe(this object? o, int ifHasProblem = -1)
        {
            string i = o.ToStringEmpty();
            if (i.IsNullOrEmpty()) return ifHasProblem;
            int ii;
            if (int.TryParse(i, out ii)) return ii;
            return ifHasProblem;
        }
        public static int? ToIntSafeNull(this object? o)
        {
            string i = o.ToStringEmpty();
            if (i.IsNullOrEmpty()) return null;
            int ii;
            if (int.TryParse(i, out ii)) return ii;
            return null;
        }
        public static DateTime ToDateTimeSafe(this object? o, DateTime? ifHasProblem)
		{
			string i = o.ToStringEmpty();
			DateTime ii;
			if (DateTime.TryParse(i, out ii)) return ii;
			if (ifHasProblem is null) return DateTime.Now;
			else return (DateTime)ifHasProblem;
		}

		public static bool ToBooleanSafe(this object? o, bool ifHasProblem = false)
		{
			string i = o.ToStringEmpty();
			if (i.IsNullOrEmpty()) return ifHasProblem;
			if (bool.TryParse(i, out bool ii)) return ii;
			return ifHasProblem;
		}
		public static int To01Safe(this bool o)
		{
			if(o) return 1;
			return 0;
		}

		public static object FixNull(this object? o, object ifNull)
        {
            if (o is null && ifNull is null) throw new ArgumentNullException(nameof(ifNull));
            if (o is null) return ifNull;
            else return o;
        }

        public static string? ToHumanReadible(this object? o)
        {
            if (o == null) return null;
            if (o is Exception ex)
            {
                return ex.Data?.Count > 0 ? string.Join("; ", ex.Data.Cast<System.Collections.DictionaryEntry>().Select(de => $"{de.Key}: {de.Value}")) : ex.Message;
            }

            return o.ToString();
        }

    }
}
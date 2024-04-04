using System.Data;

namespace AppEndCommon
{
	public static class ExtensionsForDataTable
	{
		public static void ToCSV(this DataTable dt, string tempDirPath, List<string>? exceptColumns = null, Dictionary<string, string>? columnTitles = null)
		{
			string tn = Guid.NewGuid().ToString().Replace("-", "");
			string fp = $"{tempDirPath}/{tn}.csv";
			StreamWriter sw = new(fp, false);

			// Remove byte[] fields
			dt.RemoveByteArrayColumns();

			// Remove exceptColumns
			dt.RemoveColumns(exceptColumns);

			// Write CSV header
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (columnTitles is not null && columnTitles.TryGetValue(dt.Columns[i].ColumnName, out string? value)) sw.Write(value.Replace(",", "_").Replace(SV.NL, ""));
				else sw.Write(dt.Columns[i].ColumnName);
				if (i < dt.Columns.Count - 1) sw.Write(",");
			}
			sw.Write(sw.NewLine);
			// Write CSV content
			foreach (DataRow dr in dt.Rows)
			{
				for (int i = 0; i < dt.Columns.Count; i++)
				{
					if (!Convert.IsDBNull(dr[i]))
					{
						string value = dr[i].ToStringEmpty();
						if (value.Contains(',')) sw.Write(String.Format("\"{0}\"", value.Replace(SV.NL, "")));
						else sw.Write(value.Replace(SV.NL, ""));
					}
					if (i < dt.Columns.Count - 1) sw.Write(",");
				}
				sw.Write(sw.NewLine);
			}
			sw.Close();
		}

		public static void RemoveColumns(this DataTable dt, List<string>? exceptColumns = null)
		{
			if (exceptColumns == null) return;
			foreach (string c in exceptColumns) if (dt.Columns.Contains(c)) dt.Columns.Remove(c);
		}

		public static void RemoveByteArrayColumns(this DataTable dt)
		{
			List<string> exceptColumns = [];
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (dt.Columns[i].DataType == typeof(byte[]))
				{
					exceptColumns.Add(dt.Columns[i].ColumnName);
				}
			}
			foreach (string c in exceptColumns) dt.Columns.Remove(c);
		}

	}
}
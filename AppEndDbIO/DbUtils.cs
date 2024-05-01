using AppEndCommon;

namespace AppEndDbIO
{
	public static class DbUtils
    {
        
        public static bool ColumnIsForDisplay(this DbColumn dbColumn)
        {
            if ((dbColumn.Name.ContainsIgnoreCase("Name") || dbColumn.Name.ContainsIgnoreCase("Title")) && !dbColumn.Name.ContainsIgnoreCase("File")) return true;
            return false;
        }
        public static bool ColumnIsForReadByKey(this DbColumn dbColumn)
        {
            // todo : implemention required
            return true;
        }
        public static bool ColumnIsForReadList(this DbColumn dbColumn)
        {
            if (dbColumn.Name.EndsWith("_FileBody")) return false;
            if (dbColumn.Name.EndsWith("_FileSize")) return false;
            if (dbColumn.Name.EndsWith("_FileName")) return false;
			if (dbColumn.Name.EndsWith("_FileMime")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("password")) return false;
			//if (dbColumn.IsNumerical()) return false;
            return true;
        }
        public static bool ColumnIsForAggregatedReadList(this DbColumn dbColumn)
        {
            if (dbColumn.IsPrimaryKey) return false;
            if (dbColumn.Name.ContainsIgnoreCase("_File")) return false;
            if (dbColumn.Name.ContainsIgnoreCase("Name")) return false;
            if (dbColumn.Name.ContainsIgnoreCase("Title")) return false;
            if (dbColumn.Name.EndsWith("On")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("password")) return false;
			if (!dbColumn.IsNumerical() && (dbColumn.Size is not null && int.Parse(dbColumn.Size) > 256)) return false;
            return true;
        }
        public static bool ColumnIsForDelete(this DbColumn dbColumn)
        {
			if (dbColumn.Name.ToLower().EndsWith("_xs")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("xml")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("html")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("file")) return false;
			if (dbColumn.Name.ContainsIgnoreCase("password")) return false;
            return true;
        }
        public static bool ColumnIsForCreate(this DbColumn dbColumn)
        {
            if (dbColumn.IsIdentity || dbColumn.DbDefault != null) return false;
            if (dbColumn.Name == "UpdatedBy" || dbColumn.Name == "UpdatedOn") return false;
			if (dbColumn.Name.ContainsIgnoreCase("password")) return false;
			return true;
        }
        public static bool ColumnIsForUpdateByKey(this DbColumn dbColumn)
        {
            if (dbColumn.Name == "CreatedBy" || dbColumn.Name == "CreatedOn") return false;
			if (dbColumn.Name.ContainsIgnoreCase("password")) return false;
			return true;
        }

        public static string GenParamName(string objectName, string columnName, int? index = null)
        {
            return $"{objectName}_{columnName}" + (index is null ? "" : $"_{index}");
        }

        public static string GetSetColumnParamPair(string source, string columnName,int? index)
        {
            return $"[{source}].[{columnName}]=@{DbUtils.GenParamName(source, columnName, index)}";
        }

        public static string GetTypeSize(string dbType, object? size)
        {
            string dbT = dbType.ToUpper();
            if (size == null) return dbT;
            return $"{dbT}({size})";
        }

        public static bool ColumnsAreFileCentric(List<DbColumn> dbColumns)
        {
            DbColumn? dbColumn = dbColumns.FirstOrDefault(i => i.DbType.EqualsIgnoreCase("IMAGE") || i.DbType.EqualsIgnoreCase("BINARY"));
            if (dbColumn is not null && dbColumns.Count < 8) return true;
            return false;
        }

		public static List<DbColumn> RemoveAuditingColumns(this List<DbColumn> dbColumns)
		{
			return dbColumns.Where(i => !i.Name.EqualsIgnoreCase("createdby") && !i.Name.EqualsIgnoreCase("createdon") && !i.Name.EqualsIgnoreCase("updatedby") && !i.Name.EqualsIgnoreCase("updatedon")).ToList();
		}

	}
}

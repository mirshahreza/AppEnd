using AppEndCommon;
using Microsoft.Extensions.Caching.Memory;
using System.Security.AccessControl;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppEndDbIO
{
	public class DbDialog 
    {
		private string? _dbDialogsRoot;
		public string DevNote { set; get; } = "";
		public string DbConfName { set; get; } = "";
		public string ObjectName { set; get; }
        public DbObjectType ObjectType { set; get; } = DbObjectType.Table;

		public OpenningPlace OpenCreatePlace { set; get; } = OpenningPlace.InlineDialog;
		public OpenningPlace OpenUpdatePlace { set; get; } = OpenningPlace.InlineDialog;

		public string ObjectIcon { set; get; } = "";
		public string ObjectColor { set; get; } = "";

		public string ParentColumn { set; get; } = "";
		public string NoteColumn { set; get; } = "";
		public string UiIconColumn { set; get; } = "";
		public string UiColorColumn { set; get; } = "";
		public string ViewOrderColumn { set; get; } = "";

		public List<DbColumn> Columns { set; get; } = [];
        public List<DbRelation>? Relations { set; get; }
        public List<DbQuery> DbQueries { set; get; } = [];

		public List<ClientUI>? ClientUIs { set; get; }
		public bool PreventBuildUI { set; get; } = false;
		public bool PreventAlterServerObjects { set; get; } = false;

		[JsonConstructor]
        public DbDialog() { }

        public DbDialog(string dbConfName, string objectName, string dialogsFolder)
        {
            DbConfName = dbConfName;
            ObjectName = objectName;
            _dbDialogsRoot = dialogsFolder;
        }

		public DbColumn GetPk()
		{
			DbColumn? dbColumn = Columns.FirstOrDefault(i => i.IsPrimaryKey == true) ?? throw new AppEndException("PrimaryKeyColumnIsNotDefined")
					.AddParam("DbDialog", ObjectName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			return dbColumn;
		}

		public DbColumn? GetFirstFileFieldName()
		{
			return Columns.FirstOrDefault(i => i.DbType.EqualsIgnoreCase("IMAGE"));
		}

		public List<DbColumn> GetOnAuditingFields()
		{
			List<DbColumn> dbColumns = [];
			DbColumn? createdOn = Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(SV.CreatedOn));
			DbColumn? UpdatedOn = Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(SV.UpdatedOn));
			if (createdOn is not null) dbColumns.Add(createdOn);
			if (UpdatedOn is not null) dbColumns.Add(UpdatedOn);
			return dbColumns;
		}

		public List<DbRelation> GetRelationsForAQuery(string queryName, RelationType? relationType = null, bool fileCentric = false)
		{
			DbQuery? dbQuery = DbQueries.FirstOrDefault(i => i.Name == queryName);
			if (dbQuery is null || dbQuery.Relations is null || Relations is null) return [];
			List<DbRelation> dbRelations = Relations.Where(o => ((relationType == null || o.RelationType == relationType) && o.IsFileCentric == fileCentric) && dbQuery.Relations.ContainsIgnoreCase(o.RelationName)).ToList();
			return [.. dbRelations];
		}

		public DbRelation GetRelation(string relationName)
        {
            DbRelation? dbRelation = Relations?.FirstOrDefault(i => i.RelationName == relationName);
			return dbRelation ?? throw new AppEndException("DbRelationIsNotDefined")
					.AddParam("DbDialog", ObjectName)
					.AddParam("DbRelation", relationName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
		}
		public DbColumn GetColumn(string columnName)
        {
            DbColumn? dbColumn = Columns?.FirstOrDefault(i => i.Name == columnName);
			return dbColumn ?? throw new AppEndException("ColumnIsNotExist")
					.AddParam("DbDialog", ObjectName)
					.AddParam("ColumnName", columnName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
		}
		public DbColumn? TryGetColumn(string columnName)
        {
            return Columns?.FirstOrDefault(i => i.Name == columnName);
        }
        public DbColumn? GetColumnIfExists(string columnName)
        {
            return Columns?.FirstOrDefault(i => i.Name == columnName);
        }

		public void Save()
        {
            if (_dbDialogsRoot is null) throw new AppEndException("DbDialogSaveWithNoPathIsNotPossible")
                    .AddParam("DbDialog", ObjectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;
            File.WriteAllText(GetFullFilePath(_dbDialogsRoot, DbConfName, ObjectName), this.ToJsonStringByBuiltIn(true, false));
			SV.SharedMemoryCache.TryRemove(GenCacheKey(DbConfName, ObjectName));
		}

		public bool IsTree()
		{
			DbColumn? dbColumn = Columns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == ObjectName);
			return dbColumn != null;
		}

		public DbColumn GetTreeParentColumn()
		{
			DbColumn? dbColumn = Columns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == ObjectName);
			return dbColumn is null
				? throw new AppEndException("DbDialogIsNotTree")
					.AddParam("DbDialog", ObjectName)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
				: dbColumn;
		}
		public string GetHumanIds()
		{
			return string.Join(", ", Columns.Where(i => i.IsHumanId == true).Select(i => i.Name));
		}
		public List<DbColumn> GetHumanIdsOrig()
		{
			return Columns.Where(i => i.IsHumanId == true).ToList();
		}
		public DbColumn? TryGetTreeParentColumn()
		{
			return Columns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == ObjectName);
		}
		public string GetTreeParentColumnName()
		{
			DbColumn? dbColumn = Columns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == ObjectName);
            if (dbColumn is null) return "";
            return dbColumn.Name;
		}

		public bool IsRefToOwner(string columnName)
		{
            DbColumn dbColumn = GetColumn(columnName);
            if(dbColumn.Fk is not null && dbColumn.Fk.TargetTable == ObjectName) return true;
            return false;
		}

		public string GetDbDialogFolder()
        {
            if (_dbDialogsRoot is null) return "";
            return _dbDialogsRoot;
        }

        public ClientQueryMetadata GetReadListClientQueryMetadata(string queryName)
        {
			ClientQueryMetadata cqm = new(ObjectName, ObjectType.ToString())
			{
				ParentObjectColumns= Columns.Where(i => !i.Name.ContainsIgnoreCase("password")).ToList(),
				FastSearchColumns = Columns.Where(i => i.UiProps?.SearchType == SearchType.Fast && !i.Name.ContainsIgnoreCase("password")).ToList(),
				ExpandableSearchColumns = Columns.Where(i => i.UiProps?.SearchType == SearchType.Expandable && !i.Name.ContainsIgnoreCase("password")).ToList()
			};

			DbQuery? dbQuery = DbQueries.FirstOrDefault(i => i.Name == queryName);

			if (dbQuery is not null)
            {
                cqm.Type = dbQuery.Type.ToString();
                cqm.Name = dbQuery.Name;
                if (dbQuery.Columns is not null)
                {
					foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
					{
						cqm.QueryColumns.AddSafe(dbQueryColumn.Name);
					}
				}
			}
			return cqm;
        }

        public static DbDialog Load(string dbDialogsRoot, string dbConfName, string? objectName)
        {
            string fp = GetFullFilePath(dbDialogsRoot, dbConfName, objectName);
            if (!File.Exists(fp)) throw new AppEndException("FilePathIsNotExist")
                    .AddParam("DbDialog", objectName.ToStringEmpty())
                    .AddParam("FilePath", fp)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;

            DbDialog? dbDialog;

			string dbDialogRaw = File.ReadAllText(fp);
			dbDialog = JsonSerializer.Deserialize<DbDialog>(dbDialogRaw) ?? throw new AppEndException("DeserializeError")
					.AddParam("DbDialog", objectName.ToStringEmpty())
					.AddParam("FilePath", fp)
					.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			dbDialog._dbDialogsRoot = dbDialogsRoot;
			return dbDialog;


			//string cacheKey = GenCacheKey(dbConfName, objectName);
			//SV.SharedMemoryCache.TryGetValue(cacheKey, out var cachedDbDialog);
			//if (cachedDbDialog != null)
			//{
			//	dbDialog = (DbDialog)cachedDbDialog;
			//	dbDialog._dbDialogsRoot = dbDialogsRoot;
			//	return dbDialog;
			//}
			//else
			//{
			//	string dbDialogRaw = File.ReadAllText(fp);
			//	dbDialog = JsonSerializer.Deserialize<DbDialog>(dbDialogRaw) ?? throw new AppEndException("DeserializeError")
			//			.AddParam("DbDialog", objectName.ToStringEmpty())
			//			.AddParam("FilePath", fp)
			//			.AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			//	dbDialog._dbDialogsRoot = dbDialogsRoot;
			//	SV.SharedMemoryCache.Set(cacheKey, dbDialog);
			//	return dbDialog;
			//}
		}
        public static DbDialog? TryLoad(string dbDialogsRoot, string dbConfName, string? objectName)
        {
            DbDialog? dbDialog = null;
            if (DbDialog.Exist(dbDialogsRoot, dbConfName, objectName))
            {
                dbDialog = DbDialog.Load(dbDialogsRoot, dbConfName, objectName);
            }
            return dbDialog;
        }

        public static string GetFullFilePath(string baseDbDialogFolder, string dbConfName, string? objectName)
        {
            return $"{baseDbDialogFolder}/{dbConfName}.{(objectName is null ? "db" : objectName)}.dbdialog.json";
        }

        public static bool Exist(string dbDialogsRoot, string dbConfName, string? objectName)
        {
            return File.Exists(GetFullFilePath(dbDialogsRoot, dbConfName, objectName));
        }

		public static bool IsColumnInParams(DbQuery dbQuery, string columnName)
		{
			if (dbQuery.Params is null) return false;
			DbParam? dbParam = dbQuery.Params.FirstOrDefault(i => i.Name == columnName);
			if (dbParam == null) return false;
			return true;
		}

		private static string GenCacheKey(string dbConfName, string? objectName)
		{
			if (!string.IsNullOrEmpty(objectName))
				return $"DbDialog :: {dbConfName}.{objectName}";
			else
                return $"DbDialog :: {dbConfName}";
        }

    }
}

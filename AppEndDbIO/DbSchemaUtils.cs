using System.Data;
using AppEndCommon;

namespace AppEndDbIO
{
	public class DbSchemaUtils
    {
        public string DbInfoName { init; get; }
        private DbConf DbInfo { init; get; }
        private DbIO dbIO;
        public DbIO DbIOInstance
        {
            get
            {
                dbIO ??= DbIO.Instance(DbInfo);
                return dbIO;
            }
        }
        public DbSchemaUtils(string dbInfoName)
        {
            DbInfoName = dbInfoName;
            DbInfo = DbConf.FromSettings(DbInfoName);
            dbIO = DbIO.Instance(DbInfo);
        }

        public List<DbTable> GetTables()
        {
            List<DbObject> dbObjects = GetObjects(DbObjectType.Table, null, true);
            List<DbTable> tables = [];
            foreach (DbObject dbObject in dbObjects)
            {
                DbTable dbTable = new(dbObject.Name);
                foreach (DbColumn dbColumn in GetTableViewColumns(dbObject.Name)) dbTable.Columns.Add(dbColumn.ToDbColumnChangeTrackable());
                tables.Add(dbTable);
            }
            return tables;
        }

        public List<DbView> GetViews(string? objectName = null, bool? exactNameSearch = false)
        {
            List<DbObject> dbObjects = GetObjects(DbObjectType.View, objectName, exactNameSearch);
            List<DbView> views = [];
            foreach (DbObject dbObject in dbObjects) views.Add(new DbView(dbObject.Name));
            return views;
        }

        public List<DbProcedure> GetProcedures(string? objectName = null, bool? exactNameSearch = false)
        {
            List<DbObject> dbObjects = GetObjects(DbObjectType.Procedure, objectName, exactNameSearch);
            List<DbProcedure> dbProcedures = [];
            foreach (DbObject dbObject in dbObjects) dbProcedures.Add(new DbProcedure(dbObject.Name));
            return dbProcedures;
        }

        public List<DbTableFunction> GetTableFunctions(string? objectName = null, bool? exactNameSearch = false)
        {
            List<DbObject> dbObjects = GetObjects(DbObjectType.TableFunction, objectName, exactNameSearch);
            List<DbTableFunction> dbTableFunctions = [];
            foreach (DbObject dbObject in dbObjects)
            {
                dbTableFunctions.Add(new DbTableFunction(dbObject.Name));
            }
            return dbTableFunctions;
        }
        public List<DbScalarFunction> GetScalarFunctions(string? objectName = null, bool? exactNameSearch = false)
        {
            List<DbObject> dbObjects = GetObjects(DbObjectType.ScalarFunction, objectName, exactNameSearch);
            List<DbScalarFunction> dbScalarFunctions = [];
            foreach (DbObject dbObject in dbObjects)
            {
                dbScalarFunctions.Add(new DbScalarFunction(dbObject.Name));
            }
            return dbScalarFunctions;
        }

        public List<DbObject> GetObjects(DbObjectType? objectType = null, string? objectName = null, bool? exactNameSearch = false)
        {
            string whereObjectType = objectType is null ? "" : $"ObjectType='{objectType}'";
            string whereObjectName = objectName is null ? "" : (exactNameSearch == false ? $"ObjectName LIKE '%{objectName}%'" : $"ObjectName = '{objectName}'");
            string and = "";
            if (whereObjectType != "" && whereObjectName != "") { and = " AND "; }
            string finalWhere = whereObjectType + and + whereObjectName;
            finalWhere = finalWhere.Trim() != "" ? $" WHERE {finalWhere}" : "";
            DataTable dataTable = DbIOInstance.ToDataTable($"SELECT * FROM ZzSelectObjectsDetails{finalWhere}").FirstOrDefault().Value;
            List<DbObject> objects = [];
            foreach (DataRow row in dataTable.Rows)
            {
                var obj = new DbObject(row["ObjectName"].ToStringEmpty(), (DbObjectType)Enum.Parse(typeof(DbObjectType), row["ObjectType"].ToStringEmpty()));
                objects.Add(obj);
            }
            return objects;
        }
        public List<DbColumn> GetTableViewColumns(string objectName)
        {
            if(objectName is null || objectName=="") throw new AppEndException("ObjectNameCanNotBeNullOrEmpty", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();

            DataTable dataTable = DbIOInstance.ToDataTable($"SELECT * FROM ZzSelectTablesViewsColumns WHERE ParentObjectName='{objectName}' ORDER BY ViewOrder").FirstOrDefault().Value;
            DataTable dtFks = GetTableFks(objectName);
            List<DbColumn> columns = [];
            foreach (DataRow row in dataTable.Rows)
            {
                DbColumn dbColumn = new((string)row["ColumnName"])
                {
                    IsPrimaryKey = (bool)row["IsPrimaryKey"],
                    DbType = (string)row["ColumnType"],
                    Size = row["MaxLen"] == DBNull.Value ? null : row["MaxLen"].ToString(),
                    AllowNull = (bool)row["AllowNull"],
                    DbDefault = row["DbDefault"] == DBNull.Value ? null : (string)row["DbDefault"],
                    IsIdentity = (bool)row["IsIdentity"],
                    IdentityStart = row["IdentityStart"] == DBNull.Value ? null : row["IdentityStart"].ToString(),
                    IdentityStep = row["IdentityStep"] == DBNull.Value ? null : row["IdentityStep"].ToString()
                };
                foreach (DataRow dataRow in dtFks.Rows)
                {
                    if (dataRow["ColumnName"].ToString() == dbColumn.Name)
	                    dbColumn.Fk = new((string)dataRow["FkName"], (string)dataRow["TargetTable"], (string)dataRow["TargetColumn"])
	                    {
		                    EnforceRelation = (bool)dataRow["EnforceRelation"]
	                    };
                }
                columns.Add(dbColumn);
            }
            return columns;
        }
        public DataTable GetTableFks(string objectName)
        {
            return DbIOInstance.ToDataTable($"SELECT * FROM ZzSelectTablesFks WHERE TableName='{objectName}'").FirstOrDefault().Value;
        }
        public List<DbParam>? GetProceduresFunctionsParameters(string objectName)
        {
            DataTable dt = DbIOInstance.ToDataTable($"SELECT * FROM ZzSelectProceduresFunctionsParameters  WHERE ObjectName='{objectName}' AND Direction='Input' ORDER BY ViewOrder").FirstOrDefault().Value;
            if (dt.Rows.Count == 0) return null;
            List<DbParam> dbParams = [];
            foreach (DataRow r in dt.Rows) 
            {
                dbParams.Add
                (
                    new DbParam(r["ParameterName"].ToStringEmpty().Replace("@", ""), r["ParameterDataType"].ToStringEmpty().ToUpper())
                    {
                        Size = r["Size"].ToString() == "" ? null : r["Size"].ToString()?.ToUpper(),
                        AllowNull = false
                    }
                );
            }
            return dbParams;
        }

        public void CreateOrAlterTable(DbTable dbTable)
        {
            DbColumnChangeTrackable? pkColumn = dbTable.Columns.FirstOrDefault(i => i.IsPrimaryKey = true) ?? throw new AppEndException("PrimaryKeyIsNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("TableName", dbTable.Name)
                    .GetEx();


			CreateMinTableIfNotExist(dbTable, pkColumn);
            foreach (var f in dbTable.Columns)
            {
                if(!f.IsPrimaryKey)
                {
                    string st = f.State.FixNullOrEmpty("");
                    if (st.Equals("n"))
                    {
                        CreateColumn(dbTable.Name, f.Name, DbUtils.GetTypeSize(f.DbType, f.Size), f.AllowNull);
                        if (f.Fk is not null) CreateOrAlterFk(dbTable.Name, f);
                    }
                    else if (st.Equals("u"))
                    {
                        if (!f.InitialName.IsNullOrEmpty() && f.InitialName != f.Name) DbIOInstance.ToNoneQuery($"EXEC DBO.ZzRenameColumn '{dbTable.Name}','{f.InitialName}','{f.Name}';");
                        AlterColumn(dbTable.Name, f.Name, DbUtils.GetTypeSize(f.DbType, f.Size), f.AllowNull, f.DbDefault);
                        if (f.Fk is not null) CreateOrAlterFk(dbTable.Name, f);
                    }
                    else if (st.Equals("d"))
                    {
                        DropColumn(dbTable.Name, f.Name);
                    }
                }
            }
        }

        public string GetCreateOrAlterObject(string objectName)
        {
            return DbIOInstance.ToScalar($"EXEC DBO.ZzGetCreateOrAlter '{objectName}';").ToStringEmpty();
        }

        public void AlterObjectScript(string objectScript)
        {
            try
            {
                DbIOInstance.ToNoneQuery(objectScript);
            }
            catch (Exception ex)
            {
                throw new AppEndException(ex.Message, System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
            }
        }

        private void CreateMinTableIfNotExist(DbTable dbTable, DbColumnChangeTrackable pk)
        {
            string fn;
            if (pk.DbType.EqualsIgnoreCase("GUID") || pk.DbType.EqualsIgnoreCase("UNIQUEIDENTIFIER"))
            {
                fn = $"EXEC DBO.ZzCreateTableGuid '{dbTable.Name}','{pk.Name}',1;";
            }
            else
            {
                fn = $"EXEC DBO.ZzCreateTableIdentity '{dbTable.Name}','{pk.Name}','{pk.DbType}',{pk.IdentityStart.FixNull("1")},{pk.IdentityStep.FixNull("1")},1;";
            }
            DbIOInstance.ToNoneQuery(fn);
        }
        public void TruncateTable(string tableName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzTruncateTable '{tableName}';");
        }
        public void DropTable(string tableName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropTable '{tableName}';");
        }
        public void RenameTable(string tableName,string newTableName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzRenameTable '{tableName}','{newTableName}';");
        }

        public void DropView(string viewName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropView '{viewName}';");
        }

        public void DropProcedure(string procedureName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropProcedure '{procedureName}';");
        }

        public void DropFunction(string functionName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropFunction '{functionName}';");
        }

        public void CreateEmptyView(string viewName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateEmptyView '{viewName}';");
        }

        public void CreateEmptyProcedure(string procedureName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateEmptyProcedure '{procedureName}';");
        }

        public void CreateEmptyTableFunction(string tableFunctionName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateEmptyTableFunction '{tableFunctionName}';");
        }

        public void CreateEmptyScalarFunction(string scalarFunctionName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateEmptyScalarFunction '{scalarFunctionName}';");
        }
        
        public void CreateColumn(string tableName, string columnName, string columnTypeSize, bool? allowNull = true)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateColumn '{tableName}','{columnName}','{columnTypeSize}',{(allowNull == true ? "1" : "0")};");
        }
        private void AlterColumn(string tableName, string columnName, string columnTypeSize, bool? allowNull = true, string? DefaultExp = null)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzAlterColumn '{tableName}','{columnName}','{columnTypeSize}',{(allowNull == true ? "1" : "0")},N'{(DefaultExp is null ? "" : DefaultExp)}';");
        }
        private void DropColumn(string tableName, string columnName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropColumn '{tableName}','{columnName}';");
        }
        private void CreateOrAlterFk(string tableName, DbColumnChangeTrackable tableColumn)
        {
            if (tableColumn.Fk?.FkName == "")
                tableColumn.Fk.FkName = $"{tableName}_{tableColumn.Name}_{tableColumn.Fk.TargetTable}_{tableColumn.Fk.TargetColumn}";
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzCreateOrAlterFk '{tableColumn.Fk?.FkName}','{tableName}','{tableColumn.Name}','{tableColumn.Fk?.TargetTable}','{tableColumn.Fk?.TargetColumn}',{(tableColumn.Fk?.EnforceRelation == true ? "1" : "0")};");
        }
        public void DropFk(string tableName, string fkName)
        {
            DbIOInstance.ToNoneQuery($"EXEC DBO.ZzDropFk '{fkName}','{tableName}'");
        }


    }
}

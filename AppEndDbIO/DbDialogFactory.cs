using AppEndCommon;
using AppEndDynaCode;
using System.Data;

namespace AppEndDbIO
{
	public class DbDialogFactory
    {
        private string DbDialogFolderPath { init; get; }
        private string DbConfName { init; get; }
		private DbConf DbInfo { init; get; }
		private DbSchemaUtils DbSchemaUtils { init; get; }
		
        private DbIO _dbIo;
        public DbIO DbIOInstance
        {
            get
            {
                _dbIo ??= DbIO.Instance(DbInfo);
                return _dbIo;
            }
        }

        public DbDialogFactory(string dbConfName)
        {
            DbDialogFolderPath = AppEndSettings.ServerObjectsPath;
            DbConfName = dbConfName;
            DbInfo = DbConf.FromSettings(DbConfName);
			DbSchemaUtils = new DbSchemaUtils(DbConfName);
			_dbIo = DbIO.Instance(DbInfo);
        }

		public void CreateLogicalFk(string fkName, string baseTable, string baseColumn, string targetTable, string targetColumn)
		{
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, baseTable);
            DbColumn dbColumn = dbDialog.GetColumn(baseColumn);
            dbColumn.Fk = new(fkName, targetTable, targetColumn) { EnforceRelation = false };
            dbDialog.Save();
		}

		public void RemoveLogicalFk(string baseTable, string baseColumn)
		{
			DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, baseTable);
			DbColumn dbColumn = dbDialog.GetColumn(baseColumn);
            dbColumn.Fk = null;
			dbDialog.Save();
		}

		public void CreateQuery(string objectName, string methodType, string methodName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            QueryType queryType = Enum.Parse<QueryType>(methodType);
            DbQuery dbQ = queryType switch
            {
                QueryType.Create => GetCreateQuery(dbDialog),
                QueryType.ReadList => GetReadListQuery(dbDialog, DbDialogFolderPath),
                QueryType.AggregatedReadList => GetAggregatedReadListQuery(dbDialog, DbDialogFolderPath),
                QueryType.ReadByKey => GetReadByKeyQuery(dbDialog),
                QueryType.UpdateByKey => GetUpdateByKeyQuery(dbDialog),
                _ => throw new AppEndException("QueryTypeNotSupported")
                                        .AddParam("QueryType", queryType)
                                        .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}"),
            };
			dbQ.Name = methodName;
            dbDialog.DbQueries.Add(dbQ);

            //add ClientUI
            ClientUI clientUi = GetClientUI(dbDialog, dbQ);
            dbDialog.ClientUIs ??= [];
            dbDialog.ClientUIs.Add(clientUi);

            dbDialog.Save();
        }
        public void CreateNewUpdateByKey(string objectName, List<string> columnsToUpdate, string methodName, string? byColumnName, string? onColumnName, string? logTableName, bool rebuildApplication = true)
        {
            string finalLogTableName = logTableName is null ? "" : logTableName;
            if (finalLogTableName == "$auto$") finalLogTableName = GenLogTableName(objectName, methodName);
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbSchemaUtils dbSchemaUtils = new(DbConfName);
            if (columnsToUpdate.Count == 0) throw new AppEndException("YouMustIndicateAtleastOneColumnToCreateUpdateByKeyApi")
                    .AddParam("ObjectName", objectName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}")
                    ;

            DbColumn pkCol = dbDialog.GetPk();

            List<string> justColumns = [.. columnsToUpdate];


            // deciding for auditing columns
            string byColName = byColumnName.FixNullOrEmpty("_Auto_");
            if (byColName != "_Ignore_")
            {
                if (byColName == "_Auto_") byColName = columnsToUpdate.Count == 1 ? columnsToUpdate[0] + "UpdatedBy" : methodName + "CalledBy";
                DbColumn? byDbCol = dbDialog.Columns.FirstOrDefault(i => i.Name == byColName);
                if (byDbCol is null)
                {
                    dbSchemaUtils.CreateColumn(objectName, byColName, "INT", true);
                    byDbCol = new DbColumn(byColName) { DbType = "INT", AllowNull = true, UiProps = new() { } };
                    dbDialog.Columns.Add(byDbCol);
                }
                justColumns.Add(byColName);
            }

            string onColName = onColumnName.FixNullOrEmpty("_Auto_");
            if (onColName != "_Ignore_")
            {
                if (onColName == "_Auto_") onColName = columnsToUpdate.Count == 1 ? columnsToUpdate[0] + "UpdatedOn" : methodName + "CalledOn";
                DbColumn? onDbCol = dbDialog.Columns.FirstOrDefault(i => i.Name == onColName);
                if (onDbCol is null)
                {
                    dbSchemaUtils.CreateColumn(objectName, onColName, "DATETIME", true);
                    onDbCol = new DbColumn(onColName) { DbType = "DATETIME", AllowNull = true, UiProps = new() { } };
                    dbDialog.Columns.Add(onDbCol);
                }
                justColumns.Add(onColName);
            }


            // remove columns from UpdateByKey query
            DbQuery? qUpdateByKey = dbDialog.DbQueries.FirstOrDefault(i => i.Name == "UpdateByKey");
            if (qUpdateByKey is not null)
            {
                foreach (string s in columnsToUpdate)
                {
                    DbQueryColumn? qCol = qUpdateByKey.Columns?.FirstOrDefault(i => i.Name == s);
                    if (qCol is not null && qUpdateByKey.Columns is not null) qUpdateByKey.Columns.Remove(qCol);
                }

                DbQueryColumn? qcBy = qUpdateByKey.Columns?.FirstOrDefault(i => i.Name == byColName);
                if (qcBy is not null && qUpdateByKey.Columns is not null)
                {
                    qUpdateByKey.Columns.Remove(qcBy);
                }
                DbQueryColumn? qcOn = qUpdateByKey.Columns?.FirstOrDefault(i => i.Name == onColName);
                if (qcOn is not null && qUpdateByKey.Columns is not null)
                {
                    qUpdateByKey.Columns.Remove(qcOn);
                }
            }

            // add new columns to ReadList query
            DbQuery? qReadList = dbDialog.DbQueries.FirstOrDefault(i => i.Name == "ReadList");
            if (qReadList is not null)
            {
                foreach (string s in columnsToUpdate)
                {
                    DbQueryColumn? qCol = qReadList.Columns?.FirstOrDefault(i => i.Name == s);
                    if (qCol is null && qReadList.Columns is not null) qReadList.Columns.Add(new DbQueryColumn() { Name = s });
                }
            }


            // create dbquery
            DbQuery dbQueryUpdate = GetUpdateByKeyQuery(dbDialog, justColumns, byColName, onColName);
            if (dbQueryUpdate.Columns is not null)
            {
				if (!dbQueryUpdate.Columns.Exists(i => i.Name == pkCol.Name))
				{
                    dbQueryUpdate.Columns.Insert(0, new DbQueryColumn() { Name = pkCol.Name });
				}
			}
			dbQueryUpdate.Name = methodName;
            if (!finalLogTableName.IsNullOrEmpty()) dbQueryUpdate.LogTable = finalLogTableName;
            dbDialog.DbQueries.Add(dbQueryUpdate);

            // updating UpdateGroup
            foreach(string col in justColumns)
            {
                dbDialog.GetColumn(col).UpdateGroup = methodName.Replace("State", "Update");
			}

            // add ClientUI
            ClientUI clientUi = GetClientUI(dbDialog, dbQueryUpdate);
            dbDialog.ClientUIs ??= [];
            dbDialog.ClientUIs.Add(clientUi);

			// save DbDialog
			dbDialog.Save();

            // add related csharp method
            DynaCode.Refresh();
			DynaCode.CreateMethod($"{DbConfName}.{objectName}", methodName);
			
            if (rebuildApplication)
            {
				DynaCode.Refresh();
			}

			// create log table and related server objects
			if (!finalLogTableName.IsNullOrEmpty()) CreateOrAlterLogTableForAnUpdateQuery(objectName, methodName, finalLogTableName);

        }

        public void CreateOrAlterLogTableForAnUpdateQuery(string objectName, string updateQueryName, string logTableName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbObject? logTable = DbSchemaUtils.GetObjects(DbObjectType.Table, logTableName, true).FirstOrDefault(i => i.Name == logTableName);
            DbQuery? updateQ = dbDialog.DbQueries.FirstOrDefault(i => i.Name == updateQueryName);
            if (updateQ is null) return;
            if (logTable is null)
            {
                DbColumn pk = dbDialog.GetPk();
                DbTable dbTable = new(logTableName);
                dbTable.Columns.Add(new DbColumnChangeTrackable("Id") { DbType = "INT", AllowNull = false, IsIdentity = true, IdentityStart = "10000", IdentityStep = "1", IsPrimaryKey = true });
                DbColumnChangeTrackable masterId = new("MasterId") { DbType = pk.DbType, AllowNull = false, Fk = new("", objectName, pk.Name) };
                dbTable.Columns.Add(masterId);
                if (updateQ.Columns is null) return;
                foreach (DbQueryColumn dbQueryColumn in updateQ.Columns)
                {
                    if (dbQueryColumn.Name is not null && dbQueryColumn.Name != pk.Name)
                    {
                        DbColumn dbColumn = dbDialog.GetColumn(dbQueryColumn.Name);
                        DbColumnChangeTrackable dbColumnChangeTrackable = new(dbColumn.Name) { DbType = dbColumn.DbType, Size = dbColumn.Size, AllowNull = dbColumn.AllowNull };
                        dbTable.Columns.Add(dbColumnChangeTrackable);
                    }
                }
                
                dbTable.Columns.Add(new DbColumnChangeTrackable("CreatedBy") { DbType = "INT", Size = null, AllowNull = false });
                dbTable.Columns.Add(new DbColumnChangeTrackable("CreatedOn") { DbType = "DATETIME", AllowNull = false });
                DbSchemaUtils.CreateOrAlterTable(dbTable);
                Thread.Sleep(100);
                logTable = DbSchemaUtils.GetObjects(DbObjectType.Table, logTableName, true).FirstOrDefault(i => i.Name == logTableName);
            }

            if (!DbDialog.Exist(DbDialogFolderPath, DbConfName, objectName) && logTable is not null)
            {
                CreateServerObjectsFor(logTable);
            }
        }

        public void RemoveRemovedRelationsFromDbQueries(string objectName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            foreach (DbQuery dbQuery in dbDialog.DbQueries)
            {
                if (dbQuery.Relations is not null && dbQuery.Relations.Count > 0)
                {
                    List<string> toRemove = [];
                    foreach (string dbRelationName in dbQuery.Relations)
                    {
                        DbRelation? dbRelation = dbDialog.Relations?.FirstOrDefault(i => i.RelationName == dbRelationName);
                        if (dbRelation == null) toRemove.Add(dbRelationName);
                    }
                    foreach(string s in toRemove) dbQuery.Relations.Remove(s);
                }
            }
            dbDialog.Save();
        }
        public void SyncDbDialog(string objectName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            List<DbColumn> dbColumns = DbSchemaUtils.GetTableViewColumns(objectName);


            // add new column
            foreach(DbColumn dbColumn in dbColumns)
            {
                var lst = dbDialog.Columns.Where(i => i.Name == dbColumn.Name).ToList();
                if (lst.Count == 0)
                {
					SetUiProps(dbColumn);
					dbDialog.Columns.Add(dbColumn);
                }
            }

            List<DbColumn> toRemove = [];
			foreach (DbColumn dbColumn in dbDialog.Columns)
			{
				var lst = dbColumns.Where(i => i.Name == dbColumn.Name).ToList();
				if (lst.Count == 0)
				{
					toRemove.Add(dbColumn);
                }
			}

            foreach(DbColumn dbColumn in toRemove)
            {
                dbDialog.Columns.Remove(dbColumn);

				foreach (DbQuery dbQuery in dbDialog.DbQueries)
				{
					if (dbQuery.Columns?.Count > 0)
					{
						DbQueryColumn? dbQueryColumn = dbQuery.Columns.FirstOrDefault(i => i.Name == dbColumn.Name);
						if (dbQueryColumn != null)
						{
							dbQuery.Columns.Remove(dbQueryColumn);
						}
					}
				}
			}

			dbDialog.Save();
        }

        public void RemoveServerObjectsFor(string dbObjectName)
        {
            string dbDialogFilePath = DbDialog.GetFullFilePath(DbDialogFolderPath, DbConfName, dbObjectName);
            string settingsFilePath = dbDialogFilePath.Replace(".dbdialog.json", ".settings.json");
            string csharpFilePath = dbDialogFilePath.Replace(".dbdialog.json", ".cs");
            if (File.Exists(dbDialogFilePath)) { File.Delete(dbDialogFilePath); }
            if (File.Exists(settingsFilePath)) { File.Delete(settingsFilePath); }
            if (File.Exists(csharpFilePath)) {  File.Delete(csharpFilePath); }
        }

        public void CreateServerObjectsFor(DbObject dbObject)
        {
            AppEndClass appEndClass = new(dbObject.Name, DbConfName);

            DbDialog dbDialog = new(DbConfName, dbObject.Name, DbDialogFolderPath)
            {
                ObjectName = dbObject.Name,
                ObjectType = dbObject.DbObjectType,
                DbConfName = DbConfName
            };

            if (dbObject.DbObjectType == DbObjectType.Table || dbObject.DbObjectType == DbObjectType.View)
            {
                List<DbColumn> dbColumns = DbSchemaUtils.GetTableViewColumns(dbObject.Name);
                foreach(DbColumn dbColumn in dbColumns)
                {
                    dbColumn.IsHumanId = dbColumn.ColumnIsForDisplay() ? true : null;
                    SetUiProps(dbColumn);
                }
                
                dbDialog.Columns.AddRange(dbColumns);

                dbDialog.Relations = GetRelations(dbDialog, DbSchemaUtils);

				// set moreinfo items
				dbDialog.ObjectIcon = dbDialog.IsTree() ? "fa-tree" : "fa-list";
                if(dbDialog.GetColumnIfExists("Note") is not null ) dbDialog.NoteColumn = "Note";
				if (dbDialog.GetColumnIfExists("ViewOrder") is not null) dbDialog.ViewOrderColumn = "ViewOrder";
				if (dbDialog.GetColumnIfExists("UiColor") is not null) dbDialog.UiColorColumn = "UiColor";
				if (dbDialog.GetColumnIfExists("UiIcon") is not null) dbDialog.UiIconColumn = "UiIcon";
			}

			if (dbObject.DbObjectType == DbObjectType.Table)
            {
                dbDialog.DbQueries.Add(GetCreateQuery(dbDialog));
                dbDialog.DbQueries.Add(GetReadByKeyQuery(dbDialog));
                dbDialog.DbQueries.Add(GetReadListQuery(dbDialog, DbDialogFolderPath));
                dbDialog.DbQueries.Add(GetUpdateByKeyQuery(dbDialog));
                dbDialog.DbQueries.Add(GetDelete(dbDialog));
                dbDialog.DbQueries.Add(GetDeleteByKeyQuery(dbDialog));

                appEndClass.DbMethods.Add(nameof(QueryType.Create));
                appEndClass.DbMethods.Add(nameof(QueryType.ReadList));
                appEndClass.DbMethods.Add(nameof(QueryType.ReadByKey));
                appEndClass.DbMethods.Add(nameof(QueryType.UpdateByKey));
                appEndClass.DbMethods.Add(nameof(QueryType.Delete));
                appEndClass.DbMethods.Add(nameof(QueryType.DeleteByKey));
            }
            else if (dbObject.DbObjectType == DbObjectType.View)
            {
                dbDialog.DbQueries.Add(GetReadListQuery(dbDialog, DbDialogFolderPath));
                appEndClass.DbMethods.Add(nameof(QueryType.ReadList));
            }
            else if (dbObject.DbObjectType == DbObjectType.Procedure)
            {
                dbDialog.DbQueries.Add(GetExecQuery(dbDialog, DbSchemaUtils));
                appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            }
            else if (dbObject.DbObjectType == DbObjectType.TableFunction)
            {
                dbDialog.DbQueries.Add(GetSelectForTableFunction(dbDialog, DbSchemaUtils));
                appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            }
            else if (dbObject.DbObjectType == DbObjectType.ScalarFunction)
            {
                dbDialog.DbQueries.Add(GetSelectForScalarFunction(dbDialog, DbSchemaUtils));
                appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            }

            // adding default ClientUIs
            foreach (DbQuery dbQuery in dbDialog.DbQueries)
            {
                if (IsDbQueryTypeSuitableForClientUI(dbQuery.Type))
                {
                    dbDialog.ClientUIs ??= [];
                    dbDialog.ClientUIs.Add(GetClientUI(dbDialog, dbQuery));
                }
            }

            dbDialog.Save();

            // generating controller file
            string csharpFileContent = appEndClass.ToCode();
            string csharpFilePath = DbDialog.GetFullFilePath(DbDialogFolderPath, DbConfName, dbObject.Name).Replace(".dbdialog.json", ".cs");
            File.WriteAllText(csharpFilePath, csharpFileContent);

            // generating additional UpdateByKey methods
            if (dbObject.DbObjectType == DbObjectType.Table)
            {
                List<DbColumn> dbColumnsToCreateUpdateMethod = dbDialog.Columns.Where(i => i.IsPrimaryKey == false && i.IsIdentity == false).ToList();
                foreach (DbColumn dbColumn in dbColumnsToCreateUpdateMethod)
                {
                    List<string> colsToUpdate = [dbColumn.Name];
                    DbColumn? dbColBy = dbDialog.Columns.FirstOrDefault(i => i.Name == dbColumn.Name + "UpdatedBy" || i.Name == dbColumn.Name + "By");
                    DbColumn? dbColOn = dbDialog.Columns.FirstOrDefault(i => i.Name == dbColumn.Name + "UpdatedOn" || i.Name == dbColumn.Name + "On");
                    if(dbColBy is not null || dbColOn is not null)
						CreateNewUpdateByKey(dbObject.Name, colsToUpdate, $"{dbColumn.Name}Update", dbColBy is null ? "_Ignore_" : dbColBy.Name, dbColOn is null ? "_Ignore_" : dbColOn.Name, null, false);
				}
			}

        } 

        public void ReCreateMethodJson(DbObject dbObject, string methodName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, dbObject.Name);
            var theQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == methodName);
            if (theQuery is null) return;

            theQuery = theQuery.Type switch
            {
                QueryType.Create => GetCreateQuery(dbDialog),
                QueryType.ReadByKey => GetReadByKeyQuery(dbDialog),
                QueryType.ReadList => GetReadListQuery(dbDialog, DbDialogFolderPath),
                QueryType.UpdateByKey => GetUpdateByKeyQuery(dbDialog),
                QueryType.DeleteByKey => GetDeleteByKeyQuery(dbDialog),
                QueryType.Procedure => GetExecQuery(dbDialog, DbSchemaUtils),
                QueryType.TableFunction => GetSelectForTableFunction(dbDialog, DbSchemaUtils),
                QueryType.ScalarFunction => GetSelectForScalarFunction(dbDialog, DbSchemaUtils),
                _ => throw new AppEndException("QueryTypeNotSupported")
                                        .AddParam("QueryType", theQuery.Type)
                                        .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}"),
            };
			dbDialog.Save();
        }

        public void DuplicateQuery(string objectName, string methodName, string methodCopyName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(s => s.Name == methodName);
            if (dbQuery is null) return;

            string tempString = dbQuery.ToJsonStringByBuiltIn(true, false);
            DbQuery? dbQueryCopy = ExtensionsForJson.TryDeserializeTo<DbQuery>(tempString) ?? throw new AppEndException("DeserializeError")
                    .AddParam("ObjectName", objectName)
                    .AddParam("MethodName", methodName)
                    .AddParam("Site", $"{System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name}, {System.Reflection.MethodBase.GetCurrentMethod()?.Name}");
			dbQueryCopy.Name = methodCopyName;
            dbDialog.DbQueries.Add(dbQueryCopy);
            dbDialog.Save();
        }
        public void RemoveQuery(string objectName, string methodName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(s => s.Name == methodName);
            if (dbQuery == null) return;

            if (dbQuery.Columns is not null)
                foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
                    if (dbQueryColumn.Name is not null && dbDialog.GetColumn(dbQueryColumn.Name).UpdateGroup == methodName)
                        dbDialog.GetColumn(dbQueryColumn.Name).UpdateGroup = "";

			if (dbQuery.LogTable is not null && dbQuery.LogTable != "")
            {
                RemoveServerObjectsFor(dbQuery.LogTable);
                DbSchemaUtils.DropTable(dbQuery.LogTable);
            }

            dbDialog.DbQueries.Remove(dbQuery);
            dbDialog.Save();
            DynaCode.RemoveMethod($"{DbConfName}.{objectName}.{methodName}");
        }

        public static List<DbRelation>? GetRelations(DbDialog dbDialog,DbSchemaUtils dbSchemaUtils)
        {
            if (dbDialog.ObjectName.EndsWith("BaseInfo")) return null;
            List<DbRelation> list = [];
            List<DbTable> tables = dbSchemaUtils.GetTables();
            foreach (DbTable table in tables)
            {
                List<DbColumn> dbColumns = dbSchemaUtils.GetTableViewColumns(table.Name).RemoveAuditingColumns();
                DbColumn? tablePk = dbColumns.FirstOrDefault(i => i.IsPrimaryKey == true);
                if (tablePk != null)
                {
                    bool fileCentric = DbUtils.ColumnsAreFileCentric(dbColumns);
                    DbColumn? fkToThis = dbColumns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == dbDialog.ObjectName);
                    if (fkToThis != null)
                    {
                        DbRelation otm = new(table.Name, tablePk.Name, fkToThis.Name)
                        {
                            RelationType = RelationType.OneToMany,
                            CreateQuery = "Create",
                            ReadListQuery = "ReadList",
                            UpdateByKeyQuery = "UpdateByKey",
                            DeleteQuery = "Delete",
                            DeleteByKeyQuery = "DeleteByKey",
                            IsFileCentric = fileCentric
                        };
                        if (dbColumns.Count == 3) // it is a ManyToMany table
                        {
                            DbColumn? md3 = dbColumns.FirstOrDefault(i => i.Name != fkToThis.Name && i.Name != tablePk.Name);
                            if (md3 != null && md3.Fk is not null)
                            {
                                otm.RelationType = RelationType.ManyToMany;
                                otm.LinkingTargetTable = md3.Fk?.TargetTable;
                                otm.LinkingColumnInManyToMany = md3.Name;
                                otm.RelationUiWidget = md3.Fk?.TargetTable.ContainsIgnoreCase("tags") == true ? RelationUiWidget.AddableList : RelationUiWidget.CheckboxList;
                            }
                        }
                        else
                        {
                            if (fileCentric) otm.RelationUiWidget = RelationUiWidget.Cards;
                            else otm.RelationUiWidget = RelationUiWidget.Grid;
                        }
                        list.Add(otm);
                    }
                }
            }
            if(list.Count > 0) return list;
            return null;
        }
		public static ClientUI GetClientUI(DbDialog dbDialog, DbQuery dbQuery)
		{
            ClientUI clientUi = new() { TemplateName = GetTemplateName(dbDialog, dbQuery), FileName = GetClientUIComponentName(dbDialog.DbConfName, dbDialog.ObjectName, dbQuery.Name) };

			if (dbQuery.Type == QueryType.Create) clientUi.LoadAPI = "";
			else if (dbQuery.Type == QueryType.UpdateByKey) clientUi.LoadAPI = "ReadByKey";
			else clientUi.LoadAPI = dbQuery.Name;

			if (dbQuery.Type == QueryType.Create || dbQuery.Type == QueryType.UpdateByKey) clientUi.SubmitAPI = dbQuery.Name;
			else clientUi.SubmitAPI = "";

			return clientUi;
		}
		public static DbQuery GetAggregatedReadListQuery(DbDialog dbDialog, string dbDialogFolderPath)
		{
			DbQuery dbQuery = new(nameof(QueryType.AggregatedReadList), QueryType.AggregatedReadList) { Columns = [] };
			foreach (DbColumn col in dbDialog.Columns)
			{
				if (col.ColumnIsForAggregatedReadList())
				{
					DbQueryColumn dbQueryColumn = new() { Name = col.Name };
					if (col.Fk is not null && DbDialog.Exist(dbDialogFolderPath, dbDialog.DbConfName, col.Fk?.TargetTable))
					{
						dbQueryColumn.RefTo = new(col.Fk.TargetTable, col.Fk.TargetColumn)
						{
							Columns = []
						};

						DbDialog dbDialogTarget = DbDialog.Load(dbDialogFolderPath, dbDialog.DbConfName, col.Fk?.TargetTable);
						foreach (var targetCol in dbDialogTarget.Columns)
						{
							if (targetCol.Name.ContainsIgnoreCase("Title") || targetCol.Name.ContainsIgnoreCase("Name"))
							{
								dbQueryColumn.RefTo.Columns.Add(new() { Name = targetCol.Name, As = $"{col.Name}_{targetCol.Name}" });
							}
						}
						if (dbQueryColumn.RefTo.Columns.Count == 0)
						{
							DbColumn? dbColumn = dbDialogTarget.Columns.FirstOrDefault(i => !i.IsPrimaryKey);
							if (dbColumn == null)
							{
								dbQueryColumn.RefTo = null;
							}
							else
							{
								dbQueryColumn.RefTo?.Columns.Add(new() { Name = dbColumn.Name, As = $"{col.Name}_{dbColumn.Name}" });
							}
						}
					}
					dbQuery.Columns.Add(dbQueryColumn);
				}
			}
			dbQuery.PaginationMaxSize = 100;
			dbQuery.Aggregations = [new DbAggregation("Count", "COUNT(*)")];
			return dbQuery;
		}
		public static DbQuery GetReadListQuery(DbDialog dbDialog, string dbDialogFolderPath)
		{
			DbQuery dbQuery = new(nameof(QueryType.ReadList), QueryType.ReadList) { Columns = [] };
			foreach (DbColumn col in dbDialog.Columns)
			{
				if (col.ColumnIsForReadList())
				{
					DbQueryColumn dbQueryColumn = new() { Name = col.Name };
					if (col.Fk is not null && DbDialog.Exist(dbDialogFolderPath, dbDialog.DbConfName, col.Fk?.TargetTable))
					{
						dbQueryColumn.RefTo = new(col.Fk.TargetTable, col.Fk.TargetColumn) { Columns = [] };

						DbDialog dbDialogTarget = DbDialog.Load(dbDialogFolderPath, dbDialog.DbConfName, col.Fk?.TargetTable);

						foreach (var targetCol in dbDialogTarget.Columns)
							if (targetCol.Name.ContainsIgnoreCase("Title") || targetCol.Name.ContainsIgnoreCase("Name"))
								dbQueryColumn.RefTo.Columns.Add(new() { Name = targetCol.Name, As = $"{col.Name}_{targetCol.Name}" });

						if (dbQueryColumn.RefTo.Columns.Count == 0)
						{
							DbColumn? dbColumn = dbDialogTarget.Columns.FirstOrDefault(i => !i.IsPrimaryKey);
							if (dbColumn == null) dbQueryColumn.RefTo = null;
							else dbQueryColumn.RefTo?.Columns.Add(new() { Name = dbColumn.Name, As = $"{col.Name}_{dbColumn.Name}" });
						}
					}
					dbQuery.Columns.Add(dbQueryColumn);
				}
			}
			dbQuery.PaginationMaxSize = 100;
			dbQuery.Relations = GetRelationsForDbQueries(dbQuery, dbDialog.Relations);
			dbQuery.Aggregations = [new DbAggregation("Count", "COUNT(*)")];
			return dbQuery;
		}
		public static DbQuery GetSelectForScalarFunction(DbDialog dbDialog, DbSchemaUtils dbSchemaUtils)
		{
			return new("Calculate", QueryType.ScalarFunction) { Params = dbSchemaUtils.GetProceduresFunctionsParameters(dbDialog.ObjectName) };
		}
		public static DbQuery GetSelectForTableFunction(DbDialog dbDialog, DbSchemaUtils dbSchemaUtils)
		{
			return new("Select", QueryType.TableFunction) { Params = dbSchemaUtils.GetProceduresFunctionsParameters(dbDialog.ObjectName) };
		}
		public static DbQuery GetExecQuery(DbDialog dbDialog, DbSchemaUtils dbSchemaUtils)
		{
			return new("Exec", QueryType.Procedure) { Params = dbSchemaUtils.GetProceduresFunctionsParameters(dbDialog.ObjectName) };
		}
		public static DbQuery GetCreateQuery(DbDialog dbDialog)
		{
			DbQuery dbQuery = new(nameof(QueryType.Create), QueryType.Create) { Columns = [], Params = [] };
			foreach (DbColumn col in dbDialog.Columns)
			{
				if (col.ColumnIsForCreate())
				{
					dbQuery.Columns.Add(new DbQueryColumn() { Name = col.Name });
					if (col.Name.EndsWith("_xs"))
						dbQuery.Params.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForImage(col.Name), Size = col.Size, AllowNull = col.AllowNull });
					if (col.Name == "CreatedBy")
						dbQuery.Params.Add(new DbParam(col.Name, "INT") { ValueSharp = GetValueSharpForContext("UserId"), AllowNull = col.AllowNull });
					if (col.Name == "CreatedOn")
						dbQuery.Params.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForNow(), Size = col.Size, AllowNull = col.AllowNull });
				}
			}
			dbQuery.Relations = GetRelationsForDbQueries(dbQuery, dbDialog.Relations);
			return dbQuery;
		}
		public static DbQuery GetReadByKeyQuery(DbDialog dbDialog)
		{
			DbColumn pkColumn = dbDialog.GetPk();
			DbQuery dbQuery = new(nameof(QueryType.ReadByKey), QueryType.ReadByKey) { Columns = [] };
			foreach (DbColumn col in dbDialog.Columns) if (col.ColumnIsForReadByKey()) dbQuery.Columns.Add(new DbQueryColumn() { Name = col.Name });
			dbQuery.Where = GetByPkWhere(pkColumn, dbDialog);
			dbQuery.Relations = GetRelationsForDbQueries(dbQuery, dbDialog.Relations);
			return dbQuery;
		}
		public static DbQuery GetUpdateByKeyQuery(DbDialog dbDialog, List<string>? justColumns = null, string? byColName = null, string? onColName = null)
		{
			DbColumn pkColumn = dbDialog.GetPk();
			DbQuery dbQuery = new(nameof(QueryType.UpdateByKey), QueryType.UpdateByKey) { Columns = [], Params = [] };
			foreach (DbColumn col in dbDialog.Columns)
			{
				if ((justColumns is null && col.ColumnIsForUpdateByKey()) || (justColumns is not null && justColumns.ContainsIgnoreCase(col.Name)))
				{
					dbQuery.Columns.Add(new DbQueryColumn() { Name = col.Name });
					if (col.Name.EndsWith("_xs"))
						dbQuery.Params.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForImage(col.Name), Size = col.Size, AllowNull = col.AllowNull });
					if ((justColumns is null && col.Name == "UpdatedBy") || (justColumns is not null && col.Name == byColName))
						dbQuery.Params.Add(new DbParam(col.Name, "INT") { ValueSharp = GetValueSharpForContext("UserId"), AllowNull = col.AllowNull });
					if ((justColumns is null && col.Name == "UpdatedOn") || (justColumns is not null && col.Name == onColName))
						dbQuery.Params.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForNow(), Size = col.Size, AllowNull = col.AllowNull });
				}
			}
			dbQuery.Where = GetByPkWhere(pkColumn, dbDialog);
			if (justColumns is null)
			{
				dbQuery.Relations = GetRelationsForDbQueries(dbQuery, dbDialog.Relations);
			}

			return dbQuery;
		}
		public static Where GetByPkWhere(DbColumn pkColumn, DbDialog dbDialog)
		{
			return new() { SimpleClauses = [new ComparePhrase(DbUtils.GetSetColumnParamPair(dbDialog.ObjectName, pkColumn.Name, null))] };
		}
		public static List<string>? GetRelationsForDbQueries(DbQuery dbQuery, List<DbRelation>? dbRelations)
        {
            if (dbRelations == null) return null;
            if (dbQuery.Name == "ReadByKey" || dbQuery.Name == "UpdateByKey" || dbQuery.Name == "Create")
                return dbRelations.Select(i => i.RelationName).ToList();
            else if (dbQuery.Name == "ReadList")
                return dbRelations.Where(i => i.RelationType == RelationType.ManyToMany).Select(i => i.RelationName).ToList();
            else return null;
        }
		public static DbQuery GetDelete(DbDialog dbDialog)
		{
			DbQuery dbQuery = new(nameof(QueryType.Delete), QueryType.Delete) { Columns = [] };

			foreach (DbColumn col in dbDialog.Columns)
				if (col.ColumnIsForDelete()) dbQuery.Columns.Add(new DbQueryColumn() { Name = col.Name });

			return dbQuery;
		}
		public static DbQuery GetDeleteByKeyQuery(DbDialog dbDialog)
		{
			DbColumn pkColumn = dbDialog.GetPk();
			DbQuery dbQuery = new(nameof(QueryType.DeleteByKey), QueryType.DeleteByKey)
			{
				Columns = [new DbQueryColumn() { Name = pkColumn.Name }],
				Where = GetByPkWhere(pkColumn, dbDialog)
			};
			dbQuery.Relations = GetRelationsForDbQueries(dbQuery, dbDialog.Relations);

			return dbQuery;
		}
		public static string GetTemplateName(DbDialog dbDialog, DbQuery dbQuery)
        {
            if (dbQuery.Type == QueryType.ReadList && dbDialog.IsTree()) return "ReadTreeList";
			return dbQuery.Type.ToString();
        }
        public static string GetClientUIComponentName(string dbConfName, string objectName, string templateName)
        {
            return $"{dbConfName}_{objectName}_{templateName}";
        }
        public static string GenLogTableName(string mainTableName, string queryNameToLog)
        {
            return $"{mainTableName}{queryNameToLog}Log";
        }
        public static bool IsDbQueryTypeSuitableForClientUI(QueryType qT)
        {
            if (qT == QueryType.ReadList || qT == QueryType.AggregatedReadList || qT == QueryType.Create || qT == QueryType.UpdateByKey) return true;
            return false;
        }
		public static string GetValueSharpForImage(string columnName)
		{
			return $"#Resize:{columnName.Replace("_xs", "")},75";
		}
		public static string GetValueSharpForNow()
		{
			return $"#Now";
		}
		public static string GetValueSharpForContext(string contextName)
		{
			return $"#Context:{contextName}";
		}
		public static void SetUiProps(DbColumn dbColumn)
        {
            dbColumn.UiProps = new UiProps
            {
                UiWidget = dbColumn.CalculateBestUiWidget(),
                IsDisabled = dbColumn.CalculateIsDisabled(),
                Required = !dbColumn.AllowNull,
                SearchType = SearchType.None
            };

            if (dbColumn.IsHumanId == true || dbColumn.UiProps.UiWidget == UiWidget.Combo || dbColumn.UiProps.UiWidget == UiWidget.Radio)
                dbColumn.UiProps.SearchType = SearchType.Fast;

            if (!dbColumn.DbType.EqualsIgnoreCase("image") && !dbColumn.IsDateTime() && !dbColumn.IsDate()) 
                dbColumn.UiProps.SearchType = SearchType.Expandable;
            else 
                dbColumn.UiProps.SearchType = SearchType.None;


			if (dbColumn.IsNumerical()) dbColumn.UiProps.ValidationRule = ":=i(0,10000)";
            else if (dbColumn.IsDateTime()) dbColumn.UiProps.ValidationRule = "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)";
            else if (dbColumn.IsDate()) dbColumn.UiProps.ValidationRule = "d(1900-01-01,2100-12-30)";

            if (dbColumn.IsAuditing()) dbColumn.UiProps.Group = "Auditing";
        }
		
	}
}

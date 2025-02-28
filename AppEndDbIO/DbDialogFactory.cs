using AppEndCommon;
using AppEndDynaCode;
using System.Data;
using System.Security.AccessControl;

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

        


        public void CreateNewUpdateByKey(string objectName, string readByKeyApiName, List<string> columnsToUpdate, string partialUpdateApiName, string byColumnName, string onColumnName, string historyTableName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            if (columnsToUpdate.Count == 0) throw new AppEndException("YouMustIndicateAtleastOneColumnToCreateUpdateByKeyApi", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ObjectName", objectName)
                    .GetEx();

            DbSchemaUtils dbSchemaUtils = new(DbConfName);
			DbColumn pkCol = dbDialog.GetPk();
            List<string> finalColsForNewUpdateByKeyApi = [];
			if (!columnsToUpdate.Contains(pkCol.Name)) finalColsForNewUpdateByKeyApi.Add(pkCol.Name);
            finalColsForNewUpdateByKeyApi.AddRange(columnsToUpdate);

            if (!byColumnName.IsNullOrEmpty()) //  create UpdatedBy column if it is not empty
            {
                DbColumn? byDbCol = dbDialog.Columns.FirstOrDefault(i => i.Name == byColumnName);
                if (byDbCol is null)
                {
                    dbSchemaUtils.CreateColumn(objectName, byColumnName, "INT", true);
                    byDbCol = new DbColumn(byColumnName) { DbType = "INT", AllowNull = true, UiProps = new() { } };
                    dbDialog.Columns.Add(byDbCol);
                }
                if (!finalColsForNewUpdateByKeyApi.Contains(byColumnName)) finalColsForNewUpdateByKeyApi.Add(byColumnName);
            }

            if (!onColumnName.IsNullOrEmpty()) //  create UpdatedOn column if it is not empty
			{
                DbColumn? onDbCol = dbDialog.Columns.FirstOrDefault(i => i.Name == onColumnName);
                if (onDbCol is null)
                {
                    dbSchemaUtils.CreateColumn(objectName, onColumnName, "DATETIME", true);
                    onDbCol = new DbColumn(onColumnName) { DbType = "DATETIME", AllowNull = true, UiProps = new() { } };
                    dbDialog.Columns.Add(onDbCol);
                }
				if (!finalColsForNewUpdateByKeyApi.Contains(onColumnName)) finalColsForNewUpdateByKeyApi.Add(onColumnName);
            }


            // remove columns from UpdateByKey query
            DbQuery? mainUpdateByKeyQ = dbDialog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase("UpdateByKey"));
            if (mainUpdateByKeyQ is not null)
            {
                foreach (string s in columnsToUpdate)
                {
                    DbQueryColumn? qCol = mainUpdateByKeyQ.Columns?.FirstOrDefault(i => i.Name.EqualsIgnoreCase(s));
                    if (qCol is not null && mainUpdateByKeyQ.Columns is not null) mainUpdateByKeyQ.Columns.Remove(qCol);
                }

                DbQueryColumn? qcBy = mainUpdateByKeyQ.Columns?.FirstOrDefault(i => i.Name.EqualsIgnoreCase(byColumnName));
                if (qcBy is not null && mainUpdateByKeyQ.Columns is not null) mainUpdateByKeyQ.Columns.Remove(qcBy);

                DbQueryColumn? qcOn = mainUpdateByKeyQ.Columns?.FirstOrDefault(i => i.Name.EqualsIgnoreCase(onColumnName));
                if (qcOn is not null && mainUpdateByKeyQ.Columns is not null) mainUpdateByKeyQ.Columns.Remove(qcOn);
            }

            // Create/Alter ReadByKey query
            DbQuery? readByKeyQ = dbDialog.DbQueries.FirstOrDefault(i => i.Name == readByKeyApiName);
            if (readByKeyQ is null) // create the new ReadByKey
            {
				readByKeyQ = GetReadByKeyQuery(dbDialog);
				readByKeyQ.Name = readByKeyApiName;
				dbDialog.DbQueries.Add(readByKeyQ);
				DynaCode.CreateMethod($"{DbConfName}.{objectName}", readByKeyApiName);
			}
			readByKeyQ.Columns ??= [];
            if(!readByKeyApiName.EqualsIgnoreCase(LibSV.ReadByKey))
                readByKeyQ.Columns.RemoveAll(i => !i.Name.EqualsIgnoreCase(pkCol.Name));
			foreach (string s in columnsToUpdate)
				if (readByKeyQ.Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(s)) is null) 
                    readByKeyQ.Columns.Add(new DbQueryColumn() { Name = s });

			// gen/get Partial UpdateByKey query
			DbQuery existingUpdateByKeyQ = GenOrGetUpdateByKeyQuery(dbDialog, partialUpdateApiName, finalColsForNewUpdateByKeyApi, byColumnName, onColumnName);
            if(!dbDialog.DbQueries.Contains(existingUpdateByKeyQ)) dbDialog.DbQueries.Add(existingUpdateByKeyQ);

			// refreshing UpdateGroup
			foreach (string col in finalColsForNewUpdateByKeyApi)
            {
                if(!pkCol.Name.EqualsIgnoreCase(dbDialog.GetColumn(col).Name))
                {
                    dbDialog.GetColumn(col).UpdateGroup = partialUpdateApiName;
                }
            }

            // add ClientUI
            
            var clientUiTuple = GenOrGetClientUI(dbDialog, existingUpdateByKeyQ, readByKeyApiName);
            dbDialog.ClientUIs ??= [];
            if (clientUiTuple.Item2 == false) dbDialog.ClientUIs.Add(clientUiTuple.Item1);

			// save DbDialog
			dbDialog.Save();

            // add related csharp method

            DynaCode.Refresh();
            if (!DynaCode.MethodExist($"{DbConfName}.{objectName}.{partialUpdateApiName}")) 
            {
                DynaCode.CreateMethod($"{DbConfName}.{objectName}", partialUpdateApiName);
            }


            // create log table and related server objects
            if (!historyTableName.IsNullOrEmpty())
            {
                existingUpdateByKeyQ.HistoryTable = historyTableName;
                dbDialog.Save();
                CreateOrAlterHistoryTable(objectName, partialUpdateApiName, historyTableName);
            }
            DynaCode.ReBuild();
        }
        public void CreateOrAlterHistoryTable(string objectName, string updateQueryName, string historyTableName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbTable? historyTable = DbSchemaUtils.GetTables().FirstOrDefault(i => i.Name.EqualsIgnoreCase(historyTableName));

            DbQuery? masterUpdateQ = dbDialog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase(updateQueryName));
            if (masterUpdateQ is null) return;

            DbColumn pk = dbDialog.GetPk();
            DbTable dbTable = new(historyTableName);

            dbTable.Columns.Add(SetAndGetColumnState(historyTable, new("FakeId") { DbType = "INT", AllowNull = false, IsIdentity = true, IdentityStart = "1", IdentityStep = "1", IsPrimaryKey = true }));
            dbTable.Columns.Add(SetAndGetColumnState(historyTable, new("Id") { DbType = pk.DbType, AllowNull = false, Fk = new("", objectName, pk.Name) }));
            dbTable.Columns.Add(SetAndGetColumnState(historyTable, new(LibSV.CreatedBy) { DbType = "INT", Size = null, AllowNull = false }));
            dbTable.Columns.Add(SetAndGetColumnState(historyTable, new(LibSV.CreatedOn) { DbType = "DATETIME", AllowNull = false }));

            if (masterUpdateQ.Columns is null) return;
            foreach (DbQueryColumn dbQueryColumn in masterUpdateQ.Columns)
            {
                if (dbQueryColumn.Name is not null && dbQueryColumn.Name != pk.Name)
                {
                    DbColumn dbColumn = dbDialog.GetColumn(dbQueryColumn.Name);
                    dbTable.Columns.Add(SetAndGetColumnState(historyTable, new(dbColumn.Name) { State = "n", DbType = dbColumn.DbType, Size = dbColumn.Size, AllowNull = dbColumn.AllowNull }));
                }
            }

            DbSchemaUtils.CreateOrAlterTable(dbTable);
            Thread.Sleep(250);
            historyTable = DbSchemaUtils.GetTables().FirstOrDefault(i => i.Name.EqualsIgnoreCase(historyTableName));
            RemoveServerObjectsFor(historyTableName);
            CreateServerObjectsFor(historyTable, false);
        }

        private DbColumnChangeTrackable SetAndGetColumnState(DbTable? dbObject, DbColumnChangeTrackable col)
        {
            if (dbObject is null || dbObject.Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(col.Name)) is null)
            {
                col.State = "n";
            }
            else
            {
                col.State = "u";
            }
            return col;
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
				QueryType.UpdateByKey => GenOrGetUpdateByKeyQuery(dbDialog, methodName),
				_ => throw new AppEndException("QueryTypeNotSupported", System.Reflection.MethodBase.GetCurrentMethod())
										.AddParam("QueryType", queryType)
                                        .GetEx(),
			};
            dbQ.Name = methodName;
			dbDialog.DbQueries.Add(dbQ);

			//add ClientUI
			var clientUITuple = GenOrGetClientUI(dbDialog, dbQ, "ReadByKey");
			dbDialog.ClientUIs ??= [];
			if (clientUITuple.Item2 == false) dbDialog.ClientUIs.Add(clientUITuple.Item1);

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

            if (!dbQuery.HistoryTable.IsNullOrEmpty()) RemoveServerObjectsFor(dbQuery.HistoryTable);

            dbDialog.DbQueries.Remove(dbQuery);
            dbDialog.ClientUIs?.RemoveAll(i => i.FileName.EqualsIgnoreCase(GetClientUIComponentName(DbConfName, objectName, methodName)));
            dbDialog.Save();
            DynaCode.RemoveMethod($"{DbConfName}.{objectName}.{methodName}");
            DynaCode.Refresh();
        }
        public void DuplicateQuery(string objectName, string methodName, string methodCopyName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(s => s.Name == methodName);
            if (dbQuery is null) return;

            string tempString = dbQuery.ToJsonStringByBuiltIn(true, false);
            DbQuery? dbQueryCopy = ExtensionsForJson.TryDeserializeTo<DbQuery>(tempString) ?? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ObjectName", objectName)
                    .AddParam("MethodName", methodName)
                    .GetEx();
            dbQueryCopy.Name = methodCopyName;
            dbDialog.DbQueries.Add(dbQueryCopy);
            dbDialog.Save();
        }
        public void ReCreateMethodJson(DbObject? dbObject, string methodName)
        {
            if(dbObject == null) return;
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, dbObject.Name);
            var theQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == methodName);
            if (theQuery is null) return;

            theQuery = theQuery.Type switch
            {
                QueryType.Create => GetCreateQuery(dbDialog),
                QueryType.ReadByKey => GetReadByKeyQuery(dbDialog),
                QueryType.ReadList => GetReadListQuery(dbDialog, DbDialogFolderPath),
                QueryType.UpdateByKey => GenOrGetUpdateByKeyQuery(dbDialog, methodName),
                QueryType.DeleteByKey => GetDeleteByKeyQuery(dbDialog),
                QueryType.Procedure => GetExecQuery(dbDialog, DbSchemaUtils),
                QueryType.TableFunction => GetSelectForTableFunction(dbDialog, DbSchemaUtils),
                QueryType.ScalarFunction => GetSelectForScalarFunction(dbDialog, DbSchemaUtils),
                _ => throw new AppEndException("QueryTypeNotSupported", System.Reflection.MethodBase.GetCurrentMethod())
                                        .AddParam("QueryType", theQuery.Type)
                                        .GetEx(),
            };
            dbDialog.Save();
        }


        public void CreateServerObjectsFor(DbObject? dbObject, bool? createAdditionalUpdateByKeyQueries = true)
        {
			if (dbObject == null) return;

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
                foreach (DbColumn dbColumn in dbColumns)
                {
                    dbColumn.IsHumanId = dbColumn.ColumnIsForDisplay() ? true : null;
                    dbColumn.IsSortable = dbColumn.ColumnIsSortable() ? true : null;
                    SetUiProps(dbColumn);
                }

                dbDialog.Columns.AddRange(dbColumns);

                dbDialog.Relations = GetRelations(dbDialog, DbSchemaUtils);

                // set moreinfo items
                dbDialog.OpenCreatePlace = OpenningPlace.InlineDialog;
                dbDialog.OpenUpdatePlace = OpenningPlace.InlineDialog;
                dbDialog.ObjectIcon = dbDialog.IsTree() ? "fa-tree" : "fa-list";
                if (dbDialog.GetTreeParentColumnName() != "") dbDialog.ParentColumn = dbDialog.GetTreeParentColumnName();
                if (dbDialog.GetColumnIfExists("Note") is not null) dbDialog.NoteColumn = "Note";
                if (dbDialog.GetColumnIfExists("ViewOrder") is not null) dbDialog.ViewOrderColumn = "ViewOrder";
                if (dbDialog.GetColumnIfExists("UiColor") is not null) dbDialog.UiColorColumn = "UiColor";
                if (dbDialog.GetColumnIfExists("UiIcon") is not null) dbDialog.UiIconColumn = "UiIcon";
            }

            if (dbObject.DbObjectType == DbObjectType.Table)
            {
                dbDialog.DbQueries.Add(GetReadListQuery(dbDialog, DbDialogFolderPath));
                dbDialog.DbQueries.Add(GetCreateQuery(dbDialog));
                dbDialog.DbQueries.Add(GetReadByKeyQuery(dbDialog));
                dbDialog.DbQueries.Add(GenOrGetUpdateByKeyQuery(dbDialog, "UpdateByKey"));
                dbDialog.DbQueries.Add(GetDelete(dbDialog));
                dbDialog.DbQueries.Add(GetDeleteByKeyQuery(dbDialog));

                appEndClass.DbDialogMethods.Add(nameof(QueryType.ReadList));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.Create));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.ReadByKey));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.UpdateByKey));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.Delete));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.DeleteByKey));
            }
            else if (dbObject.DbObjectType == DbObjectType.View)
            {
                dbDialog.DbQueries.Add(GetReadListQuery(dbDialog, DbDialogFolderPath));
                appEndClass.DbDialogMethods.Add(nameof(QueryType.ReadList));
            }


            //else if (dbObject.DbObjectType == DbObjectType.Procedure)
            //{
            //    dbDialog.DbQueries.Add(GetExecQuery(dbDialog, DbSchemaUtils));
            //    appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            //}
            //else if (dbObject.DbObjectType == DbObjectType.TableFunction)
            //{
            //    dbDialog.DbQueries.Add(GetSelectForTableFunction(dbDialog, DbSchemaUtils));
            //    appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            //}
            //else if (dbObject.DbObjectType == DbObjectType.ScalarFunction)
            //{
            //    dbDialog.DbQueries.Add(GetSelectForScalarFunction(dbDialog, DbSchemaUtils));
            //    appEndClass.DbMethods.Add(dbDialog.DbQueries[0].Name);
            //}

            // adding default ClientUIs
            foreach (DbQuery dbQuery in dbDialog.DbQueries)
            {
                if (IsDbQueryTypeSuitableForClientUI(dbQuery.Type))
                {
                    dbDialog.ClientUIs ??= [];
                    var clientUITuple = GenOrGetClientUI(dbDialog, dbQuery, "ReadByKey");
                    if (clientUITuple.Item2 == false) dbDialog.ClientUIs.Add(clientUITuple.Item1);
                }
            }

            dbDialog.Save();

            // generating controller file
            string csharpFileContent = appEndClass.ToCode();
            string csharpFilePath = DbDialog.GetFullFilePath(DbDialogFolderPath, DbConfName, dbObject.Name).Replace(".dbdialog.json", ".cs");
            File.WriteAllText(csharpFilePath, csharpFileContent);

            // generating additional UpdateByKey methods
            //if (dbObject.DbObjectType == DbObjectType.Table && createAdditionalUpdateByKeyQueries == true)
            //{
            //    foreach (DbColumn dbColumn in dbDialog.Columns.Where(i => i.IsPrimaryKey == false && i.IsIdentity == false).ToList())
            //    {
            //        List<string> colsToUpdate = [dbColumn.Name];
            //        DbColumn? dbColBy = dbDialog.Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(dbColumn.Name + SV.UpdatedBy));
            //        DbColumn? dbColOn = dbDialog.Columns.FirstOrDefault(i => i.Name.EqualsIgnoreCase(dbColumn.Name + SV.UpdatedOn));
            //        if (dbColBy is not null || dbColOn is not null)
            //            CreateNewUpdateByKey(dbObject.Name, SV.ReadByKey, colsToUpdate, $"{dbColumn.Name}{SV.Update}", dbColBy is null ? "" : dbColBy.Name, dbColOn is null ? "" : dbColOn.Name, "");
            //    }
            //}
            DynaCode.Refresh();
        }
        public void RemoveServerObjectsFor(string? dbObjectName)
        {
            if (dbObjectName == null) return;
            string dbDialogFilePath = DbDialog.GetFullFilePath(DbDialogFolderPath, DbConfName, dbObjectName);
            string settingsFilePath = dbDialogFilePath.Replace(".dbdialog.json", ".settings.json");
            string csharpFilePath = dbDialogFilePath.Replace(".dbdialog.json", ".cs");
            if (File.Exists(dbDialogFilePath)) { File.Delete(dbDialogFilePath); }
            if (File.Exists(settingsFilePath)) { File.Delete(settingsFilePath); }
            if (File.Exists(csharpFilePath)) { File.Delete(csharpFilePath); }
        }
        public void SyncDbDialog(string objectName)
        {
            DbDialog? dbDialog = DbDialog.TryLoad(DbDialogFolderPath, DbConfName, objectName);
            if (dbDialog == null) return;
            List<DbColumn> dbColumns = DbSchemaUtils.GetTableViewColumns(objectName);


            // add new column
            foreach (DbColumn dbColumn in dbColumns)
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

            foreach (DbColumn dbColumn in toRemove)
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
        public void RemoveRemovedRelationsFromDbQueries(string objectName)
        {
            DbDialog dbDialog = DbDialog.Load(DbDialogFolderPath, DbConfName, objectName);
            int initialDbQueriesCount = dbDialog.DbQueries.Count;
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
                    foreach (string s in toRemove) dbQuery.Relations.Remove(s);
                }
            }
            if (dbDialog.DbQueries.Count != initialDbQueriesCount) dbDialog.Save();
        }

        public void SynchDbDirectMethods()
        {
            string objectName = "DbDirect";
			AppEndClass appEndClass = new(objectName, DbConfName);

			DbSchemaUtils dbSchemaUtils = new(DbConfName);

			List<DbObject> procedures = dbSchemaUtils.GetObjects(DbObjectType.Procedure, "");
			foreach (DbObject o in procedures)
			{
				appEndClass.DbProducerMethods.Add(o.Name, DbParamsToCsharpParams(o.Name));
			}

			List<DbObject> scalarFunctions = dbSchemaUtils.GetObjects(DbObjectType.ScalarFunction, "");
			foreach (DbObject o in scalarFunctions)
			{
				appEndClass.DbScalarFunctionMethods.Add(o.Name, DbParamsToCsharpParams(o.Name));
			}

			List<DbObject> tableFunctions = dbSchemaUtils.GetObjects(DbObjectType.TableFunction, "");
			foreach (DbObject o in tableFunctions)
			{
				appEndClass.DbTableFunctionMethods.Add(o.Name, DbParamsToCsharpParams(o.Name));
			}

			// generating controller file
			string csharpFileContent = appEndClass.ToCode();
			string csharpFilePath = DbDialog.GetFullFilePath(DbDialogFolderPath, DbConfName, objectName).Replace(".dbdialog.json", ".cs");
			File.WriteAllText(csharpFilePath, csharpFileContent);
			DynaCode.Refresh();
		}

        private List<string> DbParamsToCsharpParams(string objectName)
        {
			List<string> inputParams = new List<string>();
			List<DbParam>? dbParams = DbSchemaUtils.GetProceduresFunctionsParameters(objectName);
			if (dbParams != null)
			{
				dbParams = dbParams.Where(i => i.Name != "Returns").ToList();
				if (dbParams.Count > 0)
				{
					foreach (DbParam dbParam in dbParams)
					{
						inputParams.Add(DbIOInstance.DbParamToCSharpInputParam(dbParam));
					}
				}
			}
            return inputParams;
		}

		#region LogicalFk
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
        #endregion

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
        public static Tuple<ClientUI, bool> GenOrGetClientUI(DbDialog dbDialog, DbQuery dbQuery, string readByKeyApiName)
        {
            string fileName = GetClientUIComponentName(dbDialog.DbConfName, dbDialog.ObjectName, dbQuery.Name);
            ClientUI? clientUI = dbDialog.ClientUIs?.FirstOrDefault(i => i.FileName.EqualsIgnoreCase(fileName));
            bool exist = clientUI is not null;
			clientUI ??= new() { TemplateName = GetTemplateName(dbDialog, dbQuery), FileName = fileName };

            if (dbQuery.Type == QueryType.Create) clientUI.LoadAPI = "";
            else if (dbQuery.Type == QueryType.UpdateByKey) clientUI.LoadAPI = readByKeyApiName;
            else clientUI.LoadAPI = dbQuery.Name;

            if (dbQuery.Type == QueryType.Create || dbQuery.Type == QueryType.UpdateByKey) clientUI.SubmitAPI = dbQuery.Name;
            else clientUI.SubmitAPI = "";

            return new Tuple<ClientUI, bool>(clientUI, exist);
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
                    DbQueryColumn dbQueryColumn = new();
					if (col.Name.EqualsIgnoreCase(LibSV.CreatedBy) || col.Name.EqualsIgnoreCase(LibSV.UpdatedBy))
					{
                        dbQueryColumn.As = col.Name;
                        dbQueryColumn.Phrase = "$UserId$";
					}
					if (col.Name.EqualsIgnoreCase(LibSV.CreatedOn) || col.Name.EqualsIgnoreCase(LibSV.UpdatedOn))
					{
						dbQueryColumn.As = col.Name;
						dbQueryColumn.Phrase = "GETDATE()";
					}

                    if (dbQueryColumn.As.IsNullOrEmpty())
                    {
                        dbQueryColumn.Name = col.Name;
                    }

					dbQuery.Columns.Add(dbQueryColumn);
					if (col.Name.EndsWith("_xs"))
						dbQuery.Params.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForImage(col.Name), Size = col.Size, AllowNull = col.AllowNull });
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
        public static DbQuery GenOrGetUpdateByKeyQuery(DbDialog dbDialog, string? UpdateByKeyApiName, List<string>? specificColumns = null, string? byColName = null, string? onColName = null)
        {
            if(UpdateByKeyApiName == null || UpdateByKeyApiName.IsNullOrEmpty()) throw new AppEndException("UpdateApiNameBanNotBeNullOrEmpty", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", dbDialog)
					.GetEx();

			bool isMainUpdateByKey = specificColumns is null || specificColumns.Count == 0 ? true : false;
            DbColumn pkColumn = dbDialog.GetPk();

            DbQuery? existingUpdateByKeyQ = dbDialog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase(UpdateByKeyApiName));
            existingUpdateByKeyQ ??= new(nameof(QueryType.UpdateByKey), QueryType.UpdateByKey) { Columns = [], Params = [] };
            existingUpdateByKeyQ.Name = UpdateByKeyApiName;

			foreach (DbColumn col in dbDialog.Columns)
			{
				if ((isMainUpdateByKey == true && col.ColumnIsForUpdateByKey()) || (isMainUpdateByKey == false && specificColumns.ContainsIgnoreCase(col.Name)))
				{
                    if(existingUpdateByKeyQ.Columns?.FirstOrDefault(c=>c.Name.EqualsIgnoreCase(col.Name)) is null)
                    {
                        DbQueryColumn dbQueryColumn = new();
						if (col.Name.EqualsIgnoreCase(LibSV.CreatedBy) || col.Name.EqualsIgnoreCase(LibSV.UpdatedBy))
						{
							dbQueryColumn.As = col.Name;
							dbQueryColumn.Phrase = "$UserId$";
						}
						if (col.Name.EqualsIgnoreCase(LibSV.CreatedOn) || col.Name.EqualsIgnoreCase(LibSV.UpdatedOn))
						{
							dbQueryColumn.As = col.Name;
							dbQueryColumn.Phrase = "GETDATE()";
						}

						if (dbQueryColumn.As.IsNullOrEmpty())
						{
							dbQueryColumn.Name = col.Name;
						}

						existingUpdateByKeyQ.Columns?.Add(dbQueryColumn);
                        if (col.Name.EndsWith("_xs"))
                            existingUpdateByKeyQ.Params?.Add(new DbParam(col.Name, col.DbType) { ValueSharp = GetValueSharpForImage(col.Name), Size = col.Size, AllowNull = col.AllowNull });
                    }
                }
			}
			existingUpdateByKeyQ.Where = GetByPkWhere(pkColumn, dbDialog);
			if (isMainUpdateByKey == true) existingUpdateByKeyQ.Relations = GetRelationsForDbQueries(existingUpdateByKeyQ, dbDialog.Relations);

			return existingUpdateByKeyQ;
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
        public static string GetClientUIComponentName(string dbConfName, string objectName, string endfixName)
        {
            if(dbConfName.EqualsIgnoreCase(AppEndSettings.DefaultDbConfName)) return $"{objectName}_{endfixName}";
			return $"{dbConfName}_{objectName}_{endfixName}";
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
            else if (!dbColumn.DbType.EqualsIgnoreCase("image") && !dbColumn.IsDateTime() && !dbColumn.IsDate()) 
                dbColumn.UiProps.SearchType = SearchType.Expandable;
            else 
                dbColumn.UiProps.SearchType = SearchType.None;


            if (dbColumn.IsString()) dbColumn.UiProps.ValidationRule = ":=s(0," + dbColumn.Size.FixNullOrEmpty("256") + ")";

            else if (dbColumn.DbType.EqualsIgnoreCase("tinylint")) dbColumn.UiProps.ValidationRule = ":=i(0,255)";
            else if (dbColumn.DbType.EqualsIgnoreCase("smallint")) dbColumn.UiProps.ValidationRule = ":=i(0,32767)";
            else if (dbColumn.DbType.EqualsIgnoreCase("int")) dbColumn.UiProps.ValidationRule = ":=i(0,2147483647)";
            else if (dbColumn.DbType.EqualsIgnoreCase("bigint")) dbColumn.UiProps.ValidationRule = ":=i(0,9223372036854775807)";

            else if (dbColumn.IsDateTime()) dbColumn.UiProps.ValidationRule = "dt(1900-01-01 00:01:00,2100-12-30 11:59:59)";
            else if (dbColumn.IsDate()) dbColumn.UiProps.ValidationRule = "d(1900-01-01,2100-12-30)";

            if (dbColumn.IsAuditing()) dbColumn.UiProps.Group = "Auditing";
        }
		
	}
}

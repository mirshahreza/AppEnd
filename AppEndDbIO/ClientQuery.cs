using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using AppEndCommon;
using Newtonsoft.Json.Linq;

namespace AppEndDbIO
{
	public class ClientQuery : IDisposable
    {
        
        public string QueryFullName { set; get; }
        public List<ClientParam>? Params { set; get; }
        public Where? Where { set; get; }
        public List<OrderClause>? OrderClauses { set; get; }
        public Pagination? Pagination { set; get; }
        public bool AddAggregationsToMainSelect { get; set; } = false;
        public bool IsSubQuery { get; set; } = false;
        public int? SubQueryIndex { get; set; }
        public Dictionary<string, List<List<ClientParam>>>? Relations { set; get; }

        public Containment ColumnsContainment { get; set; } = Containment.IncludeAll;
        public List<string>? ClientIndicatedColumns { get; set; }
        public Containment? AggregationsContainment { get; set; }
        public List<string>? ClientIndicatedAggregations { get; set; }
        public Containment? RelationsContainment { get; set; }
        public List<string>? ClientIndicatedRelations { get; set; }

        private Hashtable? UserContext { get; set; }

        private DbIO dbIO;
        private DbDialog dbDialog;
        private DbQuery dbQuery;
		
		#region Initiation
		public static ClientQuery GetInstanceByQueryName(string queryFullName, Hashtable? userContext = null)
        {
			ClientQuery cq = new() { QueryFullName = queryFullName, UserContext = userContext };
            cq.Init(cq);
            return cq;
        }
        public static ClientQuery GetInstanceByQueryText(string clientQueryText, Hashtable? userContext = null)
        {
            ClientQuery? cq = ExtensionsForJson.TryDeserializeTo<ClientQuery>(clientQueryText);
			return cq == null
				? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("ClientQuery", clientQueryText)
					.GetEx()
				: GetInstanceByQueryObject(cq, userContext);
		}
		public static ClientQuery GetInstanceByQueryObject(ClientQuery clientQuery, Hashtable? userContext = null)
        {
            ClientQuery cq = GetInstanceByQueryName(clientQuery.QueryFullName, userContext);
            cq.Init(clientQuery);
            return cq;
        }
        public static ClientQuery GetInstanceByQueryJson(JsonElement clientQuery, Hashtable? userContext = null)
        {
            ClientQuery? cq = ExtensionsForJson.TryDeserializeTo<ClientQuery>(clientQuery) ?? throw new AppEndException("DeserializeError", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("ClientQuery", clientQuery)
                    .GetEx();
			ClientQuery cq2 = GetInstanceByQueryName(cq.QueryFullName, userContext);
            cq2.Init(cq);
            return cq2;
        }
        private void Init(ClientQuery clientQuery)
        {
            QueryFullName = clientQuery.QueryFullName;
            Params = clientQuery.Params;
            Where = clientQuery.Where;
            OrderClauses = clientQuery.OrderClauses;
            Pagination = clientQuery.Pagination;
            ClientIndicatedColumns = clientQuery.ClientIndicatedColumns;
            ClientIndicatedAggregations = clientQuery.ClientIndicatedAggregations;
            ClientIndicatedRelations = clientQuery.ClientIndicatedRelations;
            ColumnsContainment = clientQuery.ColumnsContainment;
            AggregationsContainment = clientQuery.AggregationsContainment;
            RelationsContainment = clientQuery.RelationsContainment;
            Relations = clientQuery.Relations;

            string[] queryFullNameParts = QueryFullName.Split('.');
            dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, queryFullNameParts[0], queryFullNameParts[1]);
            DbQuery? dbq = dbDialog.DbQueries.FirstOrDefault(i => i.Name == queryFullNameParts[2]) ?? throw new AppEndException("RequestedQueryIsNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("QueryFullName", QueryFullName)
                    .GetEx();
			dbQuery = dbq;
            dbIO = DbIO.Instance(DbConf.FromSettings(queryFullNameParts[0]));
        }
        #endregion


        private string GetCreateStatement()
        {
            if (dbQuery.Columns is null) throw new AppEndException("CanNotInsertWhileThereIsNoColumnSpecified", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
            DbColumn pk = dbDialog.GetPk();
            string stmMain = dbIO.GetSqlTemplate(dbQuery.Type, IsSubQuery);
            string targetTable = GetFinalObjectName();
            string columns = "";
            string values = "";
            string sep = "";
            foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
            {
                if(dbQueryColumn.Name is null) continue;
                columns += $"{sep}{dbQueryColumn.Name}";
                values += $"{sep}@{GetFinalParamName(dbQueryColumn.Name)}";
                sep = ", ";
            }
            stmMain = stmMain
                .Replace("{TargetTable}", targetTable)
                .Replace("{PkTypeSize}", DbUtils.GetTypeSize(pk.DbType, pk.Size))
                .Replace("{Columns}", columns)
                .Replace("{PkName}", pk.Name)
                .Replace("{Values}", values)
                ;


            if (Relations is not null && Relations.Count > 0)
            {
                string subQueries = "";
                foreach (var otm in Relations)
                {
                    DbRelation? dbRelation = dbDialog.Relations?.FirstOrDefault(i => i.RelationTable == otm.Key);
                    if (dbRelation != null)
                    {
                        string createQ = $"{dbDialog.DbConfName}.{otm.Key}.{dbRelation.CreateQuery}";
                        List<List<ClientParam>> rows = otm.Value;
                        int ind = 1;

                        ValidateRelationCount(dbRelation, rows, false);

						foreach (var row in rows)
                        {
                            ClientQuery subCq = GetInstanceByQueryName(createQ, UserContext);
                            subCq.IsSubQuery = true;
                            subCq.SubQueryIndex = ind;
                            subCq.Params = row;
                            string fkParamName = DbUtils.GenParamName(subCq.dbDialog.ObjectName, dbRelation.RelationFkColumn, ind);
                            subQueries += $"{SV.NL}{subCq.GetCreateStatement()}".Replace($"@{fkParamName}", "@MasterId");
                            subCq.PreExec();
                            if (subCq.dbQuery.Params is not null) dbQuery?.FinalDbParameters.AddRange(subCq.dbQuery.FinalDbParameters);
                            Params?.AddRange(subCq.Params);
                            ind++;
                        }
                    }
                }
                stmMain = stmMain.Replace("{SubQueries}", $"{SV.NL}{subQueries}{SV.NL}");
            }

            stmMain = stmMain.Replace("{SubQueries}", "");
            if (!IsSubQuery) stmMain = EncloseByTran(targetTable, stmMain);
            return stmMain;
        }

        private string GetCreateStatementForHistory(List<DbQueryColumn>? columnsToInsert, string masterTable, string masterTablePkName, string masterTablePkParamName)
        {
            if (dbQuery.Columns is null) throw new AppEndException("CanNotInsertWhileThereIsNoColumnSpecified", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
            DbColumn pk = dbDialog.GetPk();
            string stmMain = dbIO.GetSqlTemplate(dbQuery.Type, IsSubQuery);
            string targetTable = GetFinalObjectName();
            string columns = "";
            string values = "";
            string sep = "";
            foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
            {
                if (dbQueryColumn.Name is null) continue;
                columns += $"{sep}{dbQueryColumn.Name}";
                if (columnsToInsert?.FirstOrDefault(i => i.Name.EqualsIgnoreCase(dbQueryColumn.Name)) != null)
                {
                    values += $"{sep}(SELECT TOP 1 T.[{dbQueryColumn.Name}] FROM {masterTable} T WHERE T.[{masterTablePkName}]=@{masterTablePkParamName})";
                }
                else
                {
                    values += $"{sep}@{GetFinalParamName(dbQueryColumn.Name)}";
                }
                sep = ", ";
            }
            stmMain = stmMain
                .Replace("{TargetTable}", targetTable)
                .Replace("{PkTypeSize}", DbUtils.GetTypeSize(pk.DbType, pk.Size))
                .Replace("{Columns}", columns)
                .Replace("{PkName}", pk.Name)
                .Replace("{Values}", values)
                ;

            stmMain = stmMain.Replace("{SubQueries}", "");
            if (!IsSubQuery) stmMain = EncloseByTran(targetTable, stmMain);
            return stmMain;
        }

        private string GetReadByKeyStatement()
        {
            if (dbQuery.Columns is null) throw new AppEndException("CanNotSelectWhileThereIsNoColumnSpecified", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
            AddClientWhereToDbQueryWhere();
            string targetTable = GetFinalObjectName();
            string queryWhere = CompileWhere(dbQuery.Where);
            string order = CompileOrder();
            string pagination = order != "" ? CompilePagination() : "";
            Tuple<string, string, string?> columnsLefts = CompileColumnsToSelect();
            string subQueries = CompileRelationsToSelectForReadByKey();
            Tuple<string, string> aggregationsAggregationsNoCounting = CompileAggregations();

            string finalSql = dbIO.GetSqlTemplate(dbQuery.Type)
                .Replace("{TargetTable}", targetTable)
                .Replace("{Columns}", columnsLefts.Item1)
                .Replace("{Aggregations}", aggregationsAggregationsNoCounting.Item2 != "" ? $", {SV.NL}{aggregationsAggregationsNoCounting.Item2}" : "")
                .Replace("{SubQueries}", subQueries)
                .Replace("{Lefts}", columnsLefts.Item2)
                .Replace("{Where}", queryWhere)
                .Replace("{Order}", order)
                .Replace("{Pagination}", pagination);

			return finalSql;
        }
        private List<string> GetReadListStatement()
        {
            if (dbQuery.Columns is null) throw new AppEndException("CanNotSelectWhileThereIsNoColumnSpecified", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();

			List<string> entierSelect = [];
			AddClientWhereToDbQueryWhere();
            string targetTable = GetFinalObjectName();
            string queryWhere = CompileWhere(dbQuery.Where);
            string order = CompileOrder();
            string pagination = order != "" ? CompilePagination() : "";
            Tuple<string, string, string?> columnsLefts = CompileColumnsToSelect();
            string subQueries = CompileRelationsToSelectForReadList();
            Tuple<string, string> aggsAggsNoCounting = CompileAggregations();
            string aggMain = AddAggregationsToMainSelect && !aggsAggsNoCounting.Item2.IsNullOrEmpty() ? $", {SV.NL}{aggsAggsNoCounting.Item2}" : "";
            string sqlTemplate = dbIO.GetSqlTemplate(dbQuery.Type);

			string mainSelect = sqlTemplate
				.Replace("{TargetTable}", targetTable)
                .Replace("{Columns}", columnsLefts.Item1)
                .Replace("{Aggregations}", aggMain)
                .Replace("{SubQueries}", subQueries)
                .Replace("{Lefts}", columnsLefts.Item2)
                .Replace("{Where}", queryWhere)
                .Replace("{Order}", order)
                .Replace("{Pagination}", pagination);
            entierSelect.Add(mainSelect);

            if (aggsAggsNoCounting.Item1 != "")
            {
                string aggSelect = sqlTemplate
					.Replace("{TargetTable}", targetTable)
                    .Replace("{Columns}", "")
                    .Replace("{Aggregations}", aggsAggsNoCounting.Item1)
                    .Replace("{SubQueries}", "")
                    .Replace("{Lefts}", "")
                    .Replace("{Where}", queryWhere)
                    .Replace("{Order}", "")
                    .Replace("{Pagination}", "");
                entierSelect.Add(aggSelect);
            }

            return entierSelect;
        }
        private string GetAggregatedReadListStatement()
        {
            AddClientWhereToDbQueryWhere();
            string targetTable = GetFinalObjectName();
            string queryWhere = CompileWhere(dbQuery.Where);
            Tuple<string, string, string?> columnsLefts = CompileColumnsToSelect();
            string order = CompileOrder();
            string pagination = order != "" ? CompilePagination() : "";
            Tuple<string, string> aggregationsAggregationsNoCounting = CompileAggregations();
            string groupSql = columnsLefts.Item3 is not null ? dbIO.GetGroupSqlTemplate().Replace("{Groups}", columnsLefts.Item3) : "";
            string sep = columnsLefts.Item1 != "" && aggregationsAggregationsNoCounting.Item1 != "" ? $", {SV.NL}" : "";
            
            string mainSelect = dbIO.GetSqlTemplate(dbQuery.Type)
                .Replace("{TargetTable}", targetTable)
                .Replace("{Columns}", columnsLefts.Item1)
                .Replace("{Aggregations}", sep + aggregationsAggregationsNoCounting.Item1)
                .Replace("{Lefts}", columnsLefts.Item2)
                .Replace("{Where}", queryWhere)
                .Replace("{GroupBy}", groupSql)
                .Replace("{Order}", order)
                .Replace("{Pagination}", pagination);

            return mainSelect;
        }

        private string GetUpdateByKeyStatement()
        {
            if (dbQuery.Columns is null) throw new AppEndException("CanNotUpdateWhileThereIsNoColumnsSpecifiedToUpdate", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
            DbColumn pk = dbDialog.GetPk();
            string pkParamName = GetFinalParamName(pk.Name);
            string stmMain = dbIO.GetSqlTemplate(QueryType.UpdateByKey);
            string targetTable = GetFinalObjectName();
            string sets = "";
            string sep = "";
            foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
            {
                DbParam? dbParam = dbQuery?.Params?.FirstOrDefault(i => i.Name == dbQueryColumn.Name);
                if (dbParam is not null && dbQueryColumn.Name != pk.Name && dbQueryColumn.Name is not null)
                {
                    sets += $"{sep}{DbUtils.GetSetColumnParamPair(GetFinalObjectName(), dbQueryColumn.Name, SubQueryIndex)}";
                    sep = ", ";
                }
            }
            string where = CompileWhere(dbQuery?.Where);
            stmMain = stmMain.Replace("{TargetTable}", targetTable).Replace("{Sets}", sets).Replace("{Where}", where);

            string subQueries = "";
            string preQueries = "";

            if (dbQuery is not null && Relations is not null && Relations.Count > 0 && dbQuery?.Relations is not null && dbQuery.Relations.Count > 0)
            {
                foreach (var otm in Relations)
                {
                    DbRelation? dbRelation = dbDialog.Relations?.FirstOrDefault(i => i.RelationTable == otm.Key);

					if (dbRelation is not null)
                    {
						string? dbQueryRelation = dbQuery?.Relations?.FirstOrDefault(i => i == dbRelation.RelationName);
						if(dbQueryRelation is not null)
                        {
	                        string createQ = $"{dbDialog.DbConfName}.{otm.Key}.{dbRelation.CreateQuery}";
							string UpdateQ = $"{dbDialog.DbConfName}.{otm.Key}.{dbRelation.UpdateByKeyQuery}";
							string deleteQ = $"{dbDialog.DbConfName}.{otm.Key}.{dbRelation.DeleteByKeyQuery}";
							List<List<ClientParam>> rows = otm.Value;
							int ind = 1;

                            ValidateRelationCount(dbRelation, rows, true);

							foreach (var row in rows)
							{
								string theQ = "";
								ClientParam? flag = row.FirstOrDefault(i => i.Name == "_flag_");
								if (flag is null || flag.Value is null || flag.Value.ToStringEmpty() == "" || flag.Value.ToStringEmpty() == "c") theQ = createQ;
								else if (flag.Value is not null)
								{
									string f = flag.Value.ToStringEmpty();
									if (f == "u") theQ = UpdateQ;
									if (f == "d") theQ = deleteQ;
								}
								if (theQ != "")
								{
									var subCq = GetInstanceByQueryName(theQ, UserContext);
									subCq.IsSubQuery = true;
									subCq.SubQueryIndex = ind;
									subCq.Params = row;
									subCq.PreExec();

									string fkParamName = DbUtils.GenParamName(subCq.dbDialog.ObjectName, dbRelation.RelationFkColumn, ind);
									if (subCq.dbQuery.Type == QueryType.Create)
									{
										subQueries += $"{SV.NL}{subCq.GetCreateStatement()}".Replace($"@{fkParamName}", $"@{pkParamName}");
									}
									else if (subCq.dbQuery.Type == QueryType.UpdateByKey)
									{
										string subCqpkName = DbUtils.GenParamName(subCq.GetFinalObjectName(), subCq.dbDialog.GetPk().Name, null);
										subQueries += $"{SV.NL}{subCq.GetUpdateByKeyStatement()}".Replace($"@{subCqpkName}", $"@{subCqpkName}_{ind}");
									}
									else
									{
										string subCqpkName = DbUtils.GenParamName(subCq.GetFinalObjectName(), subCq.dbDialog.GetPk().Name, null);
										subQueries += $"{SV.NL}{subCq.GetDeleteByKeyStatement()}".Replace($"@{subCqpkName}", $"@{subCqpkName}_{ind}");
									}

									dbQuery?.FinalDbParameters.AddRange(subCq.dbQuery.FinalDbParameters);
									Params?.AddRange(subCq.Params);
									ind++;
								}
							}
						}
                        
                    }
                }
            }

            if(dbQuery is not null && dbQuery.Params is not null && dbQuery.HistoryTable is not null && dbQuery.HistoryTable!="")
            {
                DbDialog dbDialogLog = DbDialog.Load(dbDialog.GetDbDialogFolder(), dbDialog.DbConfName, dbQuery.HistoryTable);
                DbQuery? qInsertHistory = dbDialogLog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase(nameof(QueryType.Create)));
                if (qInsertHistory is not null)
                {
                    ClientQuery clientQueryCreateLog = GetInstanceByQueryName($"{dbDialog.DbConfName}.{dbQuery.HistoryTable}.{QueryType.Create}", UserContext);
                    clientQueryCreateLog.IsSubQuery = true;
                    string masterIdParamNameInHistoty = DbUtils.GenParamName(dbQuery.HistoryTable, pk.Name);
                    string masterIdParamName = DbUtils.GenParamName(dbDialog.ObjectName, pk.Name);
                    List<string>? qParams = qInsertHistory.Params?.Select(i => i.Name).ToList();
                    List<DbQueryColumn>? columnsToInsert = qInsertHistory.Columns?.Where(i => qParams?.ContainsIgnoreCase(i.Name) == false).ToList();
                    preQueries = $"{SV.NL}{clientQueryCreateLog.GetCreateStatementForHistory(columnsToInsert, dbDialog.ObjectName,pk.Name,pkParamName)}{SV.NL}";
                    clientQueryCreateLog.Params = [];
                    foreach (var p in dbQuery.Params) clientQueryCreateLog.Params.Add(new(p.Name, p.Value));
                    clientQueryCreateLog.PreExec();
                    dbQuery?.FinalDbParameters.AddRange(clientQueryCreateLog.dbQuery.FinalDbParameters);                    
                }
            }

            stmMain = stmMain.Replace("{PreQueries}", $"{SV.NL}{preQueries}{SV.NL}");
            stmMain = stmMain.Replace("{SubQueries}", $"{SV.NL}{subQueries}{SV.NL}");
            if (!IsSubQuery) stmMain = EncloseByTran(targetTable, stmMain);
            return stmMain;
        }

        private void ValidateRelationCount(DbRelation dbRelation, List<List<ClientParam>> rows, bool checkForDeletedItems)
        {
            int finalRowsCount = checkForDeletedItems == true ? rows.Count - rows.Where(i => i.FirstOrDefault(cp => cp.Name == "_flag_")?.Value?.ToStringEmpty() == "d").Count() : rows.Count;
            int minN = dbRelation.MinN.ToIntSafe();
            int maxN = dbRelation.MaxN.ToIntSafe();

            if (minN != -1 && finalRowsCount < minN)
                throw new AppEndException("MinimumRelationCountError", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .AddParam("Relation", dbRelation.RelationTable)
                    .AddParam("MinN", minN)
                    .AddParam("Rows", finalRowsCount)
                    .GetEx();

            if (maxN != -1 && finalRowsCount > maxN)
                throw new AppEndException("MaximumRelationCountError", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .AddParam("Relation", dbRelation.RelationTable)
                    .AddParam("MinN", maxN)
                    .AddParam("Rows", finalRowsCount)
                    .GetEx();
        }

		private string GetDeleteByKeyStatement()
        {
            DbColumn pk = dbDialog.GetPk();
            ClientParam? pkParam = (Params?.FirstOrDefault(i => i.Name == pk.Name)) ?? throw new AppEndException("DeleteByKeyMustContainsPrimaryKeyParameter", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
			string stmMain = dbIO.GetSqlTemplate(QueryType.DeleteByKey);
            string targetTable = GetFinalObjectName();
            string where = CompileWhere(dbQuery.Where);
            stmMain = stmMain.Replace("{TargetTable}", targetTable).Replace("{Where}", where);
            
            if (dbQuery.Relations is not null && dbQuery.Relations.Count > 0)
            {
                string subQueries = "";
                foreach (var otmName in dbQuery.Relations)
                {
                    DbRelation otm = dbDialog.GetRelation(otmName);
                    DbDialog? dbDialogRelationTable = DbDialog.TryLoad(AppEndSettings.ServerObjectsPath, dbDialog.DbConfName, otm.RelationTable);
                    if(dbDialogRelationTable != null)
                    {
                        DbColumn dbColumnRelationFk = dbDialogRelationTable.GetColumn(otm.RelationFkColumn);
                        if (dbColumnRelationFk.Fk is null || dbColumnRelationFk.Fk.EnforceRelation != true)
                        {
	                        string deleteQ = $"{dbDialog.DbConfName}.{otm.RelationTable}.{otm.DeleteQuery}";
							var subCq = GetInstanceByQueryName(deleteQ, UserContext);
							subCq.IsSubQuery = true;
							subCq.Params = [new ClientParam(otm.RelationFkColumn, pkParam.Value)];
							subCq.dbQuery.Where = new()
							{
								ConjunctiveOperator = ConjunctiveOperator.AND,
								CompareClauses = [new CompareClause(otm.RelationFkColumn, pkParam.Value)]
							};
							subCq.PreExec();
							subQueries += $"{SV.NL}{subCq.GetDeleteStatement()}";
							dbQuery?.FinalDbParameters.AddRange(subCq.dbQuery.FinalDbParameters);
						}
					}
				}
                stmMain = stmMain.Replace("{SubQueries}", $"{SV.NL}{subQueries}{SV.NL}");
			}

			stmMain = stmMain.Replace("{SubQueries}", "");
			if (!IsSubQuery) stmMain = EncloseByTran(targetTable, stmMain);
            return stmMain;
        }

        private string GetDeleteStatement()
        {
            string stmMain = dbIO.GetSqlTemplate(QueryType.Delete);
            string where = CompileWhere(dbQuery.Where);
            stmMain = stmMain.Replace("{TargetTable}", GetFinalObjectName()).Replace("{Where}", where);
            return stmMain;
        }

        private string GetProcedureStatement()
        {
            string stm = dbIO.GetSqlTemplate(dbQuery.Type);
            string prms = "";
            string sep = "";
            if(dbQuery.Params is not null)
            {
                foreach (DbParam p in dbQuery.Params)
                {
                    prms += $"{sep}@{p.Name}=@{GetFinalParamName(p.Name)}";
                    sep = ", ";
                }
            }
            return stm.Replace("{StoredProcedureName}", GetFinalObjectName()).Replace("{InputParams}", prms);
        }
        private string GetTableScalerFunctionStatement()
        {
            string stm = dbIO.GetSqlTemplate(dbQuery.Type);
            string prms = "";
            string sep = "";
            if (dbQuery.Params is not null)
            {
                foreach (DbParam p in dbQuery.Params)
                {
                    prms += $"{sep}@{GetFinalParamName(p.Name)}";
                    sep = ", ";
                }
            }
            return stm.Replace("{FunctionName}", GetFinalObjectName()).Replace("{InputParams}", prms);
        }

        private Tuple<string, string, string?> CompileColumnsToSelect()
        {
            string sep = "";
            string columns = "";
            string lefts = "";
            string? groupColumns = dbQuery.Type == QueryType.AggregatedReadList ? "" : null;
            string targetTable = GetFinalObjectName();
            List<DbQueryColumn>? columnsToDo;
            if (ColumnsContainment == Containment.ExcludeIndicatedItems)
            {
                columnsToDo = ClientIndicatedColumns == null ? dbQuery.Columns : dbQuery.Columns?.Where(i => !ClientIndicatedColumns.ContainsIgnoreCase(i.Name)).ToList();
            }
            else if (ColumnsContainment == Containment.IncludeIndicatedItems)
            {
                columnsToDo = ClientIndicatedColumns == null ? dbQuery.Columns : dbQuery.Columns?.Where(i => ClientIndicatedColumns.ContainsIgnoreCase(i.Name)).ToList();
            }
            else 
            {
                columnsToDo = dbQuery.Columns;
            }
            if (columnsToDo is not null)
            {
                foreach (DbQueryColumn dbQueryColumn in columnsToDo)
                {
                    string cc = CompileDbQueryColumn(dbQueryColumn, targetTable);
                    columns += sep + cc;
                    if (groupColumns is not null) groupColumns += sep + (cc.ContainsIgnoreCase(" AS ") ? cc.Split(SV.AsStr, StringSplitOptions.None)[0] : cc);
                    if (dbQueryColumn.RefTo is not null)
                    {
                        if (dbQueryColumn.Name is null) throw new AppEndException("LeftColumnNameCanNotBeUnknown", System.Reflection.MethodBase.GetCurrentMethod())
                                .AddParam("Query", QueryFullName)
                                .GetEx();
                        Tuple<string, string> left = CompileRefTo(targetTable, dbQueryColumn.Name, dbQueryColumn.RefTo);
                        if (groupColumns is not null) groupColumns += ", " + left.Item1.Split(SV.AsStr, StringSplitOptions.None)[0];
                        columns += ", " + left.Item1;
                        lefts += left.Item2;
                    }
                    sep = ", ";
                }
            }
            return new Tuple<string, string, string?>(columns, lefts, groupColumns);
        }
        private string CompileRelationsToSelectForReadList()
        {
            if (dbQuery.Relations is not null && dbQuery.Relations.Count > 0)
            {
                List<string>? relationsToDo;
                if (RelationsContainment == Containment.ExcludeIndicatedItems)
                {
                    relationsToDo = ClientIndicatedRelations == null ?
                        dbQuery.Relations :
                        dbQuery.Relations?.Where(i => !ClientIndicatedRelations.ContainsIgnoreCase(i)).ToList();
                }
                else if (RelationsContainment == Containment.IncludeIndicatedItems)
                {
                    relationsToDo = ClientIndicatedRelations == null ?
                        dbQuery.Relations :
                        dbQuery.Relations?.Where(i => ClientIndicatedRelations.ContainsIgnoreCase(i)).ToList();
                }
                else if (RelationsContainment == Containment.ExcludeAll)
                {
                    relationsToDo = null;
                }
                else
                {
                    relationsToDo = dbQuery.Relations;
                }

                if (relationsToDo is not null)
                {
                    string targetTable = GetFinalObjectName();
                    string sep2 = $"{SV.NL},{SV.NL}";
                    string subQueries = "";
                    foreach (string otmName in relationsToDo)
                    {
                        DbRelation dbRelation = dbDialog.GetRelation(otmName);
                        if (dbRelation.RelationType == RelationType.ManyToMany && dbRelation.LinkingTargetTable is not null && dbRelation.LinkingColumnInManyToMany is not null)
                        {
                            string linkingTargetTable = dbRelation.LinkingTargetTable;
                            string linkingColumnInMTM = dbRelation.LinkingColumnInManyToMany;
                            DbDialog dbDialogLinkingTargetTable = DbDialog.Load(dbDialog.GetDbDialogFolder(), dbDialog.DbConfName, linkingTargetTable);
                            string linkingTargetTablePkName = dbDialogLinkingTargetTable.GetPk().Name;
                            string[] hIds = dbDialogLinkingTargetTable.GetHumanIds().Split(",");
                            string cols = "";
                            string stm = dbIO.GetSqlTemplate(QueryType.ReadList, true);
                            string sep = "";
                            foreach(string hId in hIds)
                            {
                                cols += sep + $"[{linkingTargetTable}].[{hId}]";
                                sep = ", ";
                            }

							//cols = $"[{linkingTargetTable}].[Id]," + cols;
							DbColumn pk = dbDialog.GetPk();
							string left = $"LEFT OUTER JOIN [{linkingTargetTable}] ON [{linkingTargetTable}].[{linkingTargetTablePkName}]=[{dbRelation.RelationTable}].[{linkingColumnInMTM}]";
                            stm = stm.Replace("{Columns}", cols);
                            stm = stm.Replace("{TargetTable}", dbRelation.RelationTable);
                            stm = stm.Replace("{Lefts}", left);
                            stm = stm.Replace("{Where}", $"WHERE [{dbRelation.RelationTable}].[{dbRelation.RelationFkColumn}]=[{targetTable}].[{pk.Name}]");
                            stm = stm.Replace("{Order}", "");
                            subQueries += $"{sep2}({SV.NL}{stm}{SV.NL}) AS {otmName}";
                        }
                    }
                    return subQueries;
                }
            }
            return "";
        }
        private string CompileRelationsToSelectForReadByKey()
        {
            DbColumn pk = dbDialog.GetPk();
            if (dbQuery.Relations is not null && dbQuery.Relations.Count > 0)
            {
                List<string>? relationsToDo;
                if (RelationsContainment == Containment.ExcludeIndicatedItems)
                {
                    relationsToDo = ClientIndicatedRelations == null ?
                        dbQuery.Relations :
                        dbQuery.Relations?.Where(i => !ClientIndicatedRelations.ContainsIgnoreCase(i)).ToList();
                }
                else if (RelationsContainment == Containment.IncludeIndicatedItems)
                {
                    relationsToDo = ClientIndicatedRelations == null ?
                        dbQuery.Relations :
                        dbQuery.Relations?.Where(i => ClientIndicatedRelations.ContainsIgnoreCase(i)).ToList();
                }
                else if (RelationsContainment == Containment.ExcludeAll)
                {
                    relationsToDo = null;
                }
                else
                {
                    relationsToDo = dbQuery.Relations;
                }

                if (relationsToDo is not null)
                {
                    string targetTable = GetFinalObjectName();
                    string sep = $"{SV.NL},{SV.NL}";
                    string subQueries = "";
                    foreach (string otmName in relationsToDo)
                    {
                        DbRelation dbRelation = dbDialog.GetRelation(otmName);
                        string targetDbDialog = $"{dbDialog.DbConfName}.{dbRelation.RelationTable}.{dbRelation.ReadListQuery}";
						
                        if (DbDialog.Exist(AppEndSettings.ServerObjectsPath, dbDialog.DbConfName, dbRelation.RelationTable))
                        {
							ClientQuery subQuery = GetInstanceByQueryName(targetDbDialog, UserContext);
							subQuery.RelationsContainment = Containment.ExcludeAll;
							subQuery.IsSubQuery = true;
							subQuery.Where = new() { SimpleClauses = [new($"[{dbRelation.RelationTable}].[{dbRelation.RelationFkColumn}]=[TARGETTABLE].[{pk.Name}]")] };

                            string sq = $"{sep}({SV.NL}{subQuery.GetReadListStatement()[0]}{SV.NL}) AS {otmName}".Replace(";", " FOR JSON PATH");

                            sq = sq.Replace("FROM [" + dbRelation.RelationTable + "]", "FROM [" + dbRelation.RelationTable + "] T");
                            sq = sq.Replace("[" + dbRelation.RelationTable + "].", "T.");
                            sq = sq.Replace("[TARGETTABLE].", $"[{targetTable}].");


							subQueries += sq;
						}
					}
                    return subQueries;
                }
            }
            return "";
        }
        private Tuple<string,string> CompileAggregations()
        {
            string aggregationColumns = "";
            string aggregationColumnsNoCount = "";
            if (dbQuery.Aggregations is not null && dbQuery.Aggregations.Count > 0 && AggregationsContainment!= Containment.ExcludeAll)
            {
                string sep = "";
                List<DbAggregation>? aggregationsToDo;

                if (AggregationsContainment == Containment.ExcludeIndicatedItems)
                {
                    aggregationsToDo = ClientIndicatedAggregations == null ? 
                        dbQuery.Aggregations : 
                        dbQuery.Aggregations?.Where(i => !ClientIndicatedAggregations.ContainsIgnoreCase(i.Name)).ToList();
                }
                else if (AggregationsContainment == Containment.IncludeIndicatedItems)
                {
                    aggregationsToDo = ClientIndicatedAggregations == null ? 
                        dbQuery.Aggregations : 
                        dbQuery.Aggregations?.Where(i => ClientIndicatedAggregations.ContainsIgnoreCase(i.Name)).ToList();
                }
                else if (AggregationsContainment == Containment.ExcludeAll)
                {
                    aggregationsToDo = null;
                }
                else
                {
                    aggregationsToDo = dbQuery.Aggregations;
                }

                List<DbAggregation>? aggregationsToDoNoCount = aggregationsToDo?.Where(i => !i.Name.EqualsIgnoreCase("count")).ToList();

                if (aggregationsToDo is not null)
                {
                    sep = "";
                    foreach (var aggregation in aggregationsToDo)
                    {
                        aggregationColumns += sep + $"({aggregation.Phrase}) AS {aggregation.Name}";
                        sep = ", ";
                    }
                }
                if (aggregationsToDoNoCount is not null)
                {
                    sep = "";
                    foreach (var aggregation in aggregationsToDoNoCount)
                    {
                        aggregationColumnsNoCount += sep + $"({aggregation.Phrase}) AS {aggregation.Name}";
                        sep = ", ";
                    }
                }
            }
            return new Tuple<string, string>(aggregationColumns, aggregationColumnsNoCount);
        }
        private string CompilePagination()
        {
            Pagination ??= new();
            return dbIO.GetPaginationSqlTemplate().Replace("{PageIndex}", ((Pagination.PageNumber - 1) * Pagination.PageSize).ToString()).Replace("{PageSize}", Pagination.PageSize.ToString());
        }
        private string CompileOrder()
        {
            string orderTemplate = dbIO.GetOrderSqlTemplate();
            if (OrderClauses is not null)
            {
                string clauses = "";
                string sep = "";
                foreach (var clause in OrderClauses)
                {
                    string n = clause.Name;
                    if (!n.ContainsIgnoreCase(".") && !n.ContainsIgnoreCase("[") && !n.ContainsIgnoreCase("("))
                    {
                        n = $"[{GetFinalObjectName()}].[{n}]";
                    }
                    clauses += $"{sep}{n} {clause.OrderDirection}";
                    sep = ", ";
                }
                return orderTemplate.Replace("{Orders}", clauses);
            }
            else
            {
                if (dbQuery.Type == QueryType.AggregatedReadList)
                {
                    DbQueryColumn? c = dbQuery.Columns?.FirstOrDefault();
                    if (c != null) return orderTemplate.Replace("{Orders}", $"[{GetFinalObjectName()}].[{c.Name}]") ;
                    else
                    {
                        DbAggregation? agg = dbQuery.Aggregations?.FirstOrDefault();
                        if (agg != null) return orderTemplate.Replace("{Orders}", agg.Phrase);
                        else return "";
                    }
                }
                else
                {
                    DbColumn pk = dbDialog.GetPk();
                    return orderTemplate.Replace("{Orders}", $"[{GetFinalObjectName()}].[{pk.Name}]") ;
                }
            }
        }
        private Tuple<string, string> CompileRefTo(string mainTable, string mainColumn, DbRefTo dbRefTo)
        {
            string sep = "";
            string columnsString = "";
            string targetTableAs = $"{dbRefTo.TargetTable}_{mainColumn}";
            string joinString = dbIO.GetLeftJoinSqlTemplate()
                .Replace("{TargetTable}", dbRefTo.TargetTable)
                .Replace("{TargetTableAs}", targetTableAs)
                .Replace("{TargetColumn}", dbRefTo.TargetColumn)
                .Replace("{MainTable}", mainTable)
                .Replace("{MainColumn}", mainColumn);

            foreach (DbQueryColumn leftField in dbRefTo.Columns)
            {
                columnsString += $"{sep}{SV.NL}{SV.NT}[{targetTableAs}].[{leftField.Name}]{(leftField.As != null ? $" AS [{leftField.As}]" : "")}";
                sep = ", ";
                if (leftField.RefTo != null)
                {
                    if (leftField.Name is null) throw new AppEndException("LeftColumnNameCanNotBeUnknown", System.Reflection.MethodBase.GetCurrentMethod()).GetEx();
                    Tuple<string, string> translated = CompileRefTo(targetTableAs, leftField.Name, leftField.RefTo);
                    columnsString += $", {translated.Item1}";
                    joinString += translated.Item2;
                }
            }
            return new Tuple<string, string>(columnsString, joinString);
        }
        private string CompileDbQueryColumn(DbQueryColumn dbQueryColumn, string? targetTable)
        {
            string? s = (dbQueryColumn.Name is not null ? $"[{targetTable}].[{dbQueryColumn.Name}]" : $"({dbQueryColumn.Phrase})") ?? throw new AppEndException("NameAndPhraseCanNotBeUnknownTogether", System.Reflection.MethodBase.GetCurrentMethod())
                    .AddParam("Query", QueryFullName)
                    .GetEx();
			if (dbQueryColumn.As != null)
            {
                s = $"{s} AS {dbQueryColumn.As}";
            }
            return s;
        }
        private string CompileWhere(Where? dbWhere, int indent = 1)
        {
            if (dbWhere is null) return "";
            string compiledWhere = "";
            string connector = dbWhere.ConjunctiveOperator.ToString();
            string andOr = "";

            if (dbWhere.SimpleClauses is not null)
            {
                foreach (var compiledSimpleRule in dbWhere.SimpleClauses)
                {
                    if (compiledSimpleRule.Phrase.Trim() != "")
                    {
                        compiledWhere += $"{andOr}{SV.NL}{compiledSimpleRule.Phrase} ";
                        andOr = $" {connector} ";
                    }
                }
            }
            if(dbWhere.CompareClauses is not null)
            {
                dbQuery.Params ??= [];
                foreach (CompareClause wcc in dbWhere.CompareClauses)
                {
                    string dbParamName = StaticMethods.GetUniqueName(wcc.Name);
                    string dbType = "";

					List<CompareOperator> nullOrNotComps = [CompareOperator.IsNull, CompareOperator.IsNotNull];
					List<CompareOperator> inOrNotComps = [CompareOperator.In, CompareOperator.NotIn];

					string columnFullName = $"[{GetFinalObjectName()}].[{wcc.Name}]";
					if (!nullOrNotComps.Contains(wcc.CompareOperator) && !inOrNotComps.Contains(wcc.CompareOperator))
                    {
						DbColumn? dbColumn = dbDialog.Columns.FirstOrDefault(c => c.Name == wcc.Name);
						if (dbColumn != null)
						{
							DbParam dbParam = new(dbParamName, dbColumn.DbType) { Value = wcc.Value?.ToString() };
							dbType = dbColumn.DbType;
							dbQuery.FinalDbParameters.Add(ToDbParameter(dbParam));
						}
                        compiledWhere += $"{andOr}{SV.NL}{dbIO.CompileWhereCompareClause(wcc, GetFinalObjectName(), columnFullName, dbParamName, dbType)}";
                        andOr = $" {connector} ";
                    }
					else if (inOrNotComps.Contains(wcc.CompareOperator))
					{
						if (wcc.Value?.ToStringEmpty() != "[]")
						{
                            string tempComp = dbIO.CompileWhereCompareClause(wcc, GetFinalObjectName(), columnFullName, dbParamName, dbType);
                            tempComp = tempComp.Replace($"@{DbUtils.GenParamName(GetFinalObjectName(), dbParamName, null)}", wcc.Value?.ToStringEmpty().Replace("[", "(").Replace("]", ")"));
                            compiledWhere += $"{andOr}{SV.NL}{tempComp}";
							andOr = $" {connector} ";
						}
					}
					else if (nullOrNotComps.Contains(wcc.CompareOperator))
					{
						compiledWhere += $"{andOr}{SV.NL}{dbIO.CompileWhereCompareClause(wcc, GetFinalObjectName(), columnFullName, dbParamName, dbType)}";
						andOr = $" {connector} ";
					}
				}
			}

            if (dbWhere.ComplexClauses is not null)
            {
                foreach (Where whereClauseComplex in dbWhere.ComplexClauses)
                {
                    string subWhere = CompileWhere(whereClauseComplex, indent + 1);
                    if (subWhere.Trim() != "")
                    {
                        string ind = SV.NT.RepeatN(indent);
                        compiledWhere += $"{SV.NL}{ind}{andOr} ({subWhere}{SV.NL}{ind})";
                        andOr = $" {connector} ";
                    }
                }
            }

            if (indent == 1 && compiledWhere.Trim() != "") compiledWhere = $"{SV.NL}WHERE{SV.NL}{SV.NT}{compiledWhere}";

            return compiledWhere;
        }


        private List<DbParameter>? ToDbParameters(List<DbParam>? dbParams)
        {
            if (dbParams is null) return null;
            List<DbParameter> dbParameters = [];
            foreach (DbParam dbParam in dbParams)
            {
                dbParameters.Add(ToDbParameter(dbParam));
            }
            return dbParameters;
        }
        private DbParameter ToDbParameter(DbParam dbParam)
        {
            try
            {
				int? size = dbParam.Size is null ? null : int.Parse(dbParam.Size);
				if (dbParam.DbType.EqualsIgnoreCase("bit") && dbParam.Value is not null && dbParam.Value.ToString() == "") dbParam.Value = DBNull.Value;
				DbParameter dbParameter = dbIO.CreateParameter(GetFinalParamName(dbParam.Name), dbParam.DbType, size, dbParam.Value);
				return dbParameter;
			}
			catch (Exception ex)
            {
				var aeEx = new AppEndException($"SqlStatementError", System.Reflection.MethodBase.GetCurrentMethod())
								.AddParam("Message", ex.Message)
								.AddParam("ParameterName", dbParam.Name)
								.AddParam("ParameterValue", dbParam.Value.ToStringEmpty())
								.AddParam("ParameterValueSharp", dbParam.ValueSharp.ToStringEmpty())
								.GetEx();
                throw aeEx;
			}
        }


        private void AddClientWhereToDbQueryWhere()
        {
            if (Where is not null)
            {
                dbQuery.Where ??= new();
                if (dbQuery.Where.ComplexClauses is null) dbQuery.Where.ComplexClauses = [];
                dbQuery.Where?.ComplexClauses?.Add(Where);
            }
        }
        private string GetFinalObjectName()
        {
            return dbQuery.BaseObjectName is not null ? dbQuery.BaseObjectName : dbDialog.ObjectName;
        }
        private string GetFinalParamName(string columnName)
        {
            return DbUtils.GenParamName(GetFinalObjectName(), columnName, SubQueryIndex);
        }
        private string EncloseByTran(string targetTable,string sqlBody)
        {
            string tranName = $"{StaticMethods.GetRandomName("T_")}_{targetTable}".TruncateTo(30);
            return dbIO.GetTranBlock().Replace("{TranName}", tranName).Replace("{SqlBody}", sqlBody);
        }

        public void PreExec()
        {
            MixParams();
            CalculateSharpParams();
            List<DbParameter>? dbParameters = ToDbParameters(dbQuery.Params);
            if (dbParameters is not null) dbQuery.FinalDbParameters.AddRange(dbParameters);
            if (Params is not null)
            {
                var listDbParams = new List<DbParam>();
				foreach (var p in Params)
				{
                    if (dbQuery.FinalDbParameters.FirstOrDefault(pExist => pExist.ParameterName == GetFinalObjectName() + "_" + p.Name) == null)
                    {
                        listDbParams.Add(new DbParam(p.Name, "NVARCHAR") { Value = p.Value?.ToString() });
                    }
				}
				List<DbParameter>? dbParametersAdditional = ToDbParameters(listDbParams);
				if (dbParametersAdditional is not null) dbQuery.FinalDbParameters.AddRange(dbParametersAdditional);
			}
		}
        private void MixParams()
        {
			dbQuery.Params ??= [];
			if (dbQuery.Columns is not null)
            {
                foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
                {
                    DbParam? dbParam = dbQuery.Params.FirstOrDefault(i => i.Name == dbQueryColumn.Name);
                    if (dbParam is not null) continue;
                    ClientParam? clientParam = Params?.FirstOrDefault(i => i.Name == dbQueryColumn.Name);
					object? v = clientParam?.Value?.ToString();
					DbColumn? dbColumn = dbDialog.Columns.FirstOrDefault(i => i.Name == dbQueryColumn.Name);
                    if (dbColumn is null || dbQueryColumn.Name is null) continue;
					dbQuery.Params.Add(new DbParam(dbQueryColumn.Name, dbColumn.DbType) { Value = v });
                }
            }
			foreach (DbParam dbp in dbQuery.Params)
			{
				if (dbp.ValueSharp is not null) continue;
				ClientParam? clientParam = Params?.FirstOrDefault(i => i.Name == dbp.Name);
				object? vv = clientParam?.Value?.ToString();
				DbColumn? dbColumn = dbDialog.Columns.FirstOrDefault(i => i.Name == dbp.Name);
				if (dbColumn is null) continue;

				if (dbColumn.IsNumerical() && vv is string v && v == "") vv = null;
				if (dbColumn.DbType.EqualsIgnoreCase("image") && vv is string v1 && v1 == "") vv = null;
				if (dbColumn.DbType.EqualsIgnoreCase("image") && vv is not null) vv = Convert.FromBase64String((string)vv);

				dbp.Value = vv;
			}
		}
		private void CalculateSharpParams()
        {
            if (dbQuery.Params is null) return;
            foreach (DbParam dbParam in dbQuery.Params)
            {
                if (dbParam.ValueSharp is not null)
                {
                    object? obj = null;
                    string vs = dbParam.ValueSharp;
                    if (vs == "#Now")
                    {
						obj = DateTime.Now;
					}
                    else if (vs.StartsWith("#Resize"))
                    {
                        string[] s = vs.Replace("#Resize:", "").Trim().Split(',');
                        DbParam? from = dbQuery.Params.FirstOrDefault(i => i.Name == s[0]);
                        if (from is not null && from.Value is not null)
                            obj = ((byte[])from.Value).ResizeImage(int.Parse(s[1]));
                    }
					else if (vs.StartsWith("#ToMD5:") && dbParam.Value is not null)
                    {
                        obj = dbParam.Value.ToString()?.GetMD5Hash();
                    }
					else if (vs.StartsWith("#ToMD4:") && dbParam.Value is not null)
                    {
                        obj = dbParam.Value.ToString()?.GetMD4Hash();
                    }
					else if (vs.StartsWith("#Context:"))
                    {
                        string s = vs.Replace("#Context:", "").Trim();
                        if (UserContext is null || !UserContext.ContainsKey(s)) throw new AppEndException("ExpectedKeyAtUserContextDoesNotExist", System.Reflection.MethodBase.GetCurrentMethod())
                                .AddParam("Query", QueryFullName)
                                .AddParam("Key", s)
                                .GetEx();
                        obj = UserContext[s];
                    }
                    dbParam.Value = obj;
                }
            }
        }
        public object? Exec()
        {
            PreExec();
            object? r = null;
            string s = "";
            try
            {
				switch (dbQuery.Type)
				{
					case QueryType.Unknown:
						break;
					case QueryType.Create:
                        s = ReplaceDollarValues(GetCreateStatement());
						r = dbIO.ToScalar(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.ReadList:
						List<string> statements = GetReadListStatement();
						if (statements.Count == 1)
						{
                            statements[0] = ReplaceDollarValues(statements[0]);
							r = dbIO.ToDataTable(statements[0], dbQuery.FinalDbParameters);
						}
						else
						{
							statements[0] = ReplaceDollarValues(statements[0]);
							statements[1] = ReplaceDollarValues(statements[1]);
							r = dbIO.ToDataSet(string.Join(SV.NL2x, statements), dbQuery.FinalDbParameters, ["Master", "Aggregations"]);
						}
						break;
					case QueryType.AggregatedReadList:
						s = ReplaceDollarValues(GetAggregatedReadListStatement());
						r = dbIO.ToDataTable(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.ReadByKey:
						s = GetReadByKeyStatement();
						r = dbIO.ToDataTable(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.UpdateByKey:
						s = ReplaceDollarValues(GetUpdateByKeyStatement());
						dbIO.ToNoneQuery(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.DeleteByKey:
						s = ReplaceDollarValues(GetDeleteByKeyStatement());
						dbIO.ToNoneQuery(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.Delete:
						break;
					case QueryType.Procedure:
						s = ReplaceDollarValues(GetProcedureStatement());
						r = dbIO.ToDataSet(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.TableFunction:
						s = ReplaceDollarValues(GetTableScalerFunctionStatement());
						r = dbIO.ToDataTable(s, dbQuery.FinalDbParameters);
						break;
					case QueryType.ScalarFunction:
						s = ReplaceDollarValues(GetTableScalerFunctionStatement());
						r = dbIO.ToScalar(s, dbQuery.FinalDbParameters);
						break;
					default:
						break;
				}
				Dispose();
				return r;
			}
			catch (Exception ex)
            {
                var aeEx = new AppEndException($"SqlStatementError", System.Reflection.MethodBase.GetCurrentMethod())
								.AddParam("Message", ex.Message)
								.AddParam("SqlStatement", s)
                                .GetEx();

                Dispose();
                throw aeEx;
            }
        }

        private string ReplaceDollarValues(string s)
        {
            return s
                .Replace("$Method$", QueryFullName.Split('.')[2])
				.Replace("$UserId$", UserContext?["UserId"].ToStringEmpty())
				.Replace("$UserName$", UserContext?["UserName"].ToStringEmpty())
                ;
		}

        public void Dispose()
        {
            dbIO.Dispose();
            GC.SuppressFinalize(this);
        }
	}

    public static class ClientQueryExtensions
    {
        public static object? ParamValue(this ClientQuery query, string ParamName)
        {
            JObject jo = query.ToJsonStringByBuiltIn().ToJObjectByNewtonsoft();
            if (jo == null || jo["Params"] == null || jo["Params"] is not JArray) return null;
            JArray ja = jo["Params"].ToJArray();
            var p = ja.FirstOrDefault(i => ((JObject)i)["Name"].ToStringEmpty().EqualsIgnoreCase(ParamName));
            if (p is null) return null;
            return p["Value"];
        }
    }

}

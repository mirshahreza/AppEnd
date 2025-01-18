using AppEndCommon;
using AppEndDbIO;
using Newtonsoft.Json.Linq;
using RazorEngine;
using RazorEngine.Templating; 
using System.Data;
using Microsoft.AspNetCore.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Html;

namespace AppEndServer
{
    public static class TemplateServices
    {
        public static string RunTemplate(string dbConfName, string objectName, ClientUI clientUi)
        {
            DbDialog dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, dbConfName, objectName);
			BuildInfo buildInfo = new(dbDialog, clientUi);
			string s = CompileTemplate(buildInfo, $"{clientUi.TemplateName}");
			s = FormatAsHtml(s);
			return s;
        }

		public static string CompileTemplate(this BuildInfo buildInfo, string templateName, string dbDialogName, string loadApi, DbQueryColumn dbQueryColumn, string columnName, string renderHint)
		{
			Dictionary<string, object> parameters = new()
			{
				{ "DbDialogName", dbDialogName },
				{ "LoadAPI", loadApi },
				{ "Column", dbQueryColumn },
				{ "RenderHint", renderHint },
				{ "ColumnName", columnName }
			};
			return CompileTemplate(buildInfo, templateName, parameters);
		}

		public static string CompileTemplate(this BuildInfo buildInfo, string templateName, string columnName, string submitApi = "")
		{
			Dictionary<string, object> parameters = new() { { "ColumnName", columnName }, { "SubmitApi", submitApi } };
			return CompileTemplate(buildInfo, templateName, parameters);
		}

		public static string CompileTemplate(this BuildInfo buildInfo, string templateName, Dictionary<string, object>? parameters = null)
        {
            string templateFile = $"{AppEndSettings.ClientObjectsPath}/a..templates/{templateName}.cshtml";
            string templateBody = File.ReadAllText(templateFile);
			templateBody = ReplaceVueAtSign(templateBody);
            if (parameters is not null) buildInfo.Parameters = parameters;
            var result = Engine.Razor.RunCompile(templateBody, Guid.NewGuid().ToString(), typeof(BuildInfo), buildInfo);
			result = ReverseVueAtSign(result);
			return result;
        }

		private static string ReplaceVueAtSign(string s)
		{
			return s.Replace("@change", "AtSignChange").Replace("@click", "AtSignClick").Replace("@keyup.enter", "AtSignEnter");
		}
		private static string ReverseVueAtSign(string s)
		{
			return s.Replace("AtSignChange", "@change").Replace("AtSignClick", "@click").Replace("AtSignEnter", "@keyup.enter");
		}

		public static string GetFirstFileFieldName(this BuildInfo buildInfo, string dbDialogName)
		{
			DbDialog dbd = DbDialog.Load(buildInfo.DbDialog.GetDbDialogFolder(), buildInfo.DbDialog.DbConfName, dbDialogName);

			DbColumn? dbColumn = dbd.GetFirstFileFieldName() ?? throw new AppEndException("DbDialogDoesNotContainFileField", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", dbDialogName)
					.GetEx();

			return dbColumn.Name;
		}

		public static List<string> GetTargetHumanIdsFor(this BuildInfo buildInfo, DbColumn dbColumn)
		{
			if (dbColumn.Fk is null || dbColumn.Fk.Lookup is null) return [];
			string targetTable = dbColumn.Fk.TargetTable;
			DbDialog targetDbDialog = DbDialog.Load(buildInfo.DbDialog.GetDbDialogFolder(), buildInfo.DbDialog.DbConfName, targetTable);
			return targetDbDialog.GetHumanIdsOrig().Select(i=>i.Name).ToList();
		}

        public static string GetTargetPkColumn(this DbDialog dbDialog, DbColumn dbColumn)
        {
            if (dbColumn.Fk is null || dbColumn.Fk.Lookup is null) return "";
            DbDialog targetDbDialog = DbDialog.Load(dbDialog.GetDbDialogFolder(), dbDialog.DbConfName, dbColumn.Fk.TargetTable);
			return targetDbDialog.GetPk().Name;
        }
        public static string GetDisplayColumns(this BuildInfo buildInfo, DbColumn dbColumn, bool isCollectionView = true, string sep = " ")
		{
			if (dbColumn.Fk is null || dbColumn.Fk.Lookup is null) return "";
			string dis = "";
			string sep2 = "";
			string targetTable = dbColumn.Fk.TargetTable;
			DbDialog? targetDbDialog = DbDialog.TryLoad(buildInfo.DbDialog.GetDbDialogFolder(), buildInfo.DbDialog.DbConfName, targetTable);
			if(targetDbDialog is not null)
			{
				if (isCollectionView == true)
				{
					foreach (string hId in targetDbDialog.GetHumanIds().Split(','))
					{
						dis += sep2 + "{{i." + hId.Trim() + "}}";
						sep2 = sep;
					}
				}
				else
				{
					sep2 = sep;
					dis += "{{row." + dbColumn.Name + "}}";
					foreach (string hId in targetDbDialog.GetHumanIds().Split(','))
					{
						dis += sep2 + "{{row." + dbColumn.Name + "_" + hId.Trim() + "}}";
					}
				}
			}
			return dis;
		}
		public static List<DbColumn> GetDisplayDbColumns(this BuildInfo buildInfo, DbColumn dbColumn)
		{
			if (dbColumn.Fk is null || dbColumn.Fk.Lookup is null) return [];
			string targetTable = dbColumn.Fk.TargetTable;
			DbDialog targetDbDialog = DbDialog.Load(buildInfo.DbDialog.GetDbDialogFolder(), buildInfo.DbDialog.DbConfName, targetTable);
			return targetDbDialog.GetHumanIdsOrig();
		}
		public static string GetDisplayColumnsAsVueTitle(this BuildInfo buildInfo, DbColumn dbColumn,string datasetName="row" ,string sep = " ")
		{
			if (dbColumn.Fk is null || dbColumn.Fk.Lookup is null) return "";
			string targetTable = dbColumn.Fk.TargetTable;
			DbDialog targetDbDialog = DbDialog.Load(buildInfo.DbDialog.GetDbDialogFolder(), buildInfo.DbDialog.DbConfName, targetTable);
			List<DbColumn> dbColumns = targetDbDialog.GetHumanIdsOrig();
			string sep2 = "+'" + sep + "'+";
			string res = datasetName +"." + dbColumn.Name;
			foreach(DbColumn dbc in dbColumns)
			{
				res += sep2 + datasetName+"." + dbColumn.Name + "_" + dbc.Name;
			}
			return res;
		}

		public static string GetObjectMoreInfo(this BuildInfo buildInfo)
		{
			DbQuery? dbQueryDeleteByKey = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Type == QueryType.DeleteByKey);

			Dictionary<string, string> values = new()
			{
				{ "ComponentsPath", "/a.Components/" },

				{ "PkColumn", buildInfo.DbDialog.GetPk().Name },
				{ "LoadApi", buildInfo.ClientUI.LoadAPI.IsNullOrEmpty() ? "" : $"{buildInfo.DbDialog.DbConfName}.{buildInfo.DbDialog.ObjectName}.{buildInfo.ClientUI.LoadAPI}" },
				{ "SubmitApi", buildInfo.ClientUI.SubmitAPI.IsNullOrEmpty() ? "" : $"{buildInfo.DbDialog.DbConfName}.{buildInfo.DbDialog.ObjectName}.{buildInfo.ClientUI.SubmitAPI}" },
				
				{ "DeleteByKeyApi", dbQueryDeleteByKey is null ? "" : $"{buildInfo.DbDialog.DbConfName}.{buildInfo.DbDialog.ObjectName}.{dbQueryDeleteByKey.Name}" },
				
				{ "ObjectName", buildInfo.DbDialog.ObjectName },
				{ "ObjectType", buildInfo.DbDialog.ObjectType.ToString() },

				{ "Titles", buildInfo.DbDialog.GetHumanIds() },
				{ "ParentId", buildInfo.DbDialog.GetTreeParentColumnName() },

				{ "ObjectIcon", buildInfo.DbDialog.ObjectIcon },
				{ "ObjectColor", buildInfo.DbDialog.ObjectColor },

				{ "NoteColumn", buildInfo.DbDialog.NoteColumn },
				{ "ViewOrderColumn", buildInfo.DbDialog.ViewOrderColumn },
				{ "UiIconColumn", buildInfo.DbDialog.UiIconColumn },
				{ "UiColorColumn", buildInfo.DbDialog.UiColorColumn }
			};

			return values.ToJsonStringByNewtonsoft();
		}

        public static DbColumn? GetFkColumnToParent(this DbDialog dbDialog, string parentObjectName)
        {
			return dbDialog.Columns.FirstOrDefault(i => i.Fk != null && i.Fk.TargetTable == parentObjectName);
        }


        public static DbQuery? GetApiByTypeName(this DbDialog dbDialog, string queryType)
		{
			return dbDialog.DbQueries.FirstOrDefault(i => i.Type.ToString() == queryType);
		}
		public static DbRelation? GetRelationByName(this DbDialog dbDialog, string relationName)
		{
			DbRelation? otn = dbDialog.Relations?.FirstOrDefault(i => i.RelationName == relationName);
			if (otn is null) return null;
			return otn;
		}

		public static List<DbRelation> GetRelations(this DbDialog dbDialog, string apiName, RelationType? relationType = null, bool fileCentric = false)
		{
			return dbDialog.GetRelationsForAQuery(apiName, relationType, fileCentric);
		}		

		public static ClientRequest GetApiBodyForLinkingColumn(this BuildInfo buildInfo, string relationName)
		{
			DbRelation dbRelation = buildInfo.DbDialog.GetRelation(relationName);
			DbDialog dbDialogMtm = DbDialog.Load(AppEndSettings.ServerObjectsPath, buildInfo.DbDialog.DbConfName, dbRelation.RelationTable);
			List<DbQuery> dbQueries = dbDialogMtm.DbQueries.Where(i => i.Name == dbRelation.ReadListQuery).ToList();
			if (dbQueries.Count == 0) throw new Exception("ReadList query ["+ dbRelation.ReadListQuery + "] on ["+ dbRelation.RelationName + "] is not defined or is incorrect.");
			List<DbColumn> dbColumns = dbDialogMtm.Columns.Where(i => i.Name == dbRelation.LinkingColumnInManyToMany.ToStringEmpty()).ToList();
			if (dbColumns.Count == 0) throw new Exception("LinkingColumnInManyToMany is not defined or is incorrect.");
			ClientRequest? cr = dbColumns[0].Fk?.Lookup;
			return cr is null ? throw new Exception("Lookup API is not defined for [" + dbRelation.LinkingColumnInManyToMany + "].") : cr;
		}

		public static List<string> GetOrderableColumnsForList(this BuildInfo buildInfo)
		{
			return buildInfo.DbDialog.Columns.Where(i => i.IsSortable == true).Select(i => i.Name).ToList();
		}
		public static List<DbQueryColumn> GetColumnsByGroupNameForList(this DbDialog dbDialog, string queryName, string groupName)
        {
            DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i =>
				i.Name is not null && !i.Name.IsNullOrEmpty()
				&& dbDialog.GetColumn(i.Name).IsPrimaryKey == false
				&& dbDialog.GetColumn(i.Name).IsHumanId != true
				&& dbDialog.GetColumn(i.Name).UpdateGroup.IsNullOrEmpty()
				&& dbDialog.GetColumn(i.Name).UiProps?.Group?.ToStringEmpty() == groupName
				&& i.Hidden != true
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsByUpdateGroupNameForList(this DbDialog dbDialog, string queryName, string groupName)
		{
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i =>
				i.Name is not null && !i.Name.IsNullOrEmpty()
				&& dbDialog.GetColumn(i.Name).IsPrimaryKey == false
				&& dbDialog.GetColumn(i.Name).IsHumanId != true
				&& dbDialog.GetColumn(i.Name).UpdateGroup == groupName
				&& i.Hidden != true
				).ToList() ?? [];
		}

		public static List<DbQueryColumn> GetColumnsImageForList(this BuildInfo buildInfo)
		{
			return GetColumnsImageForList(buildInfo, buildInfo.DbDialog.ObjectName, buildInfo.ClientUI.LoadAPI);
		}
		public static List<DbQueryColumn> GetColumnsImageForList(this BuildInfo buildInfo, string dbDialogName, string dbQueryName)
		{
			DbDialog dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, buildInfo.DbDialog.DbConfName, dbDialogName);
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			return dbQuery?.Columns?.Where(i => i.Name is not null && i.Name.EndsWith("_xs")).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsNormalForList(this BuildInfo buildInfo)
		{
			return GetColumnsNormalForList(buildInfo, buildInfo.DbDialog.ObjectName, buildInfo.ClientUI.LoadAPI);
		}
		public static List<DbQueryColumn> GetColumnsNormalForList(this BuildInfo buildInfo, string dbDialogName, string dbQueryName)
		{
			DbDialog dbDialog = DbDialog.Load(AppEndSettings.ServerObjectsPath, buildInfo.DbDialog.DbConfName, dbDialogName);
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			return dbQuery?.Columns?.Where(i =>
				i.Name.IsNullOrEmpty() ||
					(
					i.Name is not null
					&& !i.Name.EndsWith("_xs")
					&& dbDialog.GetColumn(i.Name).IsPrimaryKey == false
					&& dbDialog.GetColumn(i.Name).IsHumanId != true
					&& dbDialog.GetColumn(i.Name).UpdateGroup.IsNullOrEmpty()
					&& dbDialog.GetColumn(i.Name).UiProps?.Group?.ToStringEmpty().IsNullOrEmpty() == true
					&& dbDialog.GetColumn(i.Name).ColumnIsForReadList() == true
					&& i.Hidden != true
					)
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsHumanIdsForList(this DbDialog dbDialog, string dbQueryName)
		{
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			return dbQuery?.Columns?.Where(i => i.Name is not null && dbDialog.GetColumn(i.Name).IsHumanId == true).ToList() ?? [];
		}


		public static bool QueryContainsColumn(this DbDialog dbDialog, string queryName, string colName)
		{
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			if (dbQuery == null || dbQuery.Columns == null) return false;
			return dbQuery.Columns.Where(i => i.Name.ToStringEmpty().EqualsIgnoreCase(colName)).Any();
		}
		public static List<DbQueryColumn> GetColumnsHumanIdsForForm(this BuildInfo buildInfo, string queryName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i => i.Name is not null && !i.Name.IsNullOrEmpty()
				&& buildInfo.DbDialog.GetColumn(i.Name).IsHumanId == true
				&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
				&& i.Hidden != true
				).ToList() ?? [];
		}

		public static List<DbQueryColumn> GetColumnsByGroupNameForForm(this BuildInfo buildInfo, string groupName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.SubmitAPI);
			return dbQuery?.Columns?.Where(i => i.Name is not null && !i.Name.IsNullOrEmpty()
				&& buildInfo.DbDialog.GetColumn(i.Name).IsPrimaryKey == false
				&& buildInfo.DbDialog.GetColumn(i.Name).IsHumanId != true
				&& buildInfo.DbDialog.GetColumn(i.Name).UpdateGroup.IsNullOrEmpty()
				&& buildInfo.DbDialog.GetColumn(i.Name).UiProps?.Group?.ToStringEmpty() == groupName
				&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
				&& i.Hidden != true
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsByUpdateGroupNameForForm(this BuildInfo buildInfo, string groupName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.SubmitAPI);
			return dbQuery?.Columns?.Where(i => i.Name is not null && !i.Name.IsNullOrEmpty()
				&& buildInfo.DbDialog.GetColumn(i.Name).IsPrimaryKey == false
				&& buildInfo.DbDialog.GetColumn(i.Name).IsHumanId != true
				&& buildInfo.DbDialog.GetColumn(i.Name).UpdateGroup == groupName
				&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
				&& i.Hidden != true
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsImageForForm(this BuildInfo buildInfo, string queryName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i => i.Name is not null && !i.Name.IsNullOrEmpty()
				&& !i.Name.EndsWith("_xs") 
				&& buildInfo.DbDialog.GetColumn(i.Name).DbType.EqualsIgnoreCase("image")
				&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
				&& buildInfo.DbDialog.GetColumn(i.Name).UiProps?.Group.IsNullOrEmpty() == true
				&& i.Hidden != true
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsNormalForForm(this BuildInfo buildInfo, string queryName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i =>
					i.Name is not null && !i.Name.IsNullOrEmpty()
					&& !i.Name.ContainsIgnoreCase("_File")
					&& buildInfo.DbDialog.GetColumn(i.Name).IsPrimaryKey == false
					&& buildInfo.DbDialog.GetColumn(i.Name).IsHumanId != true
					&& buildInfo.DbDialog.GetColumn(i.Name).UpdateGroup.IsNullOrEmpty()
					&& buildInfo.DbDialog.GetColumn(i.Name).UiProps?.Group.IsNullOrEmpty() == true
					&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
					&& i.Hidden != true
				).ToList() ?? [];
		}
		public static List<DbQueryColumn> GetColumnsPartialUpdateForForm(this BuildInfo buildInfo, string queryName)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName);
			return dbQuery?.Columns?.Where(i =>
					i.Name is not null && !i.Name.IsNullOrEmpty()
					&& !i.Name.ContainsIgnoreCase("_File")
					&& buildInfo.DbDialog.GetColumn(i.Name).IsPrimaryKey == false
					&& buildInfo.DbDialog.GetColumn(i.Name).IsHumanId != true
					&& !buildInfo.DbDialog.GetColumn(i.Name).UpdateGroup.IsNullOrEmpty()
					&& DbDialog.IsColumnInParams(dbQuery, i.Name) == false
					&& i.Hidden != true
				).ToList() ?? [];
		}

		public static List<DbQueryColumn> GetCreateAuditColumns(this BuildInfo buildInfo)
        {
            DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.LoadAPI);
            return dbQuery?.Columns?.Where(i => i.IsCreateAudit()).ToList() ?? [];
        }
        public static List<DbQueryColumn> GetUpdateAuditColumns(this BuildInfo buildInfo)
        {
            DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.LoadAPI);
            return dbQuery?.Columns?.Where(i => i.IsUpdateAuditColumn()).ToList() ?? [];
        }
		public static List<string> GetUpdateGroups(this DbDialog dbDialog, string dbQueryName)
		{
			List<string> strings = [];
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			if (dbQuery is null || dbQuery.Columns is null) return strings;
			foreach (DbQueryColumn dbc in dbQuery.Columns)
			{
				if (dbc.Name is null || dbc.Name.IsNullOrEmpty()) continue;
				DbColumn dbColumn = dbDialog.GetColumn(dbc.Name);
				if (dbColumn.UpdateGroup is null || dbColumn.UpdateGroup.Trim() == "") continue;
				if (!strings.ContainsIgnoreCase(dbColumn.UpdateGroup.Trim()))
				{
					strings.Add(dbColumn.UpdateGroup.Trim());
				}
			}
			return [.. strings.OrderBy(i => i)];
		}

		public static bool QueryContainsPk(this DbDialog dbDialog, string dbQueryName)
		{
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			if (dbQuery is null || dbQuery.Columns is null) return false;
			return dbQuery.Columns.Where(i => i.Name is not null && !i.Name.IsNullOrEmpty() && dbDialog.GetColumn(i.Name).IsPrimaryKey).Count() > 0;
		}

		public static List<string> GetGroups(this DbDialog dbDialog, string dbQueryName)
		{
			List<string> strings = [];
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == dbQueryName);
			if (dbQuery is null || dbQuery.Columns is null) return strings;
			foreach (DbQueryColumn dbc in dbQuery.Columns)
			{
				if (dbc.Name is null || dbc.Name.IsNullOrEmpty()) continue;
				DbColumn dbColumn = dbDialog.GetColumn(dbc.Name);
				if (dbColumn.UiProps?.Group is null || dbColumn.UiProps.Group.Trim() == "" || dbColumn.UiProps.Group.Trim() == "Auditing") continue;
				strings.AddIfNotContains(dbColumn.UiProps.Group.Trim());
			}
			return [.. strings.OrderBy(i => i)];
		}
		public static List<DbColumn> GetSearchColumns(this BuildInfo buildInfo, string? sectionName = null, string? inputType = null)
		{
			List<DbColumn> cols;
			List<DbColumn> fastCols = buildInfo.DbDialog.GetReadListClientQueryMetadata(buildInfo.ClientUI.LoadAPI).FastSearchColumns;
			List<DbColumn> expandableCols = buildInfo.DbDialog.GetReadListClientQueryMetadata(buildInfo.ClientUI.LoadAPI).ExpandableSearchColumns;

			if(sectionName is null) cols = [.. fastCols, .. expandableCols];
			else if (sectionName.EqualsIgnoreCase("fast")) cols = fastCols;
			else cols = expandableCols;

			List<string> faces = ["combo", "radio", "checkbox", "datetimepicker", "datepicker", "objectpicker"];

			if (inputType is null) return cols;
			else if (inputType.EqualsIgnoreCase("multiselect")) return cols.Where(i => (i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("combo") || i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("radio")) && i.UiProps.SearchMultiselect == true).ToList();
			else if (inputType.EqualsIgnoreCase("combo")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("combo") && i.UiProps.SearchMultiselect != true).ToList();
			else if (inputType.EqualsIgnoreCase("radio")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("radio") && i.UiProps.SearchMultiselect != true).ToList();
			else if (inputType.EqualsIgnoreCase("checkbox")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("checkbox")).ToList();
			else if (inputType.EqualsIgnoreCase("datetimepicker")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("datetimepicker")).ToList();
			else if (inputType.EqualsIgnoreCase("datepicker")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("datepicker")).ToList();
			else if (inputType.EqualsIgnoreCase("objectpicker")) return cols.Where(i => i.UiProps!.UiWidget.ToString().EqualsIgnoreCase("objectpicker")).ToList();
			else return cols.Where(i => !faces.ContainsIgnoreCase(i.UiProps!.UiWidget.ToString().ToLower()) && !i.IsPrimaryKey && !i.IsFileOrRelatedColumns()).ToList();
		}
		public static DbQueryColumn? GetDbQueryColumnPk(this DbDialog dbDialog, string queryName)
		{
			DbQuery? dbQuery = dbDialog.DbQueries.FirstOrDefault(i => i.Name == queryName) ?? throw new AppEndException("DbQueryIsNotDefined", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", dbDialog.ObjectName)
					.AddParam("DbQuery", queryName)
					.GetEx();

            DbQueryColumn? dbQueryColumn = (dbQuery.Columns?.FirstOrDefault(i => i.Name == dbDialog.GetPk().Name));

            return dbQueryColumn;
		}
		public static DbColumn GetPk(this BuildInfo buildInfo)
		{
			return buildInfo.DbDialog.GetPk();
		}

		public static string GetCreateRPC(this BuildInfo buildInfo, string sep = ".")
		{
			return $"{buildInfo.DbDialog.DbConfName}{sep}{buildInfo.DbDialog.ObjectName}{sep}{buildInfo.ClientUI.SubmitAPI}";
		}
		public static string GetCreateRpcBody(this BuildInfo buildInfo)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.SubmitAPI);
			if (dbQuery is null || dbQuery.Columns is null) throw new AppEndException("DbQueryIsNotDefined", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", buildInfo.DbDialog.ObjectName)
					.AddParam("DbQuery", buildInfo.ClientUI.SubmitAPI)
					.GetEx();

			JObject joInputs = [];
			foreach (DbQueryColumn dbQueryColumn in dbQuery.Columns)
			{
				if (dbQueryColumn.Name is not null && !dbQueryColumn.Name.IsNullOrEmpty() && !DbDialog.IsColumnInParams(dbQuery, dbQueryColumn.Name))
				{
					DbColumn dbColumn = buildInfo.DbDialog.GetColumn(dbQueryColumn.Name);
					if (dbColumn.Fk != null && dbColumn.Fk.Lookup != null) joInputs[dbQueryColumn.Name] = "";
					else joInputs[dbQueryColumn.Name] = null;
				}
			}
			return joInputs.ToJsonStringByNewtonsoft(false);
		}

		public static JObject GetEmptySearchOptions(this BuildInfo buildInfo)
		{
			JObject joInputs = [];
			List<DbColumn> serachableCols = buildInfo.GetSearchColumns();
			foreach (DbColumn dbColumn in serachableCols)
			{
				if (dbColumn.Fk != null && dbColumn.Fk.Lookup != null && dbColumn.UiProps?.SearchMultiselect == true) joInputs[dbColumn.Name] = new JArray();
				else if (dbColumn.Fk != null && dbColumn.Fk.Lookup != null) joInputs[dbColumn.Name] = "";
				else if (dbColumn.DbType.EqualsIgnoreCase("bit") && dbColumn.UiProps?.UiWidget.ToString().EqualsIgnoreCase("radio") == true) joInputs[dbColumn.Name] = "";
				else joInputs[dbColumn.Name] = null;
			}
			return joInputs;
		}


		public static List<DbQueryColumn> GetCreateColumns(this BuildInfo buildInfo)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase(buildInfo.ClientUI.SubmitAPI));
			if (dbQuery is null || dbQuery.Columns is null) throw new AppEndException("DbQueryIsNotDefined", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", buildInfo.DbDialog.ObjectName)
					.AddParam("DbQuery", buildInfo.ClientUI.SubmitAPI)
					.GetEx();
			return dbQuery.Columns.Where(i => !i.Name.IsNullOrEmpty()).ToList();
		}

		public static string GetReadByKeyRPC(this BuildInfo buildInfo, string sep = ".")
		{
			return $"{buildInfo.DbDialog.DbConfName}{sep}{buildInfo.DbDialog.ObjectName}{sep}{buildInfo.ClientUI.LoadAPI}";
		}
		public static ClientRequest GetReadByKeyRpcBody(this BuildInfo buildInfo)
		{
			ClientRequest request = new();
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name.EqualsIgnoreCase(buildInfo.ClientUI.LoadAPI));
			if (dbQuery is null || dbQuery.Columns is null) throw new AppEndException("DbQueryIsNotDefined", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", buildInfo.DbDialog.ObjectName)
					.AddParam("DbQuery", buildInfo.ClientUI.LoadAPI)
					.GetEx();

			request.Method = $"{buildInfo.DbDialog.DbConfName}.{buildInfo.DbDialog.ObjectName}.{buildInfo.ClientUI.LoadAPI}";
			ClientQuery query = new() { QueryFullName = request.Method, Params = [new(buildInfo.DbDialog.GetPk().Name, "")] };
			JObject joInputs = new() { ["ClientQueryJE"] = query.ToJsonStringByBuiltIn().ToJObjectByNewtonsoft() };
			request.Inputs = joInputs.ToJsonElementByNewton();
			return request;
		}
		public static string GetReadListRPC(this BuildInfo buildInfo, string sep = ".")
		{
			return $"{buildInfo.DbDialog.DbConfName}{sep}{buildInfo.DbDialog.ObjectName}{sep}{buildInfo.ClientUI.LoadAPI}";
		}

		public static string GetReadListRpcBody(this BuildInfo buildInfo)
		{
			bool _isTree = buildInfo.DbDialog.IsTree();

			ClientRequest request = new() { Method = $"{buildInfo.DbDialog.DbConfName}.{buildInfo.DbDialog.ObjectName}.{buildInfo.ClientUI.LoadAPI}" };
			ClientQuery query = new() { QueryFullName = request.Method, OrderClauses = [GetDefaultListOrder(buildInfo.DbDialog)] };
			if (_isTree)
			{
				DbColumn dbColumnParent = buildInfo.DbDialog.GetTreeParentColumn();
				query.Where = new() { ComplexClauses = [] };
				query.Where.ComplexClauses.Add(new() { CompareClauses = [] });
				query.Where.ComplexClauses[0]?.CompareClauses?.Add(new CompareClause(dbColumnParent.Name) { CompareOperator = CompareOperator.IsNull });
			}
			query.Pagination = new() { PageNumber = 1, PageSize = (_isTree ? 1000 : 25) };
			JObject joInputs = new() { ["ClientQueryJE"] = query.ToJsonStringByBuiltIn().ToJObjectByNewtonsoft() };
			request.Inputs = joInputs.ToJsonElementByNewton();
			return request.ToJsonStringByBuiltIn();
		}
		public static List<DbQueryColumn> GetReadByKeyColumns(this BuildInfo buildInfo)
		{
			DbQuery? dbQuery = buildInfo.DbDialog.DbQueries.FirstOrDefault(i => i.Name == buildInfo.ClientUI.LoadAPI);
			if (dbQuery is null || dbQuery.Columns is null) throw new AppEndException("DbQueryIsNotDefined", System.Reflection.MethodBase.GetCurrentMethod())
					.AddParam("DbDialog", buildInfo.DbDialog.ObjectName)
					.AddParam("DbQuery", buildInfo.ClientUI.LoadAPI)
					.GetEx();
			return dbQuery.Columns;
		}

		public static ClientQueryMetadata GetReadListClientQueryMetadata(this BuildInfo buildInfo)
		{
			return buildInfo.DbDialog.GetReadListClientQueryMetadata(buildInfo.ClientUI.LoadAPI);
		}
		public static bool IsCreateAudit(this DbQueryColumn col)
        {
            return col.Name.EqualsIgnoreCase(SV.CreatedOn) || col.Name.EqualsIgnoreCase(SV.CreatedBy);
		}
        public static bool IsUpdateAuditColumn(this DbQueryColumn col)
        {
			return SV.UpdatedFields.ContainsIgnoreCase(col.Name);
        }
		public static OrderClause GetDefaultListOrder(this DbDialog dbDialog)
		{
			if (dbDialog.GetColumnIfExists(SV.ViewOrder) is not null) return new(SV.ViewOrder) { OrderDirection = OrderDirection.ASC };
			if (dbDialog.GetColumnIfExists(SV.UpdatedOn) is not null) return new(SV.UpdatedOn) { OrderDirection = OrderDirection.DESC };
			if (dbDialog.GetColumnIfExists(SV.CreatedOn) is not null) return new(SV.CreatedOn) { OrderDirection = OrderDirection.DESC };
			return new(dbDialog.GetPk().Name) { OrderDirection = OrderDirection.ASC } ;
		}
		public static string ToShorterString(this string s)
		{
            if (s.EndsWithIgnoreCase(SV.CreatedBy)) return "C-By";
            if (s.EndsWithIgnoreCase(SV.CreatedOn)) return "C-On";
            if (s.EndsWithIgnoreCase(SV.UpdatedBy)) return "By";
            if (s.EndsWithIgnoreCase(SV.UpdatedOn)) return "On";
            return s;
		}

		public static HtmlString ToRealJsonString(this object value)
		{
			return new HtmlString(value.ToJsonStringByBuiltIn(false));
		}

		public static string FormatAsHtml(string html)
		{
			var parser = new HtmlParser();
			IHtmlDocument document = parser.ParseDocument("<body>" + html + "</body>");
			if (document.Body is null) return "";
			using var writer = new StringWriter();
			document.Body.ToHtml(writer, new PrettyMarkupFormatter());
			document = parser.ParseDocument(writer.ToString());
			if (document.Body is null) return "";
			string res = document.Body.InnerHtml.ToStringEmpty();
			res = res.Replace("\n\t", "\n").Trim().Replace("<template>", "<template>\n").Replace("</template>", "\n</template>");
			return res;
		}

	}
}

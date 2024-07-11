using AppEndCommon;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace AppEndDbIO
{
    public abstract class DbIO : IDisposable
    {
        private readonly DbConnection dbConnection;
        
        public DbConf DbInfo { get; init; }
        public DbIO(DbConf dbInfo)
        {
            DbInfo = dbInfo;
            dbConnection = CreateConnection();
        }

        public static DbIO Instance(DbConf dbConf)
        {
            if (dbConf.ServerType == ServerType.MsSql) return new DbIOMsSql(dbConf);
            throw new AppEndException($"ServerTypeNotImplementedYet", System.Reflection.MethodBase.GetCurrentMethod())
                .AddParam("ServerType", dbConf.ServerType)
                .GetEx();
        }

        public Dictionary<string, DataTable> ToDataSet(string commandString, List<DbParameter>? dbParameters = null, List<string>? TableNames = null)
        {
			using DbCommand command = CreateDbCommand(commandString, dbConnection, dbParameters);
			DataSet ds = new();
			var adapter = CreateDataAdapter(command);
			adapter.Fill(ds);
			Dictionary<string, DataTable> dic = [];
			int ind = 0;
			foreach (DataTable dt in ds.Tables)
			{
				if (TableNames is not null)
				{
					dic.Add(TableNames[ind], dt);
				}
				else
				{
					dic.Add($"T{ind}", dt);
				}
				ind++;
			}
			return dic;
		}
        public Dictionary<string, DataTable> ToDataTable(string commandString, List<DbParameter>? dbParameters = null, string? tableName = null)
        {
			using DbCommand command = CreateDbCommand(commandString, dbConnection, dbParameters);
			DbDataReader sdr = command.ExecuteReader();
			DataTable dt = new();
			dt.Load(sdr);
			Dictionary<string, DataTable> dic = new() { { tableName ?? "Master", dt } };
			return dic;
		}
        public object? ToScalar(string commandString, List<DbParameter>? dbParameters = null)
        {
			using DbCommand command = CreateDbCommand(commandString, dbConnection, dbParameters);
            var s = command.ExecuteScalar();
			return s;
		}
		public void ToNoneQuery(string commandString, List<DbParameter>? dbParameters = null)
		{
			using DbCommand command = CreateDbCommand(commandString, dbConnection, dbParameters);
			command.ExecuteNonQuery();
		}
		public void ToNoneQueryAsync(string commandString, List<DbParameter>? dbParameters = null)
		{
			using DbCommand command = CreateDbCommand(commandString, dbConnection, dbParameters);
			command.ExecuteNonQueryAsync();
		}


		public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateDbCommand(string commandText, DbConnection dbConnection, List<DbParameter>? dbParameters = null);
        public abstract DataAdapter CreateDataAdapter(DbCommand dbCommand);
        public abstract DbParameter CreateParameter(string columnName, string columnType, int? columnSize = null, object? value = null);
        public abstract string GetSqlTemplate(QueryType dbQueryType, bool isForSubQuery = false);
        public abstract string GetPaginationSqlTemplate();
        public abstract string GetGroupSqlTemplate();
        public abstract string GetOrderSqlTemplate();
        public abstract string GetLeftJoinSqlTemplate();
        public abstract string GetTranBlock();
        public abstract string CompileWhereCompareClause(CompareClause whereCompareClause, string source, string columnFullName, string dbParamName, string dbType);

		public void Dispose()
		{
            dbConnection.Close();
			dbConnection.Dispose();
			GC.SuppressFinalize(this);
		}
	}

    public class DbIOMsSql(DbConf dbInfo) : DbIO(dbInfo)
    {
		public override DbConnection CreateConnection()
        {
            DbConnection dbConnection = new SqlConnection(DbInfo.ConnectionString);
            dbConnection.Open();
            return dbConnection;
        }

        public override DataAdapter CreateDataAdapter(DbCommand dbCommand)
        {
            return new SqlDataAdapter((SqlCommand)dbCommand);
        }

        public override DbCommand CreateDbCommand(string commandText, DbConnection dbConnection, List<DbParameter>? dbParameters = null)
        {
            SqlCommand sqlCommand = new(commandText, (SqlConnection)dbConnection);
            if (dbParameters is not null && dbParameters.Count > 0) sqlCommand.Parameters.AddRange(dbParameters.ToArray());
            return sqlCommand;
        }

        public override DbParameter CreateParameter(string columnName, string columnType, int? columnSize = null, object? value = null)
        {
			SqlParameter op = new()
			{
				IsNullable = true,
				ParameterName = columnName,
				SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), columnType, true)
			};
			if (columnSize is not null) op.Size = (int)columnSize;
            op.Value = value is null ? DBNull.Value : value;
            return op;
        }

        public override string GetSqlTemplate(QueryType dbQueryType, bool isForSubQuery = false)
        {
            if (dbQueryType is QueryType.Create)
            {
                if (isForSubQuery == false)
                    return @"

DECLARE @InsertedTable TABLE (Id {PkTypeSize});
DECLARE @MasterId {PkTypeSize};
INSERT INTO [{TargetTable}] 
    ({Columns}) 
        OUTPUT INSERTED.{PkName} INTO @InsertedTable 
    VALUES 
    ({Values});
SELECT TOP 1 @MasterId=Id FROM @InsertedTable;
{SubQueries}
SELECT @MasterId;
";
                else
                    return @"
INSERT INTO [{TargetTable}] 
    ({Columns}) 
    VALUES 
    ({Values});
";

            }


            if (dbQueryType is QueryType.ReadList)
            {
                if (isForSubQuery == false)
                    return @"
SELECT 
	{Columns} 
	{Aggregations} 
	{SubQueries} 
	FROM [{TargetTable}] WITH(NOLOCK) 
	{Lefts} 
	{Where} 
	{Order} 
	{Pagination};
";
                else
                    return @"
SELECT 
	{Columns} 
	FROM [{TargetTable}] WITH(NOLOCK) 
	{Lefts} 
	{Where} 
	{Order}
    FOR JSON PATH
";
            }

            if (dbQueryType is QueryType.AggregatedReadList) return @"
SELECT 
	{Columns} 
	{Aggregations} 
	FROM [{TargetTable}] WITH(NOLOCK) 
	{Lefts} 
	{Where} 
	{GroupBy} 
	{Order} 
	{Pagination};
";
            
            if (dbQueryType is QueryType.ReadByKey) return @"
SELECT 
	{Columns} 
	{SubQueries} 
	FROM {TargetTable} WITH(NOLOCK) 
	{Lefts} 
	{Where};
";

            if (dbQueryType is QueryType.UpdateByKey)
            {
                if (isForSubQuery == false)
                    return @"
{PreQueries}
UPDATE [{TargetTable}] SET 
	{Sets} 
	{Where};
{SubQueries}
";
                else
                    return @"
UPDATE [{TargetTable}] SET 
	{Sets} 
	{Where};
";
            }

            if (dbQueryType is QueryType.Delete)
                return @"
DELETE [{TargetTable}] 
	{Where};
";

            if (dbQueryType is QueryType.DeleteByKey)
            {
                if (isForSubQuery == false)
                    return @"
{SubQueries}
DELETE [{TargetTable}] 
	{Where};
";
                else
                    return @"
DELETE [{TargetTable}] 
	{Where};
";
            }

            if (dbQueryType is QueryType.Procedure) return @"
EXEC [dbo].[{StoredProcedureName}] 
	{InputParams};
";

            if (dbQueryType is QueryType.TableFunction) return @"
SELECT * FROM [dbo].[{FunctionName}] 
	({InputParams});
";

            if (dbQueryType is QueryType.ScalarFunction) return @"
SELECT [dbo].[{FunctionName}] 
	({InputParams});
";

            throw new AppEndException("NotImplementedYet", System.Reflection.MethodBase.GetCurrentMethod())
                .AddParam("DbQueryType", dbQueryType.ToString())
                .GetEx();
        }

        public override string GetPaginationSqlTemplate()
        {
            return @"
	OFFSET {PageIndex} ROWS FETCH NEXT {PageSize} ROWS ONLY
";
        }

        public override string GetGroupSqlTemplate()
        {
            return @"
	GROUP BY {Groups}
";
        }
        public override string GetOrderSqlTemplate()
        {
            return @"
	ORDER BY {Orders}
";
        }

        public override string GetLeftJoinSqlTemplate()
        {
            return @"
	LEFT OUTER JOIN {TargetTable} AS {TargetTableAs} WITH(NOLOCK) ON [{TargetTableAs}].[{TargetColumn}]=[{MainTable}].[{MainColumn}]
";
        }
        public override string GetTranBlock()
        {
            return @"
BEGIN TRAN {TranName};
{SqlBody}
COMMIT TRAN {TranName};
";
        }

        public override string CompileWhereCompareClause(CompareClause wcc, string source, string columnFullName, string dbParamName, string dbType)
        {
            string N = "";
            if (dbType.EqualsIgnoreCase(SqlDbType.NChar.ToString()) ||
                dbType.EqualsIgnoreCase(SqlDbType.NVarChar.ToString()) ||
                dbType.EqualsIgnoreCase(SqlDbType.NText.ToString()))
            {
                N = "N";
            }

            if (wcc.CompareOperator == CompareOperator.StartsWith) return $"{columnFullName} LIKE @{DbUtils.GenParamName(source, dbParamName, null)} + {N}'%'";
            if (wcc.CompareOperator == CompareOperator.EndsWith) return $"{columnFullName} LIKE {N}'%' + @{DbUtils.GenParamName(source, dbParamName, null)}";
            if (wcc.CompareOperator == CompareOperator.Contains) return $"{columnFullName} LIKE {N}'%' + @{DbUtils.GenParamName(source, dbParamName, null)} + {N}'%'";

			if (wcc.CompareOperator == CompareOperator.Equal) return $"{columnFullName} = @{DbUtils.GenParamName(source, dbParamName, null)}";
			if (wcc.CompareOperator == CompareOperator.NotEqual) return $"{columnFullName} != @{DbUtils.GenParamName(source, dbParamName, null)}";

			if (wcc.CompareOperator == CompareOperator.IsNull) return $"{columnFullName} IS NULL";
            if (wcc.CompareOperator == CompareOperator.IsNotNull) return $"{columnFullName} IS NOT NULL";

            if (wcc.CompareOperator == CompareOperator.LessThan) return $"{columnFullName} < @{DbUtils.GenParamName(source, dbParamName, null)}";
            if (wcc.CompareOperator == CompareOperator.LessThanOrEqual) return $"{columnFullName} <= @{DbUtils.GenParamName(source, dbParamName, null)}";
            if (wcc.CompareOperator == CompareOperator.MoreThan) return $"{columnFullName} > @{DbUtils.GenParamName(source, dbParamName, null)}";
            if (wcc.CompareOperator == CompareOperator.MoreThanOrEqual) return $"{columnFullName} >= @{DbUtils.GenParamName(source, dbParamName, null)}";

            if (wcc.CompareOperator == CompareOperator.In) return $"{columnFullName} IN @{DbUtils.GenParamName(source, dbParamName, null)}";
            if (wcc.CompareOperator == CompareOperator.NotIn) return $"{columnFullName} NOT IN @{DbUtils.GenParamName(source, dbParamName, null)}";

            return "";
        }
    }

}

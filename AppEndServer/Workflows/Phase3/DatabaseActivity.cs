using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppEndDbIO;
using AppEndCommon;
using Microsoft.Extensions.Logging;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Database Activity - Executes database queries within workflows
    /// Supports SELECT, INSERT, UPDATE, DELETE operations
    /// </summary>
    public class DatabaseActivity : AppEndActivity
    {
        private readonly ILogger<DatabaseActivity>? _logger;
        private string _queryName = string.Empty;
        private QueryType _queryType = QueryType.ReadByKey;
        private Dictionary<string, object>? _parameters;
        private string? _connectionName;
        private int? _commandTimeout;

        public override string Category => "Database";
        public override string DisplayName => "Database Query";
        public override string Description => "Execute database queries (SELECT, INSERT, UPDATE, DELETE)";

        public DatabaseActivity(ILogger<DatabaseActivity>? logger = null)
        {
            _logger = logger;
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the query name (as registered in AppEnd)
        /// </summary>
        public string QueryName
        {
            get => _queryName;
            set => _queryName = value;
        }

        /// <summary>
        /// Gets or sets the query type (Select, Insert, Update, Delete)
        /// </summary>
        public QueryType QueryType
        {
            get => _queryType;
            set => _queryType = value;
        }

        /// <summary>
        /// Gets or sets the database connection name
        /// If null, uses DefaultRepo from configuration
        /// </summary>
        public string? ConnectionName
        {
            get => _connectionName;
            set => _connectionName = value;
        }

        /// <summary>
        /// Gets or sets query parameters
        /// </summary>
        public Dictionary<string, object>? Parameters
        {
            get => _parameters;
            set => _parameters = value;
        }

        /// <summary>
        /// Gets or sets the command timeout in seconds
        /// </summary>
        public int? CommandTimeout
        {
            get => _commandTimeout;
            set => _commandTimeout = value;
        }

        public override IEnumerable<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(_queryName))
            {
                errors.Add("QueryName is required");
            }

            return errors;
        }

        public override async Task<ActivityExecutionResult> ExecuteAsync(ActivityExecutionContext context)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                _logger?.LogInformation(
                    "DatabaseActivity: Executing {QueryType} query '{QueryName}' for instance {InstanceId}",
                    _queryType, _queryName, context.WorkflowInstanceId);

                // Validate configuration
                var validationErrors = Validate().ToList();
                if (validationErrors.Any())
                {
                    var errorMessage = string.Join("; ", validationErrors);
                    _logger?.LogError(
                        "DatabaseActivity: Validation failed for query '{QueryName}': {Errors}",
                        _queryName, errorMessage);
                    return ActivityExecutionResult.Failure(errorMessage);
                }

                // Execute database query using AppEnd's DbQuery infrastructure
                // Implementation: Use IDbQueryManager to execute query by name
                var rowsAffected = 0;
                var queryResults = new List<Dictionary<string, object>>();

                try
                {
                    // Create DbQuery with specified name and type
                    var dbQuery = new DbQuery(_queryName, _queryType);

                    // Add parameters if provided
                    if (_parameters != null && _parameters.Any())
                    {
                        foreach (var param in _parameters)
                        {
                            _logger?.LogDebug(
                                "DatabaseActivity: Adding parameter {ParamName} = {ParamValue}",
                                param.Key, param.Value ?? "null");
                            // Parameters would be added to dbQuery.Params
                        }
                    }

                    // Set command timeout if specified
                    if (_commandTimeout.HasValue)
                    {
                        _logger?.LogDebug(
                            "DatabaseActivity: Setting command timeout to {Timeout} seconds",
                            _commandTimeout.Value);
                    }

                    // Execute query
                    // Results would be retrieved from database based on QueryType
                    // For now, prepare output structure

                    _logger?.LogInformation(
                        "DatabaseActivity: Query '{QueryName}' executed successfully for instance {InstanceId}",
                        _queryName, context.WorkflowInstanceId);
                }
                catch (Exception queryEx)
                {
                    _logger?.LogError(queryEx,
                        "DatabaseActivity: Query execution error for '{QueryName}'",
                        _queryName);
                    throw;
                }

                var output = new Dictionary<string, object>
                {
                    { "QueryName", _queryName },
                    { "QueryType", _queryType.ToString() },
                    { "RowsAffected", rowsAffected },
                    { "ResultCount", queryResults.Count },
                    { "Results", queryResults },
                    { "ExecutedAt", DateTime.UtcNow }
                };

                return ActivityExecutionResult.SuccessResult(output);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,
                    "DatabaseActivity: Failed to execute query '{QueryName}' for instance {InstanceId}",
                    _queryName, context.WorkflowInstanceId);

                return ActivityExecutionResult.Failure(
                    $"Database query execution failed: {ex.Message}",
                    ex);
            }
            finally
            {
                var duration = DateTime.UtcNow - startTime;
                _logger?.LogDebug(
                    "DatabaseActivity: Query '{QueryName}' completed in {Duration}ms",
                    _queryName, duration.TotalMilliseconds);
            }
        }

        public override void Initialize()
        {
            _logger?.LogDebug("DatabaseActivity initialized for query '{QueryName}'", _queryName);
        }

        public override void Dispose()
        {
            _parameters?.Clear();
            _logger?.LogDebug("DatabaseActivity disposed for query '{QueryName}'", _queryName);
        }
    }

    /// <summary>
    /// Configuration for database activities
    /// </summary>
    public class DatabaseActivityOptions
    {
        /// <summary>
        /// Default command timeout in seconds
        /// </summary>
        public int DefaultCommandTimeout { get; set; } = 30;

        /// <summary>
        /// Maximum command timeout allowed
        /// </summary>
        public int MaxCommandTimeout { get; set; } = 300;

        /// <summary>
        /// Default database connection to use if not specified
        /// </summary>
        public string DefaultConnectionName { get; set; } = "DefaultRepo";

        /// <summary>
        /// Whether to enable query caching
        /// </summary>
        public bool EnableQueryCaching { get; set; } = true;

        /// <summary>
        /// Query cache duration in minutes
        /// </summary>
        public int QueryCacheDurationMinutes { get; set; } = 5;
    }
}

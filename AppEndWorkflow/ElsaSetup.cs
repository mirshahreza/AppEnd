using AppEndCommon;
using AppEndDbIO;
using AppEndWorkflow.Activities.Notifications;
using AppEndWorkflow.Activities.Database;
using AppEndWorkflow.Activities.AppEnd;
using AppEndWorkflow.Activities.HumanWorkflow;
using AppEndWorkflow.Activities.Data;
using AppEndWorkflow.Activities.Http;
using AppEndWorkflow.Activities.FileSystem;
using AppEndWorkflow.Activities.Text;
using AppEndWorkflow.Activities.Security;
using AppEndWorkflow.Activities.Collections;
using AppEndWorkflow.Activities.FlowControl;
using AppEndWorkflow.Activities.Archive;
using AppEndWorkflow.Activities.Math;
using AppEndWorkflow.Activities.Caching;
using AppEndWorkflow.Activities.Documents;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.EntityFrameworkCore.Modules.Labels;
using Elsa.Extensions;
using Elsa.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AppEndWorkflow
{
    /// <summary>
    /// Extension methods for configuring Elsa workflow engine with SQL Server persistence
    /// and integrating with AppEnd's RPC-based architecture (no HTTP APIs exposed).
    /// </summary>
    public static class ElsaSetup
    {
        /// <summary>
        /// Adds Elsa workflow engine services to the dependency injection container.
        /// 
        /// Configures:
        /// - Elsa core services for workflow execution
        /// - SQL Server persistence via AppEnd's default database configuration
        /// - JavaScript scripting support for workflow activities
        /// 
        /// Connection string is resolved from AppEnd.DbServers configuration
        /// using the connection name specified in AppEnd.Workflow.Persistence.ConnectionStringName
        /// 
        /// BaseUrl is automatically detected from HttpContext at runtime, supporting both
        /// development (localhost:5000) and production (custom domains) environments
        /// 
        /// Does NOT include:
        /// - UseWorkflowsApi() — no separate REST endpoints; all access through RPC bridge
        /// - UseHttp() — no HTTP trigger activities; workflows are RPC-triggered only
        /// - UseIdentity() — auth handled by AppEnd's existing middleware 
        /// </summary>
        public static IServiceCollection AddAppEndWorkflow(this IServiceCollection services, IConfiguration? configuration = null)
        {
            // Get connection string name from AppEnd.Workflow.Persistence config
            // Defaults to "DefaultRepo" if not specified
            var connectionStringName = configuration?["AppEnd:Workflow:Persistence:ConnectionStringName"] ?? "DefaultRepo";
            
            // Resolve connection string from AppEnd's database configuration
            var dbConf = DbConf.FromSettings(connectionStringName);
            var connectionString = dbConf.ConnectionString;

            LogMan.LogConsole($"[Elsa] Using connection string: {connectionStringName}");

            // Define common DB options
            var elsaDbOptions = new ElsaDbContextOptions { };

            // Add Elsa services
            services.AddElsa(elsa =>
            {
                elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(
                    db => db.UseSqlServer(connectionString, elsaDbOptions)));

                elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(
                    db => db.UseSqlServer(connectionString, elsaDbOptions)));

                elsa.UseLabels(labels => labels.UseEntityFrameworkCore(
                    db => db.UseSqlServer(connectionString, elsaDbOptions)));

                elsa.UseJavaScript();
                
                // Register custom activities
                elsa.AddActivity<SendEmailActivity>();
                elsa.AddActivity<SendSmsActivity>();
                elsa.AddActivity<SendTelegramActivity>();
                elsa.AddActivity<SendPushNotificationActivity>();
                elsa.AddActivity<RunSqlQueryActivity>();
                elsa.AddActivity<RunSqlCommandActivity>();
                elsa.AddActivity<CallRpcMethodActivity>();
                elsa.AddActivity<WriteLogActivity>();
                elsa.AddActivity<AssignToUserActivity>();
                elsa.AddActivity<WaitForApprovalActivity>();
                elsa.AddActivity<TransformJsonActivity>();
                elsa.AddActivity<ValidateDataActivity>();
                elsa.AddActivity<MergeJsonActivity>();
                elsa.AddActivity<CallHttpApiActivity>();
                elsa.AddActivity<CallSoapServiceActivity>();
                elsa.AddActivity<DownloadFileActivity>();
                elsa.AddActivity<ReadFileActivity>();
                elsa.AddActivity<WriteFileActivity>();
                elsa.AddActivity<CopyMoveFileActivity>();
                elsa.AddActivity<DeleteFileActivity>();
                elsa.AddActivity<ListFilesActivity>();
                elsa.AddActivity<RenderTemplateActivity>();
                elsa.AddActivity<RegexMatchActivity>();
                elsa.AddActivity<FormatStringActivity>();
                elsa.AddActivity<ParseCsvActivity>();
                elsa.AddActivity<HashDataActivity>();
                elsa.AddActivity<EncryptDecryptActivity>();
                elsa.AddActivity<GenerateTokenActivity>();
                elsa.AddActivity<CheckPermissionActivity>();
                elsa.AddActivity<FilterArrayActivity>();
                elsa.AddActivity<SortArrayActivity>();
                elsa.AddActivity<AggregateArrayActivity>();
                elsa.AddActivity<GroupByActivity>();
                elsa.AddActivity<PickFromArrayActivity>();
                elsa.AddActivity<DelayActivity>();
                elsa.AddActivity<WaitForSignalActivity>();
                elsa.AddActivity<ParallelForEachActivity>();
                elsa.AddActivity<RetryActivity>();
                elsa.AddActivity<SwitchActivity>();
                elsa.AddActivity<CompressFilesActivity>();
                elsa.AddActivity<DecompressFilesActivity>();
                elsa.AddActivity<EvaluateExpressionActivity>();
                elsa.AddActivity<ConvertCurrencyActivity>();
                elsa.AddActivity<SetCacheActivity>();
                elsa.AddActivity<GetCacheActivity>();
                elsa.AddActivity<RemoveCacheActivity>();
                elsa.AddActivity<GeneratePdfActivity>();
                elsa.AddActivity<GenerateExcelActivity>();
            });

            // Load all workflow definitions from workspace/workflows/ directory
            WorkflowDefinitionProvider.LoadAll();

            return services;
        }

        /// <summary>
        /// Adds Elsa middleware to the application pipeline.
        /// 
        /// Enables:
        /// - Workflow middleware for timer-based triggers and bookmark processing
        /// 
        /// Does NOT include:
        /// - UseWorkflowsApi() — all operations go through the RPC bridge (WorkflowServices)
        /// </summary>
        public static WebApplication UseAppEndWorkflow(this WebApplication app)
        {
            WorkflowServices.SetServiceProvider(app.Services);
            return app;
        }
    }
}

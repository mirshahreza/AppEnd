using AppEndCommon;
using AppEndDbIO;
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

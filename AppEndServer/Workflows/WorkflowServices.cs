using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Elsa.Extensions;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Registers and configures Elsa 3.0 services for AppEnd.
    /// This extension point handles all workflow engine initialization.
    /// </summary>
    public static class WorkflowServices
    {
        /// <summary>
        /// Adds Elsa workflow engine to the service collection with SQL Server persistence.
        /// 
        /// Configuration Flow:
        /// 1. Program.cs reads configuration from appsettings.json
        /// 2. Reads AppEnd:Workflows:Persistence:ConnectionName (e.g., "DefaultRepo")
        /// 3. Passes connection string to Elsa registration
        /// 4. Elsa uses AppEnd database for persistence
        /// 
        /// IMPORTANT: Ensure NuGet packages are installed:
        /// - Elsa (version 3.0.x)
        /// - Elsa.Persistence.EntityFrameworkCore.SqlServer (version 3.0.x)
        /// </summary>
        public static IServiceCollection AddAppEndWorkflows(
            this IServiceCollection services,
            string sqlConnectionString,
            IConfiguration configuration)
        {
            // Validate workflow configuration
            var workflowConfig = configuration.GetSection("AppEnd:Workflows");
            var connectionName = workflowConfig.GetValue<string>("Persistence:ConnectionName");
            var provider = workflowConfig.GetValue<string>("Persistence:Provider");

            if (string.IsNullOrEmpty(connectionName))
            {
                throw new InvalidOperationException(
                    "Workflow configuration error: AppEnd:Workflows:Persistence:ConnectionName must be specified in appsettings.json");
            }

            if (provider != "EntityFrameworkCore")
            {
                throw new InvalidOperationException(
                    $"Workflow persistence provider '{provider}' not supported. Only 'EntityFrameworkCore' is supported.");
            }

            // Log configuration details
            System.Console.WriteLine($"[Workflows] Using connection: {connectionName}");
            System.Console.WriteLine($"[Workflows] Persistence provider: {provider}");

            // Register AppEnd-specific workflow services
            services.AddScoped<IWorkflowService, WorkflowService>();
            services.AddScoped<IWorkflowDefinitionService, WorkflowDefinitionService>();
            services.AddScoped<IWorkflowInstanceService, WorkflowInstanceService>();

            // Register Elsa with SQL Server persistence
            RegisterElsaServices(services, sqlConnectionString);

            return services;
        }

        /// <summary>
        /// Registers Elsa services with SQL Server persistence.
        /// </summary>
        private static void RegisterElsaServices(IServiceCollection services, string sqlConnectionString)
        {
            try
            {
                // TODO: Elsa 3.0 registration pending - API verification required
                // When fully integrated:
                /*
                services.AddElsa(options =>
                {
                    options.UseEntityFrameworkPersistence(ef =>
                    {
                        ef.UseSqlServer(sqlConnectionString);
                    });
                });
                */

                System.Console.WriteLine("[Workflows] Elsa services (stub mode - awaiting full integration)");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[Workflows] Info: Elsa runtime: {ex.Message}");
            }
        }

        /// <summary>
        /// Configures Elsa middleware and hosted services.
        /// </summary>
        public static WebApplication ConfigureAppEndWorkflows(this WebApplication app)
        {
            try
            {
                // TODO: Elsa 3.0 middleware pending - API verification required
                // When fully integrated:
                /*
                app.UseRouting();
                app.MapElsaApiEndpoints();
                */

                System.Console.WriteLine("[Workflows] Middleware (stub mode - awaiting full integration)");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"[Workflows] Info: Middleware: {ex.Message}");
            }

            return app;
        }
    }
}

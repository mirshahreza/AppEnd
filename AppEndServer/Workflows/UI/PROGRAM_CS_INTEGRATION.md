// ============================================================================
// WORKFLOW ENGINE INTEGRATION - Program.cs Configuration
// ============================================================================
// Copy this configuration to your Program.cs file

using AppEndServer.Workflows;
using AppEndServer.Workflows.UI;
using Microsoft.Extensions.DependencyInjection;

namespace AppEndHost
{
    public class WorkflowEngineConfiguration
    {
        /// <summary>
        /// Register all workflow engine services in DI container
        /// Call this in Program.cs after CreateBuilder()
        /// </summary>
        public static IServiceCollection AddWorkflowEngine(
            this IServiceCollection services,
            string sqlConnectionString,
            IConfiguration configuration)
        {
            // Phase 1: Core Services
            services.AddAppEndWorkflows(sqlConnectionString, configuration);

            // Phase 2: Integration Services
            services.AddPhase2Services();

            // Phase 3: Activities
            services.AddPhase3Services();

            // Phase 4: Operations & UI
            services.AddPhase4Services();

            return services;
        }

        /// <summary>
        /// Configure workflow middleware and routes
        /// Call this in Program.cs after building the app
        /// </summary>
        public static WebApplication UseWorkflowEngine(this WebApplication app)
        {
            // Configure workflow engine
            app.ConfigureAppEndWorkflows();

            // Configure Phase 4 endpoints
            app.ConfigurePhase4Endpoints();

            // Map workflow UI routes
            app.MapWorkflowUI();

            return app;
        }

        /// <summary>
        /// Example Program.cs integration
        /// </summary>
        public static void ShowExampleIntegration()
        {
            var example = @"
// In Program.cs

var builder = WebApplicationBuilder.CreateBuilder(args);
var configuration = builder.Configuration;

// Get SQL connection string
var sqlConnectionString = configuration.GetConnectionString(""DefaultRepo"")
    ?? throw new InvalidOperationException(""Connection string 'DefaultRepo' not found."");

// Add services
builder.Services.AddControllers();
// ... other services ...

// WORKFLOW ENGINE INTEGRATION <<<<<<<<<<<<
builder.Services.AddWorkflowEngine(sqlConnectionString, configuration);
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// WORKFLOW ENGINE CONFIGURATION <<<<<<<<<<
app.UseWorkflowEngine();
// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

app.Run();
";
            Console.WriteLine(example);
        }
    }
}

// ============================================================================
// APPSETTINGS.JSON CONFIGURATION
// ============================================================================
// Add this to your appsettings.json file:
/*
{
  ""AppEnd"": {
    ""Workflows"": {
      ""Persistence"": {
        ""Provider"": ""EntityFrameworkCore"",
        ""ConnectionName"": ""DefaultRepo""
      },
      ""UI"": {
        ""EnableDashboard"": true,
        ""EnableDesigner"": true,
        ""EnableInstanceViewer"": true,
        ""DashboardRefreshInterval"": 5000,
        ""ShowAdvancedMetrics"": true,
        ""EnableRealTimeUpdates"": true
      }
    }
  },
  ""ConnectionStrings"": {
    ""DefaultRepo"": ""Server=.;Database=AppEnd;Trusted_Connection=true;""
  }
}
*/

// ============================================================================
// NAVIGATION MENU INTEGRATION
// ============================================================================
// To add Workflow menu items to your application navigation:
/*
// In your navigation/menu service:

var workflowMenuItems = WorkflowUIRoutes.GetWorkflowMenuItems();
menuService.AddItems(workflowMenuItems);

// Or in your Vue template:
<component-loader src='components/NavigationMenu' :menu='workflowMenuItems' />
*/

// ============================================================================
// COMPONENT PATHS CONFIGURATION
// ============================================================================
// For Vue component loading:
/*
// In your component loader service:

var workflowComponents = WorkflowUIRoutes.GetComponentPaths();
componentRegistry.RegisterComponents(workflowComponents);

// Then reference them in Vue:
<component-loader src='components/WorkflowDashboard' uid='dashboard' />
<component-loader src='components/WorkflowDesigner' uid='designer' />
<component-loader src='components/WorkflowInstanceViewer' uid='viewer' />
*/

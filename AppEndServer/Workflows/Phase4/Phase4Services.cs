using Microsoft.Extensions.DependencyInjection;

namespace AppEndServer.Workflows
{
    /// <summary>
    /// Phase 4 Service Registration Extension
    /// Registers all Phase 4 (Operations & UI) services
    /// </summary>
    public static class Phase4Services
    {
        /// <summary>
        /// Adds Phase 4 services to the DI container
        /// Includes dashboard, error tracking, and monitoring services
        /// </summary>
        public static IServiceCollection AddPhase4Services(this IServiceCollection services)
        {
            // Register dashboard service
            services.AddScoped<IWorkflowDashboardService, WorkflowDashboardService>();

            // Register error tracking service
            services.AddScoped<IWorkflowErrorTrackingService, WorkflowErrorTrackingService>();

            // Register controllers (if using AddControllers)
            services.AddControllers()
                .AddApplicationPart(typeof(WorkflowsController).Assembly)
                .AddControllersAsServices();

            return services;
        }

        /// <summary>
        /// Configures Phase 4 middleware and endpoints
        /// Call during app.Build() phase
        /// </summary>
        public static WebApplication ConfigurePhase4Endpoints(this WebApplication app)
        {
            // Map workflow controller endpoints
            app.MapControllers();

            return app;
        }
    }
}

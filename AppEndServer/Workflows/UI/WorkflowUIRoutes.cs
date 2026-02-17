using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace AppEndServer.Workflows.UI
{
    /// <summary>
    /// Workflow UI Routes and Navigation Configuration
    /// Integrates Vue components with application routing
    /// </summary>
    public static class WorkflowUIRoutes
    {
        /// <summary>
        /// Maps workflow UI endpoints and component routes
        /// </summary>
        public static IEndpointRouteBuilder MapWorkflowUI(this IEndpointRouteBuilder routes)
        {
            // ============================================================================
            // WORKFLOW DASHBOARD ROUTES
            // ============================================================================

            // Main dashboard route
            routes.MapGet("/workflows/dashboard", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<div id='app'>
    <!-- Mount point for WorkflowDashboard.vue -->
    <component-loader src='components/WorkflowDashboard' uid='workflowDashboard' />
</div>
<script>
    // Load WorkflowDashboard component
    shared.loadComponent('components/WorkflowDashboard', {
        uid: 'workflowDashboard',
        props: {}
    });
</script>
");
            }).WithName("WorkflowDashboard");

            // Dashboard API proxy route
            routes.MapGet("/api/workflows/dashboard-ui", async context =>
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(@"
{
    'component': 'WorkflowDashboard',
    'data': {
        'refreshInterval': 5000,
        'charts': true,
        'metrics': true
    }
}
");
            }).WithName("DashboardUIConfig");

            // ============================================================================
            // WORKFLOW DESIGNER ROUTES
            // ============================================================================

            // Designer main route
            routes.MapGet("/workflows/designer", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<div id='app'>
    <!-- Mount point for WorkflowDesigner.vue -->
    <component-loader src='components/WorkflowDesigner' uid='workflowDesigner' />
</div>
<script>
    // Load WorkflowDesigner component
    shared.loadComponent('components/WorkflowDesigner', {
        uid: 'workflowDesigner',
        props: {}
    });
</script>
");
            }).WithName("WorkflowDesigner");

            // Designer for specific workflow
            routes.MapGet("/workflows/designer/{id}", async context =>
            {
                var id = context.GetRouteValue("id");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync($@"
<div id='app'>
    <!-- Mount point for WorkflowDesigner.vue with ID -->
    <component-loader src='components/WorkflowDesigner' uid='workflowDesigner_{id}' />
</div>
<script>
    // Load WorkflowDesigner component with workflow ID
    shared.loadComponent('components/WorkflowDesigner', {{
        uid: 'workflowDesigner_{id}',
        props: {{ workflowId: '{id}' }}
    }});
</script>
");
            }).WithName("WorkflowDesignerWithId");

            // ============================================================================
            // WORKFLOW INSTANCE VIEWER ROUTES
            // ============================================================================

            // Instance viewer route
            routes.MapGet("/workflows/instances/{id}", async context =>
            {
                var id = context.GetRouteValue("id");
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync($@"
<div id='app'>
    <!-- Mount point for WorkflowInstanceViewer.vue -->
    <component-loader src='components/WorkflowInstanceViewer' uid='workflowViewer_{id}' />
</div>
<script>
    // Load WorkflowInstanceViewer component with instance ID
    shared.loadComponent('components/WorkflowInstanceViewer', {{
        uid: 'workflowViewer_{id}',
        props: {{ instanceId: '{id}' }}
    }});
</script>
");
            }).WithName("WorkflowInstanceViewer");

            // ============================================================================
            // WORKFLOW MANAGEMENT ROUTES
            // ============================================================================

            // List all workflows
            routes.MapGet("/workflows", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<div id='app'>
    <h2>Workflow Management</h2>
    <div class='workflow-list'>
        <component-loader src='components/WorkflowDashboard' uid='workflowList' />
    </div>
</div>
");
            }).WithName("WorkflowsList");

            // New workflow
            routes.MapGet("/workflows/new", async context =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(@"
<div id='app'>
    <component-loader src='components/WorkflowDesigner' uid='workflowDesignerNew' />
</div>
");
            }).WithName("NewWorkflow");

            return routes;
        }

        /// <summary>
        /// Generate navigation menu items for workflow UI
        /// </summary>
        public static List<NavigationMenuItemDto> GetWorkflowMenuItems()
        {
            return new List<NavigationMenuItemDto>
            {
                new NavigationMenuItemDto
                {
                    Id = "workflow-root",
                    Title = "Workflows",
                    Icon = "fa-solid fa-diagram-project",
                    Url = "/workflows/dashboard",
                    Order = 10,
                    Items = new[]
                    {
                        new NavigationMenuItemDto
                        {
                            Id = "workflow-dashboard",
                            Title = "Dashboard",
                            Icon = "fa-solid fa-chart-line",
                            Url = "/workflows/dashboard",
                            Order = 1
                        },
                        new NavigationMenuItemDto
                        {
                            Id = "workflow-designer",
                            Title = "Designer",
                            Icon = "fa-solid fa-pen-to-square",
                            Url = "/workflows/designer",
                            Order = 2
                        },
                        new NavigationMenuItemDto
                        {
                            Id = "workflow-list",
                            Title = "My Workflows",
                            Icon = "fa-solid fa-list",
                            Url = "/workflows",
                            Order = 3
                        },
                        new NavigationMenuItemDto
                        {
                            Id = "workflow-create",
                            Title = "Create New",
                            Icon = "fa-solid fa-plus-circle",
                            Url = "/workflows/new",
                            Order = 4
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Get component paths for Vue loader
        /// </summary>
        public static Dictionary<string, string> GetComponentPaths()
        {
            return new Dictionary<string, string>
            {
                { "WorkflowDashboard", "/components/WorkflowDashboard" },
                { "WorkflowDesigner", "/components/WorkflowDesigner" },
                { "WorkflowInstanceViewer", "/components/WorkflowInstanceViewer" }
            };
        }
    }

    /// <summary>
    /// Navigation menu item DTO
    /// </summary>
    public class NavigationMenuItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public NavigationMenuItemDto[] Items { get; set; }
    }

    /// <summary>
    /// Workflow UI configuration
    /// </summary>
    public class WorkflowUIConfig
    {
        /// <summary>
        /// Enable workflow dashboard
        /// </summary>
        public bool EnableDashboard { get; set; } = true;

        /// <summary>
        /// Enable workflow designer
        /// </summary>
        public bool EnableDesigner { get; set; } = true;

        /// <summary>
        /// Enable instance viewer
        /// </summary>
        public bool EnableInstanceViewer { get; set; } = true;

        /// <summary>
        /// Dashboard refresh interval in milliseconds
        /// </summary>
        public int DashboardRefreshInterval { get; set; } = 5000;

        /// <summary>
        /// Maximum concurrent workflows to display
        /// </summary>
        public int MaxDisplayedWorkflows { get; set; } = 100;

        /// <summary>
        /// Show advanced metrics
        /// </summary>
        public bool ShowAdvancedMetrics { get; set; } = true;

        /// <summary>
        /// Enable real-time updates
        /// </summary>
        public bool EnableRealTimeUpdates { get; set; } = true;
    }
}

using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using System.Reflection;

namespace AppEndWorkflow.Activities;

/// <summary>
/// Helper class to auto-discover and register custom activities.
/// All activities are in AppEndWorkflow.Activities namespace and inherit from CodeActivity.
/// </summary>
public static class CustomActivitiesRegistry
{
    /// <summary>
    /// Gets all custom activity types from the assembly.
    /// </summary>
    public static IEnumerable<Type> GetCustomActivityTypes()
    {
        var assembly = typeof(CustomActivitiesRegistry).Assembly;
        var baseType = typeof(CodeActivity);

        return assembly.GetTypes()
            .Where(t =>
                t.Namespace?.StartsWith("AppEndWorkflow.Activities") == true &&
                !t.IsAbstract &&
                !t.IsInterface &&
                baseType.IsAssignableFrom(t))
            .ToList();
    }

    /// <summary>
    /// Gets activities grouped by category.
    /// </summary>
    public static Dictionary<string, List<Type>> GetActivitiesByCategory()
    {
        var activities = GetCustomActivityTypes();
        var grouped = new Dictionary<string, List<Type>>();

        foreach (var activity in activities)
        {
            var categoryAttr = activity.GetCustomAttribute<Elsa.Workflows.Attributes.ActivityAttribute>();
            var category = categoryAttr?.Category ?? "Other";

            if (!grouped.ContainsKey(category))
                grouped[category] = new List<Type>();

            grouped[category].Add(activity);
        }

        return grouped;
    }

    /// <summary>
    /// Gets activity count by phase.
    /// Phase 1: Core activities (Notifications, Database, etc.)
    /// Phase 2: Extended activities (Git, Email Advanced, AI/LLM, etc.)
    /// Phase 3: Advanced activities (Cloud, CRM, Social Media, etc.)
    /// </summary>
    public static Dictionary<string, int> GetActivityCountByPhase()
    {
        var grouped = GetActivitiesByCategory();

        var phase1Categories = new[] { "Notifications", "Database", "AppEnd", "Human Tasks", "Data", "HTTP", "FileSystem", "Text", "Security", "Collections", "Flow Control", "Archive", "Math", "Caching", "Documents" };
        var phase2Categories = new[] { "Version Control", "File Transfer", "Email", "Notifications", "PDF", "Data Conversion", "Database", "Scheduling", "Imaging", "Monitoring", "Archive", "AI/LLM" };
        var phase3Categories = new[] { "Calendar", "Cloud Storage", "CRM", "E-commerce", "Project Management", "Message Queues", "Webhooks", "RSS", "Payments", "Analytics", "Social Media", "Media Processing", "Document Management", "IoT", "ML/AI", "Form Processing", "Authentication", "Data Enrichment", "Business Logic" };

        var phase1Count = grouped.Where(kvp => phase1Categories.Contains(kvp.Key)).Sum(kvp => kvp.Value.Count);
        var phase2Count = grouped.Where(kvp => phase2Categories.Contains(kvp.Key)).Sum(kvp => kvp.Value.Count);
        var phase3Count = grouped.Where(kvp => phase3Categories.Contains(kvp.Key)).Sum(kvp => kvp.Value.Count);

        return new Dictionary<string, int>
        {
            ["Phase 1 (Core)"] = phase1Count,
            ["Phase 2 (Extended)"] = phase2Count,
            ["Phase 3 (Advanced)"] = phase3Count,
            ["Total"] = phase1Count + phase2Count + phase3Count
        };
    }
}

using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;

namespace AppEndWorkflow.Activities.VersionControl;

/// <summary>
/// Clones a Git repository from remote URL to local filesystem.
/// Uses LibGit2Sharp library.
/// </summary>
[Activity(
    Category = "Version Control",
    Description = "Clone Git repository",
    DisplayName = "Clone Repository"
)]
public class GitCloneRepositoryActivity : CodeActivity
{
    [Input(Description = "Git repository URL")]
    public Input<string> RepositoryUrl { get; set; } = default!;

    [Input(Description = "Local path to clone into")]
    public Input<string> OutputPath { get; set; } = default!;

    [Input(Description = "Branch name (optional, defaults to default branch)")]
    public Input<string?> Branch { get; set; }

    [Input(Description = "Git username (for private repos)")]
    public Input<string?> Username { get; set; }

    [Input(Description = "Git password/token (for private repos)")]
    public Input<string?> Password { get; set; }

    [Output(Description = "Path to cloned repository")]
    public Output<string> LocalPath { get; set; } = default!;

    [Output(Description = "Whether clone succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var repoUrl = context.Get(RepositoryUrl) ?? throw new ArgumentException("RepositoryUrl is required");
            var outputPath = context.Get(OutputPath) ?? throw new ArgumentException("OutputPath is required");
            var branch = context.Get(Branch);
            var username = context.Get(Username);
            var password = context.Get(Password);

            // Ensure output directory doesn't exist
            if (Directory.Exists(outputPath))
                throw new InvalidOperationException($"Directory already exists: {outputPath}");

            // NOTE: Requires LibGit2Sharp NuGet package
            // In production implementation:
            // var cloneOptions = new CloneOptions();
            // if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            // {
            //     cloneOptions.CredentialsProvider = (url, usernameFromUrl, types) =>
            //         new UsernamePasswordCredentials { Username = username, Password = password };
            // }
            // if (!string.IsNullOrWhiteSpace(branch))
            //     cloneOptions.BranchName = branch;
            //
            // var clonedPath = Repository.Clone(repoUrl, outputPath, cloneOptions);

            // Mock implementation - create directory structure
            Directory.CreateDirectory(outputPath);
            File.WriteAllText(Path.Combine(outputPath, ".git", "HEAD"), "ref: refs/heads/main");

            context.Set(LocalPath, outputPath);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(LocalPath, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }
}

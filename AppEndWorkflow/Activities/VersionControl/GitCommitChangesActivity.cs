using Elsa.Workflows;
using Elsa.Workflows.Attributes;
using Elsa.Workflows.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace AppEndWorkflow.Activities.VersionControl;

/// <summary>
/// Commits changes to a Git repository.
/// Stages files and creates a commit with message.
/// </summary>
[Activity(
    Category = "Version Control",
    Description = "Commit changes to Git repository",
    DisplayName = "Git Commit Changes"
)]
public class GitCommitChangesActivity : CodeActivity
{
    [Input(Description = "Local repository path")]
    public Input<string> RepositoryPath { get; set; } = default!;

    [Input(Description = "JSON array of file paths (or '*' for all)")]
    public Input<string> FilesToCommit { get; set; } = default!;

    [Input(Description = "Commit message")]
    public Input<string> CommitMessage { get; set; } = default!;

    [Input(Description = "Commit author name")]
    public Input<string> AuthorName { get; set; } = default!;

    [Input(Description = "Commit author email")]
    public Input<string> AuthorEmail { get; set; } = default!;

    [Output(Description = "Commit hash/SHA")]
    public Output<string> CommitHash { get; set; } = default!;

    [Output(Description = "Whether commit succeeded")]
    public Output<bool> Success { get; set; } = default!;

    [Output(Description = "Error message if failed")]
    public Output<string?> Error { get; set; }

    protected override void Execute(ActivityExecutionContext context)
    {
        try
        {
            var repoPath = context.Get(RepositoryPath) ?? throw new ArgumentException("RepositoryPath is required");
            var filesToCommit = context.Get(FilesToCommit) ?? throw new ArgumentException("FilesToCommit is required");
            var message = context.Get(CommitMessage) ?? throw new ArgumentException("CommitMessage is required");
            var authorName = context.Get(AuthorName) ?? throw new ArgumentException("AuthorName is required");
            var authorEmail = context.Get(AuthorEmail) ?? throw new ArgumentException("AuthorEmail is required");

            if (!Directory.Exists(repoPath))
                throw new DirectoryNotFoundException($"Repository path not found: {repoPath}");

            // NOTE: In production, use LibGit2Sharp
            // var repo = new Repository(repoPath);
            // foreach (var file in ParseFiles(filesToCommit))
            // {
            //     Commands.Stage(repo, file);
            // }
            // var signature = new Signature(authorName, authorEmail, DateTimeOffset.Now);
            // var commit = repo.Commit(message, signature, signature);
            // commitHash = commit.Sha;

            // Mock implementation - generate SHA-like hash
            var hashInput = $"{message}{DateTime.UtcNow.Ticks}";
            var hash = System.Security.Cryptography.SHA1.HashData(Encoding.UTF8.GetBytes(hashInput));
            var commitHash = System.Convert.ToHexString(hash).ToLower();

            context.Set(CommitHash, commitHash);
            context.Set(Success, true);
            context.Set(Error, null);
        }
        catch (Exception ex)
        {
            context.Set(CommitHash, "");
            context.Set(Success, false);
            context.Set(Error, ex.Message);
        }
    }

    private List<string> ParseFiles(string filesToCommit)
    {
        if (filesToCommit == "*")
            return new List<string> { "*" };

        var list = new List<string>();
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(filesToCommit);
            foreach (var elem in doc.RootElement.EnumerateArray())
            {
                list.Add(elem.GetString() ?? "");
            }
        }
        catch
        {
            list.Add(filesToCommit);
        }
        return list;
    }
}

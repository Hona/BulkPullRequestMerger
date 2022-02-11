using BulkPullRequestApprover.Models;
using Octokit;
using Octokit.Internal;

namespace BulkPullRequestApprover.Services;

public class GithubProvider
{
    private GithubConfig? _config;

    public GitHubClient? Client;
    
    public async Task InitializeAsync()
    {
        _config = await GithubConfig.LoadAsync();
        
        var credentials = new InMemoryCredentialStore(new Credentials(_config.PersonalAccessToken));

        Client = new GitHubClient(new ProductHeaderValue(_config.Username), credentials);
    }

    public async Task<IReadOnlyList<PullRequest>> GetAllPullRequestsAsync()
    {
        var request = new PullRequestRequest();

        // Fetch all open pull requests
        var output = await Client!.PullRequest.GetAllForRepository(_config!.TargetOwner, _config.TargetRepo, request);
        
        Console.WriteLine($"Fetched {output.Count} PRs... ({output.Count(x => x.Mergeable ?? false)}/{output.Count})");

        return output;
    }

    public async Task ApprovePullRequestAsync(PullRequest pr)
    {
        Console.WriteLine($"Approving PR #{pr.Number}");
        
        // Approve the PR
        await Client!.PullRequest.Review.Create(_config!.TargetOwner, _config.TargetRepo, pr.Number, new PullRequestReviewCreate
        {
            Event = PullRequestReviewEvent.Approve,
            Body = "Automatically approved by a script for use while in development."
        });

        Console.WriteLine("Approved");
    }

    public async Task MergePullRequestAsync(PullRequest pr)
    {
        Console.WriteLine($"Merging PR #{pr.Number}");
        
        try
        {
            await Client!.PullRequest.Merge(_config!.TargetOwner, _config.TargetRepo, pr.Number, new MergePullRequest
            {
                MergeMethod = PullRequestMergeMethod.Merge
            });

            Console.WriteLine("Merged");
        }
        catch (Exception e)
        {
            Console.WriteLine("Error merging, most likely a Merge Conflict due to merging a newer version - with a stale old version:");
            Console.WriteLine(" --- " + e.Message);
            Console.WriteLine("Continuing...");
        }
    }

    public async Task ClosePullRequestAsync(PullRequest pr)
    {
        Console.WriteLine($"Closing #{pr.Number}");
        await Client!.PullRequest.Update(_config!.TargetOwner, _config.TargetRepo, pr.Number, new PullRequestUpdate
        {
            State = ItemState.Closed
        });
        
        Console.WriteLine("Closed");
    }
}
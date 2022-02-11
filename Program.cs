using BulkPullRequestApprover.Models;
using BulkPullRequestApprover.Services;
using Cocona;
using Microsoft.Extensions.DependencyInjection;

var builder = CoconaApp.CreateBuilder();
builder.Services.AddTransient<GithubProvider>();

var app = builder.Build();

app.AddSubCommand("prs", x =>
{
    x.AddCommand("merge", async (GithubProvider githubProvider) =>
    {
        // Setup
        await githubProvider.InitializeAsync();

        // Run
        var prs = await githubProvider.GetAllPullRequestsAsync();

        foreach (var pr in prs)
        {
            Console.WriteLine();
            
            if (pr.Comments >= 1)
            {
                Console.WriteLine("PR has >= 1 comment, assuming approved PR, skipping approval step...");
            }
            else
            {
                await githubProvider.ApprovePullRequestAsync(pr);
            }

            await githubProvider.MergePullRequestAsync(pr);
            
            Console.WriteLine("Done");
            Console.WriteLine();
            Console.WriteLine("---");
        }
    });

    x.AddCommand("close", async (GithubProvider githubProvider) =>
    {
        // Setup
        await githubProvider.InitializeAsync();

        // Run
        var prs = await githubProvider.GetAllPullRequestsAsync();

        foreach (var pr in prs)
        {
            Console.WriteLine();
            
            await githubProvider.ClosePullRequestAsync(pr);
            
            Console.WriteLine("Done");
            Console.WriteLine();
            Console.WriteLine("---");
        }
    });
});

app.AddSubCommand("config", x =>
{
    x.AddCommand("update", async (string? username, string? pat, string? targetOwner, string? targetRepo) =>
    {
        GithubConfig config;
        if (GithubConfig.Exists())
        {
            config = await GithubConfig.LoadAsync();
        }
        else
        {
            config = new GithubConfig();
        }

        if (username is not null)
        {
            config.Username = username;
        }

        if (pat is not null)
        {
            config.PersonalAccessToken = pat;
        }

        if (targetOwner is not null)
        {
            config.TargetOwner = targetOwner;
        }

        if (targetRepo is not null)
        {
            config.TargetRepo = targetRepo;
        }

        Console.WriteLine("Saving config");
        await config.SaveAsync();
        
        Console.WriteLine("Done");
    });
});

app.Run();
using System.Text.Json;

namespace BulkPullRequestApprover.Models;

public class GithubConfig
{
    private static readonly string FileUri = Path.Join(Environment.CurrentDirectory, nameof(GithubConfig) + ".json");
    
    public string? Username { get; set; }
    public string? PersonalAccessToken { get; set; }
    public string? TargetOwner { get; set; }
    public string? TargetRepo { get; set; }

    public static async Task<GithubConfig> LoadAsync()
    {
        await using var stream = File.OpenRead(FileUri);
        return await JsonSerializer.DeserializeAsync<GithubConfig>(stream) ?? throw new Exception("No GithubConfig.json found in the current directory");
    }

    public async Task SaveAsync()
    {
        await using var stream = File.OpenWrite(FileUri);
        await JsonSerializer.SerializeAsync(stream, this);
    }

    public static bool Exists() => File.Exists(FileUri);
}
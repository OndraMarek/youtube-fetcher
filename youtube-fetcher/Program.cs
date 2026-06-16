using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using YouTubeFetcher.Services;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string youtubeApiKey = config["YoutubeApiKey"] ?? throw new Exception("Youtube API key not found!");

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("User-Agent", "YouTubeFetcher");

var youtubeService = new YouTubeApiService(client, youtubeApiKey);

try
{
    Console.Write("Enter youtube channel handle (@Handle): @");
    string channelHandle = Console.ReadLine()?.Trim() ?? "";

    string channelId = await youtubeService.GetChannelIdAsync(channelHandle);

    Console.WriteLine($"Saved channel ID: {channelId}");
    Console.WriteLine($"Link to youtube channel: https://www.youtube.com/channel/{channelId}\n");

    Console.Write("Enter year (2005-2026): ");
    string year = Console.ReadLine()?.Trim() ?? "";

    var videos = await youtubeService.GetVideosByYearAsync(channelId, year);

    Console.WriteLine($"\nFound videos ({videos.Count}):");
    foreach (var video in videos)
    {
        Console.WriteLine($"- {video.Snippet.Title}");
        Console.WriteLine($" Link: https://www.youtube.com/watch?v={video.Id.VideoId}\n");
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nError: {ex.Message}");
    Console.ResetColor();
}
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

string channelHandle = "@";
string channelId = "";

while (true)
{
    Console.Write($"""
        1. Change YouTube channel handle
        2. Enter year
        -----------------
        Current YouTube channel handle: {channelHandle}

        Choose an option: 
        """);

    switch (Console.ReadLine())
    {
        case "1":
            await ChangeChannelHandle();
            break;
        case "2":
            if (string.IsNullOrEmpty(channelId))
            {
                Console.WriteLine("Please set the channel handle first (Option 1).");
                break;
            }
            await EnterYear(channelId);
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

async Task ChangeChannelHandle()
{
    try
    {
        Console.Write("Enter youtube channel handle (@Handle): @");
        string newHandle = Console.ReadLine()?.Trim() ?? "";

        channelId = await youtubeService.GetChannelIdAsync(newHandle);
        channelHandle = "@" + newHandle;

        Console.WriteLine($"Saved channel ID: {channelId}");
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }
}

async Task EnterYear(string currentChannelId)
{
    try
    {
        Console.Write("Enter year (2005-2026): ");
        string year = Console.ReadLine()?.Trim() ?? "";

        Console.WriteLine("Fetching videos, please wait...");
        var videos = await youtubeService.GetVideosByYearAsync(currentChannelId, year);

        Console.WriteLine($"\nFound videos ({videos.Count}):");
        foreach (var video in videos)
        {
            Console.WriteLine($"- {video.Snippet.Title}");
            Console.WriteLine($"  Link: https://www.youtube.com/watch?v={video.Id.VideoId}\n");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }
}
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

using HttpClient client = new();

client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

string? youtubeApiKey = config["YoutubeApiKey"] ?? throw new Exception("Youtube API key not found!");

Console.Write("Enter youtube channel handle (@Handle): @");
string channelHandle = Console.ReadLine();

string channelId = await GetChannelID(client, youtubeApiKey, channelHandle);

Console.Write("Enter year (2005-2026): ");
string year = Console.ReadLine();

await GetVideosByYear(client, youtubeApiKey, channelId, year);

static async Task<string> GetChannelID(HttpClient client, string? youtubeApiKey, string channelHandle)
{
    var response = await client.GetFromJsonAsync<YouTubeChannelResponse>($"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle={channelHandle}&key={youtubeApiKey}");
    var channel = response?.Items?.FirstOrDefault();
    if (channel != null)
    {
        Console.WriteLine($"Saved channel ID: {channel.Id}     Link to youtube channel: https://www.youtube.com/channel/{channel.Id}");
        return channel.Id;
    }
    else
    {
        Console.WriteLine("Channel not found.");
        throw new Exception("Channel not found.");
    }
}

static async Task GetVideosByYear(HttpClient client, string? youtubeApiKey, string channelId, string year)
{
    string publishedAfter = $"{year}-01-01T00:00:00Z";
    string publishedBefore = $"{year}-12-31T23:59:59Z";
    string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&channelId={channelId}&maxResults=10&publishedAfter={publishedAfter}&publishedBefore={publishedBefore}&type=video&key={youtubeApiKey}";

    var response = await client.GetFromJsonAsync<YouTubeVideosResponse>(url);

    foreach(var video in response?.Items ?? Enumerable.Empty<YouTubeVideoItem>())
    {
        Console.WriteLine($"Video Title: {video.Snippet.Title}     Link to video: https://www.youtube.com/watch?v={video.Id.VideoId}");
    }
}

public record class YouTubeChannelResponse(List<YouTubeChannelItem> Items);
public record class YouTubeChannelItem(string Id);

public record class YouTubeVideosResponse(List<YouTubeVideoItem> Items);
public record class YouTubeVideoItem(YouTubeVideoId Id, YoutubeVideoTitle Snippet);
public record class YouTubeVideoId(string VideoId);
public record class YoutubeVideoTitle(string Title);
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

string? youtubeApiKey = config["YoutubeApiKey"] ?? throw new Exception("Youtube API key not found!");

Console.Write("Enter youtube channel handle (@Handle): @");
string channelHandle = Console.ReadLine();

using HttpClient client = new();

client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

await GetChannelID(client, youtubeApiKey, channelHandle);

static async Task GetChannelID(HttpClient client, string? youtubeApiKey, string channelHandle)
{
    var response = await client.GetFromJsonAsync<YouTubeResponse>($"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle={channelHandle}&key={youtubeApiKey}");
    var channel = response?.Items?.FirstOrDefault();
    if (channel != null)
    {
        Console.WriteLine($"Saved channel ID: {channel.Id}     Link to youtube channel: https://www.youtube.com/channel/{channel.Id}");
    }
    else
    {
        Console.WriteLine("Channel not found.");
    }
}

public record class YouTubeResponse(List<YouTubeItem> Items);
public record class YouTubeItem(string Id);
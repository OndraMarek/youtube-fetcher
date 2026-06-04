using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

string? youtubeApiKey = config["YoutubeApiKey"] ?? throw new Exception("Youtube API key not found!");

Console.Write("Enter youtube channel handle: ");
string channelHandel = Console.ReadLine();

using HttpClient client = new();

client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

await GetChannelID(client, youtubeApiKey, channelHandel);

static async Task GetChannelID(HttpClient client, string? youtubeApiKey, string channelHandel)
{
    var response = await client.GetFromJsonAsync<YouTubeResponse>($"https://www.googleapis.com/youtube/v3/channels?part=id&forHandle={channelHandel}&key={youtubeApiKey}");

    foreach (var item in response?.Items ?? Enumerable.Empty<YouTubeItem>())
        Console.WriteLine(item.Id);
}

public record class YouTubeResponse(List<YouTubeItem> Items);
public record class YouTubeItem(string Id);
using System.Net.Http.Json;
using YouTubeFetcher.Models;

namespace YouTubeDataFetcher.Services;

public class YouTubeApiService
{
    private readonly HttpClient _client;
    private readonly string _apiKey;
    private const string BaseUrl = "https://www.googleapis.com/youtube/v3";

    public YouTubeApiService(HttpClient client, string apiKey)
    {
        _client = client;
        _apiKey = apiKey;
    }

    public async Task<string> GetChannelIdAsync(string channelHandle)
    {
        string url = $"{BaseUrl}/channels?part=id&forHandle={channelHandle}&key={_apiKey}";
        var response = await _client.GetFromJsonAsync<YouTubeChannelResponse>(url);
        var channel = response?.Items?.FirstOrDefault();

        if (channel != null)
        {
            return channel.Id;
        }

        throw new Exception($"Channel with handle @{channelHandle} was not found.");
    }

    public async Task<List<YouTubeVideoItem>> GetVideosByYearAsync(string channelId, string year)
    {
        string publishedAfter = $"{year}-01-01T00:00:00Z";
        string publishedBefore = $"{year}-12-31T23:59:59Z";
        string url = $"{BaseUrl}/search?part=snippet&channelId={channelId}&maxResults=10&publishedAfter={publishedAfter}&publishedBefore={publishedBefore}&type=video&key={_apiKey}";

        var response = await _client.GetFromJsonAsync<YouTubeVideosResponse>(url);

        return response?.Items ?? [];
    }
}
using System.Net.Http.Json;
using System.Text.Json;
using YouTubeFetcher.Models;

namespace YouTubeFetcher.Services;

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

        int maxRetries = 3;
        int delayMilliseconds = 2000;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<YouTubeVideosResponse>();
                return data?.Items ?? [];
            }

            if (attempt == maxRetries)
            {
                await GetErrorMessage(response, maxRetries);
            }

            await Task.Delay(delayMilliseconds);
        }

        return [];
    }

    private static async Task GetErrorMessage(HttpResponseMessage response, int maxRetries)
    {
        string errorJson = await response.Content.ReadAsStringAsync();
        try
        {
            var jsonDoc = JsonDocument.Parse(errorJson);
            var errorElement = jsonDoc.RootElement.GetProperty("error");
            string errorMessage = errorElement.GetProperty("message").GetString() ?? "Unknown error";
            string errorReason = errorElement.GetProperty("errors")[0].GetProperty("reason").GetString() ?? "Unknown reason";

            throw new Exception($"Google API permanently rejected the request (after {maxRetries} attempts).\nStatus: {response.StatusCode}\nReason: {errorReason}\nDetail: {errorMessage}");
        }
        catch (JsonException)
        {
            throw new Exception($"API error after {maxRetries} attempts. Status: {response.StatusCode}\nRaw data: {errorJson}");
        }
    }
}
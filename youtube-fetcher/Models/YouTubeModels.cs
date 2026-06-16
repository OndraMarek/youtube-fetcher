namespace YouTubeFetcher.Models;

public record class YouTubeChannelResponse(List<YouTubeChannelItem> Items);
public record class YouTubeChannelItem(string Id);

public record class YouTubeVideosResponse(List<YouTubeVideoItem> Items);
public record class YouTubeVideoItem(YouTubeVideoId Id, YoutubeVideoTitle Snippet);
public record class YouTubeVideoId(string VideoId);
public record class YoutubeVideoTitle(string Title);
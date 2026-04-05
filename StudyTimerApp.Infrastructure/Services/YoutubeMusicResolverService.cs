using System;
using System.Linq;
using System.Threading.Tasks;
using StudyTimerApp.Core.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace StudyTimerApp.Infrastructure.Services;

public class YoutubeMusicResolverService : IMusicResolverService
{
    private readonly YoutubeClient _youtube;

    public YoutubeMusicResolverService()
    {
        _youtube = new YoutubeClient();
    }

    public async Task<TrackInfo?> ResolveTrackAsync(string url)
    {
        try
        {
            var videoId = YoutubeExplode.Videos.VideoId.TryParse(url);
            if (videoId == null)
            {
                return null;
            }

            var video = await _youtube.Videos.GetAsync(videoId.Value);
            var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoId.Value);
            
            // Get highest quality audio-only stream
            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
            
            if (streamInfo == null)
            {
                return null;
            }

            var thumbnailUrl = video.Thumbnails.OrderByDescending(t => t.Resolution.Area).FirstOrDefault()?.Url;

            return new TrackInfo
            {
                Title = video.Title,
                Author = video.Author.ChannelTitle,
                ThumbnailUrl = thumbnailUrl ?? string.Empty,
                StreamUrl = streamInfo.Url
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}

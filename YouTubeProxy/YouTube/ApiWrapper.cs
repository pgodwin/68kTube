using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;
using YouTubeProxy.EncodingEngine;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.YouTube
{
    public class ApiWrapper
    {
        public async Task<List<SearchResultModel>> Search(string search)
        {
            var service = YouTubeApi.GetYouTubeApi();

            var searchListRequest = service.Search.List("snippet");
            searchListRequest.Fields = "items(id,kind,snippet(channelId,channelTitle,description,publishedAt,thumbnails,title)),nextPageToken,pageInfo,prevPageToken";
            searchListRequest.Q = search;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            searchListRequest.Type = "video";
            // Only short videos are returned, this proxy ain't going to be encoding all day!
            searchListRequest.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Short__;
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videos = searchListResponse.Items;//.Where(i => i.Id.Kind == "youtube#video");

            var results = videos.Select(v => new SearchResultModel
            {
                VideoId = v.Id.VideoId,
                Title = v.Snippet.Title,
                Description = v.Snippet.Description,
                Url = "",
                ThumbUrl = v.Snippet.Thumbnails.Default__.Url.Replace("https://", "http://")
            });

            return results.ToList();
        }

        public static WatchVideoModel GetVideoDetails(EncodeDetail encodeDetail)
        {

            var service = YouTubeApi.GetYouTubeApi();

            var videoRequest = service.Videos.List("snippet,statistics");
            videoRequest.Id = encodeDetail.VideoId;
            var videoResult = videoRequest.Execute();

            if (videoResult.Items.Count == 0)
                return null;

            var item = videoResult.Items[0];
            var watchDetails = new WatchVideoModel() {
                Title = item.Snippet.Title,
                Description = item.Snippet.Description,
                Channel = item.Snippet.ChannelTitle,
                Views = (long)item.Statistics.ViewCount.GetValueOrDefault(0),
                DateUploaded = item.Snippet.PublishedAt.GetValueOrDefault(),
                DownloadUrl = "/api/download/mov/" + encodeDetail.FileId.ToString(),
                PlayUrl = "/api/download/mov/" + encodeDetail.FileId.ToString()
            };

            return watchDetails;


        }

        public static EncodeDetail GetEncodeDetails(string videoId, string profile)
        {
            var youTube = VideoLibrary.YouTube.Default;
            // Grab a low res video compatible with quicktime...
            var video = youTube.GetAllVideos("http://www.youtube.com/watch?v=" + videoId).FirstOrDefault(v => v.Format == VideoFormat.Mp4 && v.Resolution <= 360);

            var url = video.GetUri();

            return new EncodeDetail(videoId, profile, url);
        }
    }
}

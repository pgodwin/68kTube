using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.Controllers.MacApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {

        public async Task<MacDeliminatedResult<SearchResultModel>> Get(string id)
        {
            var service = YouTubeApi.GetYouTubeApi();

            var searchListRequest = service.Search.List("snippet");
            searchListRequest.Fields = "items(id,kind,snippet(channelId,channelTitle,description,publishedAt,thumbnails,title)),nextPageToken,pageInfo,prevPageToken";
            searchListRequest.Q = id; // Replace with your search term.
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            searchListRequest.Type = "video";
            searchListRequest.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Short__;
            searchListRequest.MaxResults = 10;
         
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var videos = searchListResponse.Items;//.Where(i => i.Id.Kind == "youtube#video");

            var results = videos.Select(v => new SearchResultModel
            {
                VideoId = v.Id.VideoId,
                Title = v.Snippet.Title,
                Description = v.Snippet.Description,
                Url = "",
                ThumbUrl = v.Snippet.Thumbnails.Default__.Url.Replace("https://", "http://") // TODO - proxy the thumbs
            });

            return new MacDeliminatedResult<SearchResultModel>(results);
        }
    }
}

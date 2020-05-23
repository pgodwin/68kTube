using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.Controllers.Proxy
{
    public class SearchController : Controller
    {
        public IActionResult Index(string q, string page)
        {
            var service = YouTubeApi.GetYouTubeApi();
            var searchListRequest = service.Search.List("snippet");
            searchListRequest.Fields = "items(id,kind,snippet(channelId,channelTitle,description,publishedAt,thumbnails,title)),nextPageToken,pageInfo,prevPageToken";
            searchListRequest.Q = q; // Replace with your search term.
            searchListRequest.PageToken = page;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;
            searchListRequest.Type = "video";
            //searchListRequest.VideoDuration = SearchResource.ListRequest.VideoDurationEnum.Short__;
            searchListRequest.MaxResults = 10;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.ExecuteAsync().Result;

            var videos = searchListResponse.Items;//.Where(i => i.Id.Kind == "youtube#video");

            var result = new YouTubeProxy.Models.SearchViewModel
            {
                SearchTerms = q,
                NextPageToken = searchListResponse.NextPageToken,
                PreviousPageToken = searchListResponse.PrevPageToken,
                Results = videos.Select(v => new SearchResultModel
                {
                    VideoId = v.Id.VideoId,
                    Title = v.Snippet.Title,
                    Description = v.Snippet.Description,
                    Url = "",
                    ThumbUrl = v.Snippet.Thumbnails.Default__.Url.Replace("https://", "http://")
                }).ToList()
            };

            return View(result);
        }
    }
}
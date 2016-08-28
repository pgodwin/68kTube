using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Helpers
{
    public class YouTubeApi
    {
        public static YouTubeService GetYouTubeApi()
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = File.ReadAllText("apikey.txt"),
                ApplicationName = "68kTube"
            });

            return youtubeService;
        }
    }
}

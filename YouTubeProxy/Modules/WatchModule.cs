using Nancy;
using YouTubeProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy;
using YouTubeProxy.EncodingEngine;

namespace SelfHost.Modules
{
    public class WatchModule : NancyModule
    {
        public WatchModule()
        {
            Get["/watch/{videoid}"] = Watch;
        }

        private dynamic Watch(dynamic parameters)
        {
            var profile = Context.Request.Cookies.ContainsKey("encoder") ? Context.Request.Cookies["encoder"] : Settings.DefaultProfile;
            var videoId = parameters.videoid;

            var encodeDetails = EncodeStore.GetStore().GetDetails(videoId, profile);

            if (encodeDetails == null)
            {
                encodeDetails = YouTubeProxy.YouTube.ApiWrapper.GetEncodeDetails(videoId, profile);
                EncodingQueue.GetQueue().AddToQueue(encodeDetails); 
            }


            var result = YouTubeProxy.YouTube.ApiWrapper.GetVideoDetails(encodeDetails);

            result.EncodeDetails = encodeDetails;
            

            return View["Index", result];//, new Video() { Title = "Test" }];
        }
    }
}

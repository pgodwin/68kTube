using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTubeProxy.EncodingEngine;

namespace YouTubeProxy.Controllers.Proxy
{
  
    public class WatchController : Controller
    {
        [Route("{controller}/{videoId}")]
        public IActionResult Index(string videoId)
        {
            
            var profile = Request.Cookies.ContainsKey("encoder") ? Request.Cookies["encoder"] : Settings.DefaultProfile;
  
            var encodeDetails = EncodeStore.GetStore().GetDetails(videoId, profile);

            if (encodeDetails == null)
            {
                encodeDetails = YouTubeProxy.YouTube.ApiWrapper.GetEncodeDetails(videoId, profile);
                EncodingQueue.GetQueue().AddToQueue(encodeDetails);
            }


            var result = YouTubeProxy.YouTube.ApiWrapper.GetVideoDetails(encodeDetails);

            result.EncodeDetails = encodeDetails;


            return View(result);
        }
    }
}
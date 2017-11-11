using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Http;
using VideoLibrary;
using YouTubeProxy.EncodingEngine;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.Controllers.MacApi
{
    public class VideoController : ApiController
    {
        public MacDeliminatedResult<ConversionStatusModel> Get(string id, string profile)
        {
            var encodeDetails = EncodeStore.GetStore().GetDetails(id, profile);

            if (encodeDetails == null)
            {
                // This is a video we haven't seen before,
                // Queue it up...
                // Get and queue the video
                encodeDetails = YouTube.ApiWrapper.GetEncodeDetails(id, profile);
                EncodingQueue.GetQueue().AddToQueue(encodeDetails);
            }

            return new MacDeliminatedResult<ConversionStatusModel>(new ConversionStatusModel
            {
                VideoId = encodeDetails.VideoId,
                Status = encodeDetails.Progress.Status,
                Error = encodeDetails.Progress.Error,
                Progress = encodeDetails.Progress.Progress,
                Duration = encodeDetails.Progress.Duration
            });
            
            
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using VideoLibrary;
using YouTubeProxy;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.Controllers
{
    public class VideoController : ApiController
    {
        public MacDeliminatedResult<ConversionStatusModel> Get(string id, string profile)
        {
            if (File.Exists(@".\videos\" + id + ".mov"))
            {
                return new MacDeliminatedResult<ConversionStatusModel>(new ConversionStatusModel()
                {
                    VideoId = id,
                    Status = ConversionStatusModel.StatusCodes.ReadyForDownload
                });
            }
            if (GlobalStatus.ConversionStatus.ContainsKey(id))
            {
                return new MacDeliminatedResult<ConversionStatusModel>(GlobalStatus.ConversionStatus[id]);
            }

            // Start the encode
            var status = new ConversionStatusModel()
            {
                VideoId = id,
                Status = ConversionStatusModel.StatusCodes.Encoding,
                Error = ""
            };

            GlobalStatus.ConversionStatus.TryAdd(id, status);

            var youTube = YouTube.Default;
            var video = youTube.GetAllVideos("http://www.youtube.com/watch?v=" + id).FirstOrDefault(v=>v.Format == VideoFormat.Mp4 && v.Resolution <= 360);
                       
            if (video == null)
            {
                status.Status = ConversionStatusModel.StatusCodes.Error;
                return new MacDeliminatedResult<ConversionStatusModel>(status);
            }
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                VideoEncoder.StartEncoding(profile, id, video.GetUri());
            }).Start();

            return new MacDeliminatedResult<ConversionStatusModel>(status);
            
        }


    }
}

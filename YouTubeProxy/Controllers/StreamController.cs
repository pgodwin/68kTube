using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using VideoLibrary;

namespace YouTubeProxy.Controllers
{
    public class StreamController : ApiController
    {
        
        public string testARgs = "-re -y -i \"{0}\" -vf \"scale=(iw* sar)*min(352/(iw* sar)\\,288/ih):ih* min(352/(iw* sar)\\,288/ih), pad=352:288:(352-iw* min(352/iw\\,288/ih))/2:(288-ih* min(352/iw\\,288/ih))/2\" -vcodec h263 -b:v 300k -r 15 -acodec pcm_alaw -ar 8000 -ac 1 -muxdelay 0.1 -f rtsp rtsp://stream:stream@127.0.0.1:554/{1}.sdp";


        /// <summary>
        /// Temporary Test code for RTSP Streaming
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MacDeliminatedResult<StreamStatus> Get(string id)
        {
            
            var youTube = YouTube.Default;
            var video = youTube.GetAllVideos("http://www.youtube.com/watch?v=" + id).FirstOrDefault(v => v.Format == VideoFormat.Mp4 && v.Resolution <= 360);

            if (video != null)
            {
                var sourceUri = video.GetUri();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                string arguments = string.Format(testARgs, sourceUri, id);
                startInfo.FileName = "ffmpeg.exe";
                startInfo.Arguments = arguments;

                Console.WriteLine("Running: {0} {1}", startInfo.FileName, arguments);
                using (Process process = new Process() { StartInfo = startInfo })
                {
                    process.Start();
                }

                // Give ffmpeg a chance to launch
                Thread.Sleep(500);

                return new MacDeliminatedResult<StreamStatus>(new StreamStatus
                {
                    VideoId = id,
                    Success = true,
                    RtspUrl = "rtsp://127.0.0.1:554/" + id + ".sdp"
                });
                
            }

            return new MacDeliminatedResult<StreamStatus>(new StreamStatus
            {
                VideoId = id,
                Success = false
            });

        }

        
    }

    public class StreamStatus
    {
        [ColumnOrder(1)]
        public bool Success { get; set; }

        [ColumnOrder(2)]
        public string VideoId { get; set; }

        [ColumnOrder(3)]
        public string RtspUrl { get; set; }
    }
}

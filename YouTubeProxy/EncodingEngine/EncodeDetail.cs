using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.EncodingEngine
{
    public class EncodeDetail
    {

        public EncodeDetail(string videoId, string profile, string url)
        {
            FileId = Guid.NewGuid();

            VideoId = videoId;
            SourceUrl = url;
            EncodeProfileName = profile;

            DestinationFileName = FileId.ToString();
            Progress = new EncodingEngine.EncodeProgress()
            {
                Status = StatusCodes.Queued
            };
            
        }

        public Guid FileId { get; set; }

        public string VideoId { get; set; }

        public string EncodeProfileName { get; set; }

        public string SourceUrl { get; set; }

        public string DestinationFileName { get; set; }

        public EncodeProgress Progress { get; set; }

    }

    public class EncodeProgress
    {
        public int Progress { get; set; }
        public string Error { get; set; }
        public StatusCodes Status { get; set; }
        public int Duration { get; internal set; }
    }

    public enum StatusCodes
    {
        Error,
        Encoding,
        ReadyForDownload,
        Queued
    }
}

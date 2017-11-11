using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy.EncodingEngine;

namespace YouTubeProxy.Models
{
    public class WatchVideoModel : BaseViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Int64 Views { get; set; }
        public DateTime DateUploaded { get; set; }
        public string User { get; set; }
        public string Channel { get; set; }

        public string PlayUrl { get; set; }

        public string DownloadUrl { get; set; }

        public string ChannelIconUrl { get; set; }

        public List<SearchResultModel> RelatedVideos { get; set; }

        public EncodeDetail EncodeDetails { get; set; }

        
    }
}

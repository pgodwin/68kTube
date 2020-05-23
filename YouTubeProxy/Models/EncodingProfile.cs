using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Models
{
    public class EncodingProfile
    {
        public string Name { get; set; }

        public string FriendlyName { get; set; }
        public string Description { get; set; }
        
        public string Exe { get; set; }
        public string Arguments { get; set; }
        public string OutputExtension { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public float Scale { get; set; } = 1.0f;

        public string ScaleMode { get; set; } = "tofit";

    }
}

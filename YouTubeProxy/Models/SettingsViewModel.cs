using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Models
{
    public class SettingsViewModel : BaseViewModel
    {
        public string CurrentProfile { get; set; }
        public bool DoubleSize { get; internal set; }
        public List<EncodingProfile> Profiles { get; set; }
    }
}

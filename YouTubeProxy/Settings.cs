using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy
{
    /// <summary>
    /// Provides simple, quick access to configured settings
    /// </summary>
    public static class Settings
    {
        public static string ListeningUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["baseAddress"] + ":" + ConfigurationManager.AppSettings["port"];
            }
        }


        public static string RtspServer
        {
            get
            {
                return ConfigurationManager.AppSettings["rtspServer"];
            }
        }
    }
}

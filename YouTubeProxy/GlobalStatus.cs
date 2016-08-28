using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy
{
    public static class GlobalStatus
    {
        private static ConcurrentDictionary<string, Models.ConversionStatusModel> _conversionStatus = new ConcurrentDictionary<string, Models.ConversionStatusModel>();
        public static ConcurrentDictionary<string, Models.ConversionStatusModel> ConversionStatus
        {
            get
            {
                return _conversionStatus;
            }
        }  
    }
}

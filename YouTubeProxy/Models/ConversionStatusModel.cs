using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeProxy.EncodingEngine;

namespace YouTubeProxy.Models
{
    public class ConversionStatusModel
    {
        [ColumnOrder(1)]
        public string VideoId { get; set; }

        [ColumnOrder(2)]
        public StatusCodes Status { get; set; }

        [ColumnOrder(3)]
        public string Error { get; set; }

        /// <summary>
        /// Duration in seconds
        /// </summary>
        [ColumnOrder(4)]
        public int Duration { get; set; }

        [ColumnOrder(5)]
        public int Progress { get; set; }
        public static object StatusCodes { get; internal set; }
    }
}

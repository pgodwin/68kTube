using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Models
{
    public class SearchResultModel
    {

        [ColumnOrderAttribute(1)]
        public string VideoId { get; set; }

        [ColumnOrderAttribute(2)]
        public string Title { get; set; }

        [ColumnOrderAttribute(3)]
        public string Description { get; set; }

        [ColumnOrderAttribute(4)]
        public string Url { get; set; }

        [ColumnOrderAttribute(5)]
        public string ThumbUrl { get; set; }
    }
}

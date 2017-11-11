using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Models
{
    public class SearchViewModel : BaseViewModel
    {
        public string SearchTerms { get; set; }

        public List<SearchResultModel> Results { get; set; }

        public string NextPageToken { get; set; }

        public string PreviousPageToken { get; set; }
    }
}

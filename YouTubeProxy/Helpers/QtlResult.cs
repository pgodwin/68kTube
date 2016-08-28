using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Helpers
{
    public class QtlResult : HttpResponseMessage
    {

        /*  <?xml version="1.0"?>
         <?quicktime type="application/x-quicktime-media-link"?>
         <embed src="sample.mov" />  */

        public QtlResult(string url) : base(System.Net.HttpStatusCode.OK)
        {
            var qtlTemplate = @"<?xml version=""1.0""?>
<? quicktime type=""application/x-quicktime-media-link"" ?>
<embed src=""{0}"" mimetype=""video/quicktime"" />";

            var qtlFile = string.Format(qtlTemplate, url);
            // i dont see a reference to iso-8859-1, so maybe try this...
            var iso88591data = Encoding.GetEncoding("ISO-8859-1").GetBytes(qtlFile);

            this.Content = new ByteArrayContent(iso88591data);

            // Set the content headers
            this.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-quicktimeplayer"); 
        }

    }
}

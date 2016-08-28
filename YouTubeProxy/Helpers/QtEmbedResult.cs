using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy.Helpers
{
    public class QtEmbedResult : HttpResponseMessage
    {

        /*  <?xml version="1.0"?>
         <?quicktime type="application/x-quicktime-media-link"?>
         <embed src="sample.mov" />  */

        public QtEmbedResult(string url) : base(System.Net.HttpStatusCode.OK)
        {
            var qtlTemplate = @"<html><body><EMBED
SRC=""{0}""
HEIGHT=""240"" WIDTH=""320""
TYPE=""video/quicktime""
PLUGINSPAGE=""http://www.apple.com/quicktime/download/""
></EMBED></body>";

            var qtlFile = string.Format(qtlTemplate, url);
            // i dont see a reference to iso-8859-1, so maybe try this...
            var iso88591data = Encoding.GetEncoding(10000).GetBytes(qtlFile);

            this.Content = new ByteArrayContent(iso88591data);

            // Set the content headers
            this.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");// { CharSet = "macintosh" };
        }

    }
}

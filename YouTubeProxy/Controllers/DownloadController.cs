using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using YouTubeProxy.Helpers;

namespace YouTubeProxy.Controllers
{
    public class DownloadController : ApiController
    {
        [HttpGet]
        [ActionName("qtl")]
        public QtlResult GetQtl(string id)
        {
            var uri = new UriBuilder(Url.Link("DownloadApi", new { controller = "Download", action = "mov", id = id }));
            uri.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            return new QtlResult(uri.ToString());
        }

        [ActionName("html")]
        public QtEmbedResult GetHtml(string id)
        {
            //return new QtEmbedResult(Url.Link("DefaultApi", new { controller = "Download", action = "mov", id = id }));
            var uri = new UriBuilder(Url.Link("DownloadApi", new { controller = "Download", action = "mov", id = id }));
            uri.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]); 
            return new QtEmbedResult(uri.ToString());
        }


        [ActionName("mov")]
        public HttpResponseMessage GetMov(string id)
        {
            //if (!GlobalStatus.ConversionStatus.ContainsKey(id))
              //  return new HttpResponseMessage(HttpStatusCode.NotFound);

            //var status = GlobalStatus.ConversionStatus[id];

           // if (status.Status != Models.ConversionStatusModel.StatusCodes.ReadyForDownload)
             //   return new HttpResponseMessage(HttpStatusCode.NotFound);

            //var path = @".\video\" + id;

            var path = Directory.EnumerateFiles(".\\video\\", id + ".*").FirstOrDefault(); ;

            if (path == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("video/quicktime");
            result.Content.Headers.ContentLength = stream.Length;
            return result;
            
        }

        
    }
}

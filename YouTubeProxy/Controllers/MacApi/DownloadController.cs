using Microsoft.AspNetCore.Mvc;
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
using YouTubeProxy.EncodingEngine;
using YouTubeProxy.Helpers;

namespace YouTubeProxy.Controllers.MacApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : Controller
    {
        [HttpGet]
        [ActionName("qtl")]
        [HttpGet("qtl/{id}")]
        public QtlResult GetQtl(string id)
        {
            var uri = new UriBuilder(Url.Link("DownloadApi", new { controller = "Download", action = "mov", id = id }));
            uri.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            return new QtlResult(uri.ToString());
        }

        [ActionName("html")]
        [HttpGet("html/{id}")]
        public QtEmbedResult GetHtml(string id)
        {
            //return new QtEmbedResult(Url.Link("DefaultApi", new { controller = "Download", action = "mov", id = id }));
            var uri = new UriBuilder(Url.Link("DownloadApi", new { controller = "Download", action = "mov", id = id }));
            uri.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]); 
            return new QtEmbedResult(uri.ToString());
        }


        [ActionName("mov")]
        [HttpGet("mov/{id}")]
        public IActionResult GetMov(string id)
        {
            Console.WriteLine("Movie requested {0}", id);

            var idGuid = Guid.Parse(id);

            // Check the encode store
            // This is gonna be slow if we have many encodes...
            var details = EncodeStore.GetStore().GetByFileId(idGuid);
            
            if (details == null)
                return NotFound();

            var path = details.DestinationFileName;

            //var path = Directory.EnumerateFiles(Settings.EncodeLocation, id + ".*").FirstOrDefault(); ;

            if (path == null)
                return NotFound();
            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            
            return File(stream, details.Profile.MimeType);

            
        }

        
        
    }
}

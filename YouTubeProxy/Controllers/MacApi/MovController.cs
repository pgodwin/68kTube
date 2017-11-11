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

namespace YouTubeProxy.Controllers.MacApi
{
    public class RefMovController : ApiController
    {

        
        public HttpResponseMessage Get(string id)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            //var stream = new FileStream("RefTest.mov", FileMode.Open, FileAccess.Read, FileShare.Read);
            //using (var refMovie = new MemoryStream())
            var refMovie = new MemoryStream();
            {
                var movieBytes = File.ReadAllBytes("RefTest.mov");

                refMovie.Write(movieBytes, 0, movieBytes.Length);

                // Seek to the url positionv
                refMovie.Seek(1776, SeekOrigin.Begin);
                // write our url
                var rtspUrl = "rtsp://" + ConfigurationManager.AppSettings["rtspServer"] + "/" + id + ".sdp";
                var urlBytes = Encoding.UTF8.GetBytes(rtspUrl);
                refMovie.Write(urlBytes, 0, urlBytes.Length);
                refMovie.Flush();
                refMovie.Seek(0, SeekOrigin.Begin);

                result.Content = new StreamContent(refMovie);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("video/quicktime");
                result.Content.Headers.ContentLength = refMovie.Length;
                return result;
            }



        }
    }
}

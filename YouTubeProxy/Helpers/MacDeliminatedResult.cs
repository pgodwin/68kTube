using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeProxy
{
    public class MacDeliminatedResult<T> : HttpResponseMessage
    {
        private string _data;
        public MacDeliminatedResult(T value) : base (System.Net.HttpStatusCode.OK)
        {
            this.Data = (new List<T>() { value }).Serialize();
        }

        public MacDeliminatedResult(IEnumerable<T> value) : base(System.Net.HttpStatusCode.OK)
        {
            this.Data = value.Serialize();
           
        }
        
        /// <summary>
        /// Grabs the string data, changes the encoding to MacRoman and sets the Content to the supplied values
        /// </summary>
        public void SetContent()
        {
            var resultbytes = Encoding.UTF8.GetBytes(Data);
            // Encoding 10000 is MacRoman
            var macBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(10000), resultbytes);


            this.Content = new ByteArrayContent(macBytes);

            // Set the content headers
            this.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain") { CharSet = "macintosh" }; //"text/plain; charset=macintosh";
        }

        public string Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                SetContent();
            }
        }
    }
}

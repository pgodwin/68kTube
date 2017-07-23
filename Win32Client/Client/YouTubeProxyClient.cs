using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Win32Client.Helpers;
using Win32Client.Models;

namespace Win32Client.Client
{
    public class YouTubeProxyClient
    {
        private WebClient client;
        const char separator = '|';



        public YouTubeProxyClient(string baseUrl)
        {
            client = new WebClient();
            client.BaseAddress = baseUrl;
        }

        public List<SearchResultModel> Search(string terms)
        {
            var csvResult = client.DownloadData(string.Format("api/Search/{0}", Uri.EscapeDataString(terms))).MacRomanToString();

            var results = MacDeserialiser.Deserialize<SearchResultModel>(csvResult);

            return results.ToList();
        }

        public List<ConversionStatusModel> GetVideo(string videoId, string profile)
        {
            var csvResult = client.DownloadData(string.Format("api/Video/{0}/?profile={1}", videoId, profile)).MacRomanToString();

            var results = MacDeserialiser.Deserialize<ConversionStatusModel>(csvResult);

            return results.ToList();
        }

        public List<StreamStatus> GetRtspStream(string videoId)
        {
            var csvResult = client.DownloadData(string.Format("api/Stream/{0}/", videoId)).MacRomanToString();

            var results = MacDeserialiser.Deserialize<StreamStatus>(csvResult);

            return results.ToList();
        }
    }
}

using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;

namespace YouTubeProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            Helpers.BmffPortabilityFactory _pf = new Helpers.BmffPortabilityFactory();

            string baseAddress = ConfigurationManager.AppSettings["baseAddress"];
            string port = ConfigurationManager.AppSettings["port"];

            var address = baseAddress + ":" + port;

            // Start OWIN host 
            using (var owin = WebApp.Start<Startup>(url: address))
            {
                
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
            }
                      
        
        }
    }
}

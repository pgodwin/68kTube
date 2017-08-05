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
            // Used later to ensure the atoms are in the correct order for QT3
            Helpers.BmffPortabilityFactory _pf = new Helpers.BmffPortabilityFactory();
                        

            var address = Settings.ListeningUrl;

            // Start OWIN host 
            try
            {
                using (var owin = WebApp.Start<Startup>(url: address))
                {
                    Console.WriteLine("Listening on {0}", address);
                    Console.WriteLine("Press ENTER to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to create WebServer on {0}. Are you running as an Administrator?", address);
                Console.ReadLine();
            }
                      
        
        }
    }
}

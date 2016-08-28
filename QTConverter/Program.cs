using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QTConverter
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Cmd QuickTime Converter");
            
            // Args 0 is the exe
            // Args 1 is the profile
            // Args 2 is the url
            // Args 3 is optional and is the output filename

            if (args.Any(s => s.Contains("/settings")))
            {
                var profileName = args[args.ToList().IndexOf("/settings") + 1];
                AdjustSettings(profileName);
                return;
            }


            if (args.Length < 3)
            {
                PrintHelp();
                return;
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            string profile = args[0];
            string url = args[1];
            string output = args[2];

            Console.WriteLine("Converting {0} with profile {1}", url, profile);
            Convert(profile, url, output);
            stopwatch.Stop();

            Console.WriteLine("Completed in {0} seconds.", stopwatch.Elapsed.TotalSeconds);
        }

        private static void Convert(string profile, string url, string output)
        {
            //StartThread(() => QTCommon.Converter.Convert(profile, url, output));
            QTCommon.Converter.Convert(profile, url, output);
        }

        private static void AdjustSettings(string profileName)
        {
            StartThread(() => QTCommon.Converter.EditSettings(profileName));
        }

        private static void StartThread(ThreadStart method)
        {
            var t = new Thread(method);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private static void PrintHelp()
        {

            Console.WriteLine("Usage:");
            Console.WriteLine("QTConverter.exe [Profile.Xml] [URL] [OutputName]");
            Console.WriteLine("");

            Console.WriteLine("--- or to adjust settings ----");
            Console.WriteLine("QTConverter.exe /settings [profile.xml]");

            Console.WriteLine();
            Console.WriteLine("Example:");
            Console.WriteLine("QTConverter.exe Quadra.xml test.mov test.mov");
            

        }
    }
}

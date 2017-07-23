using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YouTubeProxy.Models;

namespace YouTubeProxy.Helpers
{
   
    public class VideoEncoder
    {

        /// <summary>
        /// DEFINE YOUR ENCODING PROFILES HERE!
        /// </summary>
        public static Dictionary<string, EncodingProfile> profiles = new Dictionary<string, EncodingProfile>()
        {
            { "video1", new EncodingProfile()
                {
                    Name = "video1",
                    Exe = "QTConverter.exe",
                    Arguments = @"profiles\video1.xml {0} {1}.mov"
                }
            },
            {
                "cinepak", new EncodingProfile()
                {
                    Name = "cinepak",
                    Exe = "QTConverter.exe",
                    Arguments = @"profiles\cinepak.xml {0} {1}.mov"
                }
            },
            { "h263", new EncodingProfile()
                {
                    Name = "h263",
                    Exe = "ffmpeg.exe",
                    Arguments = @"-y -i ""{0}"" -vf ""scale=(iw*sar)*min(352/(iw*sar)\,288/ih):ih*min(352/(iw*sar)\,288/ih), pad=352:288:(352-iw*min(352/iw\,288/ih))/2:(288-ih*min(352/iw\,288/ih))/2""  -vcodec h263 -b:v 500k -r 15 -acodec adpcm_ima_qt -ar 22050 -ac 1 -movflags +faststart ""{1}.mov"""
                }
            },
            { "sorrensonFFMPEG", new EncodingProfile()
                {
                    Name = "sorrenson",
                    Exe = "ffmpeg.exe",
                    Arguments = "-y -i \"{0}\" -vf scale=320:-1 -vcodec svq1 -b:v 500k -acodec adpcm_ima_qt -ar 22050 -ac 2 -movflags +faststart \"{1}.mov\""
                }
            },
            { "sorrenson", new EncodingProfile()
                {
                    Name = "sorrenson",
                    Exe = "qtconverter.exe",
                    Arguments = "quadra.xml {0} {1}.mov"
                }
            },
             { "mpeg1", new EncodingProfile()
                {
                    Name = "mpeg1",
                    Exe = "ffmpeg.exe",
                    Arguments = "-y -i \"{0}\" -vf scale=-1:360 -vcodec mpeg1video -b:v 500k -acodec mp2 -b:a 96k  \"{1}.mpeg\""
                }
            },
            { "mpeg4", new EncodingProfile()
                {
                    Name = "mpeg4",
                    Exe = "ffmpeg.exe",
                    Arguments = "-y -i \"{0}\" -vf scale=-1:360 -vcodec mpeg4 -b:v 500k -acodec aac -b:a 96k -movflags +faststart \"{1}.mp4\""
                }

            },
        };

        public static void StartEncoding(string encoder, string videoId, string uri)
        {
            Console.WriteLine("Encode started for video {0} with {1}", videoId, encoder);
            var status = GlobalStatus.ConversionStatus[videoId];

            if (!profiles.ContainsKey(encoder))
            {
                status.Error = "Invalid profile";
                status.Status = Models.ConversionStatusModel.StatusCodes.Error;
                return;

            }
            var profile = profiles[encoder];
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = profile.Exe;
            string arguments = string.Format(profile.Arguments, uri, @".\video\" + videoId);

            Console.WriteLine("Running: {0} {1}", startInfo.FileName, arguments);

            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                using (Process process = new Process() { StartInfo = startInfo })
                {
                    process.Start();
                    
                    process.OutputDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                            ReportProgress(status, profile, e.Data);

                    process.ErrorDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                            ReportProgress(status, profile, e.Data);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    
                    process.WaitForExit();
                    //exeProcess.Close();

                    if (process.ExitCode > 0)
                        throw new Exception("Encoder failed.");

                    QuickTimeContainerConverter.MakeQuickTime3Compatible(@".\video\" + videoId + ".mov");

                    Console.WriteLine("Encode Complete");
                    // TODO Check error status : exeProcess.ExitCode 
                    status.Status = Models.ConversionStatusModel.StatusCodes.ReadyForDownload;
                }
            }
            catch (Exception ex)
            {
                // Log error.
                status.Status = Models.ConversionStatusModel.StatusCodes.Error;
                status.Error = ex.Message;
            }
        }

        



        // Example duration line
        // Duration: 00:39:43.08, start: 0.040000, bitrate: 386 kb / s
        static Regex durationRegex = new Regex(@"\s*?Duration: ((\d\d)\:(\d\d)\:(\d\d)\.(\d+)).+");
        // Progress line:
        // time=00:19:25.16 bitrate= 823.0kbits/s frame=27963 fps= 7 q=0.0 size= 117085kB 
        static Regex progressRegex = new Regex(@"\s*?time=((\d\d)\:(\d\d)\:(\d\d)\.(\d+)).+");
        private static void ReportProgress(ConversionStatusModel status, EncodingProfile profile, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            // work out the percentage
            // FFMpeg 

            if (profile.Exe.Contains("ffmpeg"))
            {
                if (durationRegex.IsMatch(data))
                {
                    // Duration should be 00:39:43.08
                    // Hours:Minutes:Seconds:Frames?

                    var matches = durationRegex.Match(data).Groups;
                    var hours = Convert.ToInt32(matches[2].Value);
                    var minutes = Convert.ToInt32(matches[3].Value);
                    var seconds = Convert.ToInt32(matches[4].Value);
                    var timespan = new TimeSpan(hours, minutes, seconds);

                    status.Duration = (int)timespan.TotalSeconds;
                }
                if (progressRegex.IsMatch(data))
                {
                    var matches = progressRegex.Match(data).Groups;
                    var hours = Convert.ToInt32(matches[2].Value);
                    var minutes = Convert.ToInt32(matches[3].Value);
                    var seconds = Convert.ToInt32(matches[4].Value);
                    var timespan = new TimeSpan(hours, minutes, seconds);

                    var totalSeconds = timespan.TotalSeconds;
                    if (status.Duration == 0)
                        return;
                    var progress = (totalSeconds / (double)status.Duration) * 100;
                    status.Progress = (int)progress;
                }
            }
            else // Using QuickTime
            {
                float output;
                if (float.TryParse(data, out output))
                {
                    status.Progress = (int)output;
                }
            }
            Console.WriteLine("Progress: {0}%", status.Progress);
        }

       
    }
}

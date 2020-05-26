using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YouTubeProxy.Helpers;
using YouTubeProxy.Models;

namespace YouTubeProxy.EncodingEngine
{
    /// <summary>
    /// Contains the methods for handling the encoder...
    /// </summary>
    public class Encoder
    {
        private Profiles _profile;



        public Encoder(Profiles profiles)
        {
            this._profile = profiles;
        }
        

        public void Encode(EncodeDetail encodeDetails)
        {
            var encodeStore = EncodeStore.GetStore();
            var videoId = encodeDetails.VideoId;
            var encoder = encodeDetails.EncodeProfileName;



            if (!_profile.ContainsKey(encoder))
            {
                this.FailJob(encodeDetails, "Invalid Encoding Profile");
                return;
            }

            var profile = _profile[encoder];

            encodeDetails.Profile = profile;

            var outputFile = Path.Combine(Path.Combine(Settings.EncodeLocation), encodeDetails.DestinationFileName);


            Console.WriteLine("Encode started for video {0} with {1}", videoId, encoder);
            //var status = GlobalStatus.ConversionStatus[videoId];

            var status = encodeDetails.Progress;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = profile.Exe;
            
            string arguments = string.Format(profile.Arguments, encodeDetails.SourceUrl, outputFile);

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
                            UpdateProgress(encodeDetails, profile, e.Data);

                    process.ErrorDataReceived += (object sender, System.Diagnostics.DataReceivedEventArgs e) =>
                            UpdateProgress(encodeDetails, profile, e.Data);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                    //exeProcess.Close();

                    if (process.ExitCode > 0)
                        throw new Exception("Encoder failed.");

                    // Make this an option on the encodeProfile
                    if (profile.OutputExtension == "mov")
                        QuickTimeContainerConverter.MakeQuickTime3Compatible(outputFile + ".mov");

                    Console.WriteLine("Encode Complete");
                    // TODO Check error status : exeProcess.ExitCode 
                    this.CompleteJob(encodeDetails);
                }
            }
            catch (Exception ex)
            {
                // Log error.
                this.FailJob(encodeDetails, ex.Message);
            }
        }


        // Example duration line
        // Duration: 00:39:43.08, start: 0.040000, bitrate: 386 kb / s
        static Regex ffMpegDurationRegex = new Regex(@"\s*?Duration: ((\d\d)\:(\d\d)\:(\d\d)\.(\d+)).+");
        // Progress line:
        // time=00:19:25.16 bitrate= 823.0kbits/s frame=27963 fps= 7 q=0.0 size= 117085kB 
        static Regex ffMpegProgresRegex = new Regex(@"\s*?time=((\d\d)\:(\d\d)\:(\d\d)\.(\d+)).+");

        private static void UpdateProgress(EncodeDetail details, EncodingProfile profile, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            // work out the percentage
            // FFMpeg 
            details.Progress.Status = StatusCodes.Encoding;
            if (profile.Exe.Contains("ffmpeg"))
            {
             

                if (ffMpegDurationRegex.IsMatch(data))
                {
                    // Duration should be 00:39:43.08
                    // Hours:Minutes:Seconds:Frames?
                    var matches = ffMpegDurationRegex.Match(data).Groups;
                    var hours = Convert.ToInt32(matches[2].Value);
                    var minutes = Convert.ToInt32(matches[3].Value);
                    var seconds = Convert.ToInt32(matches[4].Value);
                    var timespan = new TimeSpan(hours, minutes, seconds);

                    details.Progress.Duration = (int)timespan.TotalSeconds;
                }
                if (ffMpegProgresRegex.IsMatch(data))
                {
                    var matches = ffMpegProgresRegex.Match(data).Groups;
                    var hours = Convert.ToInt32(matches[2].Value);
                    var minutes = Convert.ToInt32(matches[3].Value);
                    var seconds = Convert.ToInt32(matches[4].Value);
                    var timespan = new TimeSpan(hours, minutes, seconds);

                    var totalSeconds = timespan.TotalSeconds;
                    if (details.Progress.Duration == 0)
                        return;
                    var progress = (totalSeconds / (double)details.Progress.Duration) * 100;
                    details.Progress.Progress = (int)progress;
                }
            }
            else // Using QuickTime Converter
            {
                float output;
                if (float.TryParse(data, out output))
                {
                    details.Progress.Progress = (int)output;
                }
            }
            Console.WriteLine("Progress: {0}%", details.Progress.Progress);
            EncodeStore.GetStore().AddOrUpdateVideo(details);
        }

        private void FailJob(EncodeDetail encodeDetails, string error)
        {
            encodeDetails.Progress.Error = error;
            encodeDetails.Progress.Status = StatusCodes.Error;
            EncodeStore.GetStore().AddOrUpdateVideo(encodeDetails);
        }

        private void CompleteJob(EncodeDetail encodeDetails)
        {
            encodeDetails.Progress.Error = string.Empty;
            encodeDetails.Progress.Status = StatusCodes.ReadyForDownload;
        }

    }

   
}

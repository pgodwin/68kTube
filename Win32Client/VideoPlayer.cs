using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Client.Client;
using Win32Client.Models;

namespace Win32Client
{
    public partial class VideoPlayer : Form
    {
        private SearchResultModel item;
        private YouTubeProxyClient Client { get; set; }
        public ClientSettings Settings = ClientSettings.GetSettings();

        private Timer encodeCheckTimer;

        public VideoPlayer()
        {
            InitializeComponent();

        }

        public VideoPlayer(SearchResultModel item)
        {
            InitializeComponent();
            this.item = item;
            this.Client = new YouTubeProxyClient(ClientSettings.GetSettings().BaseUrl);

            // Start Encoding
            this.StartEncode();
        }

                

        private void VideoPlayer_Load(object sender, EventArgs e)
        {
            this.Text = item.Title;
            lblTitle.Text = item.Title;
            lblDescription.Text = item.Description;
        }

        private void StartEncode()
        {
            if (Settings.StreamMode == "HTTP")
            {
                encodeCheckTimer = new Timer();
                encodeCheckTimer.Interval = 1000;
                encodeCheckTimer.Tick += Timer_Tick;
                encodeCheckTimer.Start();
            }
            else
            {
                var result = Client.GetRtspStream(item.VideoId).FirstOrDefault();
                if (result != null && result.Success)
                {
                    axQTControl1.URL = result.RtspUrl;
                    axQTControl1.Show();
                    
                    axQTControl1.Movie.Play();
                }
                else
                {
                    MessageBox.Show("Error establishing RTSP stream");
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var encodeResult = Client.GetVideo(item.VideoId, Settings.Profile).First(); // should only be one


            switch (encodeResult.Status)
            {
                case ConversionStatusModel.StatusCodes.Encoding:
                    encodeProgress.Maximum = 100;
                    encodeProgress.Value = encodeResult.Progress;
                    break;
                case ConversionStatusModel.StatusCodes.Error:
                    encodeCheckTimer.Stop();
                    encodeProgress.Hide();
                    MessageBox.Show("Error Encoding Video. Error: " + encodeResult.Error);
                    break;
                case ConversionStatusModel.StatusCodes.ReadyForDownload:
                    encodeCheckTimer.Stop();
                    encodeProgress.Hide();
                    PlayVideo();
                    break;
            }
        }

        private void PlayVideo()
        {
            axQTControl1.URL = string.Concat(Settings.BaseUrl, "api/Download/mov/", this.item.VideoId);
            axQTControl1.Show();

        }
    }
}

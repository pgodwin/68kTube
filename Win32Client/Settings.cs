using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Win32Client
{
    public partial class Settings : Form
    {
        private string[] profiles = new string[]
        {
            "video1",
            "cinepak",
            "h263",
            "sorrensonFFMPEG",
            "sorrenson",
            "mpeg1",
            "mpeg4"
        };

        public Settings()
        {
            InitializeComponent();

            BuildCodecList();

            LoadSettings(ClientSettings.GetSettings());
        }

        public Settings(ClientSettings settings)
        {
            InitializeComponent();

            BuildCodecList();
            LoadSettings(settings);
        }

        private void BuildCodecList()
        {
            foreach (var codec in profiles)
            {
                encodeOptionsPanel.Controls.Add(new RadioButton()
                {
                    Text = codec,
                    
                });
            }
        }

        private ClientSettings GetSettings()
        {
            // Get the profile
            var profile = encodeOptionsPanel.Controls.Cast<RadioButton>().FirstOrDefault(c => c.Checked == true).Text;

            return new ClientSettings
            {
                BaseUrl = txtBaseUrl.Text,
                Profile = profile
            };
        }

        private void LoadSettings(ClientSettings settings)
        {
            txtBaseUrl.Text = settings.BaseUrl;
            encodeOptionsPanel.Controls.Cast<RadioButton>().FirstOrDefault(c => c.Text == settings.Profile).Checked = true;

        }

        private void encodeOptionsPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.GetSettings().SaveSettings();
        }
    }
}

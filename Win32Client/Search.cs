using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32Client.Client;
using Win32Client.Helpers;
using Win32Client.Models;


namespace Win32Client
{
    public partial class Search : Form
    {
        public Search()
        {
            InitializeComponent();

            listSearchResults.Columns.Add(new ColumnHeader());
            listSearchResults.LargeImageList = Thumbs;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var client = new YouTubeProxyClient(ClientSettings.GetSettings().BaseUrl);
            var results = client.Search(txtSearch.Text);

            ResultsToListView(results);
        }

        private void ResultsToListView(List<SearchResultModel> results)
        {
            listSearchResults.Items.Clear();
  

            foreach (var result in results)
            {
                var resultItem = new ListViewItem();

                
                resultItem.Text = result.Title;
                resultItem.Tag = result;

                resultItem.SubItems.Add(result.Description);
                
                listSearchResults.Items.Add(resultItem);
            }

            DownloadThumbs();
        }

        private async void DownloadThumbs()
        {
            Thumbs.Images.Clear();

            var client = new WebClient();
            for (int i = 0; i < listSearchResults.Items.Count; i++)
            {
                var item = listSearchResults.Items[i];
                var result = (SearchResultModel)item.Tag;
                var data = await client.DownloadDataTaskAsync(new Uri(result.ThumbUrl));
    
                    
                this.UIThread(() =>
                {
                    var item2 = listSearchResults.Items[i];

                    // Hack Alert!
                    Thumbs.Images.Add(new Bitmap(new MemoryStream(data)));
                    item2.ImageIndex = i;
                });
                
            }

        }

        private void listSearchResults_ItemActivate(object sender, EventArgs e)
        {
            
            var item = (SearchResultModel)listSearchResults.SelectedItems[0].Tag;

            var videoPlayer = new VideoPlayer(item);
            videoPlayer.Show();
        }


        private void Search_Load(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new Settings().ShowDialog();
        }
    }
}

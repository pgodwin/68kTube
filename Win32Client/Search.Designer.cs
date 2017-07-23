namespace Win32Client
{
    partial class Search
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.listSearchResults = new System.Windows.Forms.ListView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.Thumbs = new System.Windows.Forms.ImageList(this.components);
            this.mnuSettings = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(13, 27);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(386, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // listSearchResults
            // 
            this.listSearchResults.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.listSearchResults.Location = new System.Drawing.Point(13, 62);
            this.listSearchResults.MultiSelect = false;
            this.listSearchResults.Name = "listSearchResults";
            this.listSearchResults.Size = new System.Drawing.Size(467, 230);
            this.listSearchResults.TabIndex = 1;
            this.listSearchResults.UseCompatibleStateImageBehavior = false;
            this.listSearchResults.View = System.Windows.Forms.View.Tile;
            this.listSearchResults.ItemActivate += new System.EventHandler(this.listSearchResults_ItemActivate);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(405, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // Thumbs
            // 
            this.Thumbs.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.Thumbs.ImageSize = new System.Drawing.Size(32, 32);
            this.Thumbs.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mnuSettings
            // 
            this.mnuSettings.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mnuSettings.Location = new System.Drawing.Point(0, 0);
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.Size = new System.Drawing.Size(492, 24);
            this.mnuSettings.TabIndex = 3;
            this.mnuSettings.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // settingsToolStripMenuItem1
            // 
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.settingsToolStripMenuItem1.Text = "Settings...";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsToolStripMenuItem1_Click);
            // 
            // Search
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 304);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.listSearchResults);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.mnuSettings);
            this.MainMenuStrip = this.mnuSettings;
            this.Name = "Search";
            this.Text = "Search Youtube";
            this.Load += new System.EventHandler(this.Search_Load);
            this.mnuSettings.ResumeLayout(false);
            this.mnuSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ListView listSearchResults;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ImageList Thumbs;
        private System.Windows.Forms.MenuStrip mnuSettings;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
    }
}


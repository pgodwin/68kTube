namespace Win32Client
{
    partial class Settings
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbHttp = new System.Windows.Forms.RadioButton();
            this.rbRTSP = new System.Windows.Forms.RadioButton();
            this.txtBaseUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.encodeOptionsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbHttp);
            this.groupBox1.Controls.Add(this.rbRTSP);
            this.groupBox1.Location = new System.Drawing.Point(25, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 89);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Streaming Method";
            // 
            // rbHttp
            // 
            this.rbHttp.AutoSize = true;
            this.rbHttp.Checked = true;
            this.rbHttp.Location = new System.Drawing.Point(18, 55);
            this.rbHttp.Name = "rbHttp";
            this.rbHttp.Size = new System.Drawing.Size(163, 17);
            this.rbHttp.TabIndex = 1;
            this.rbHttp.TabStop = true;
            this.rbHttp.Text = "HTTP Progressive Download";
            this.rbHttp.UseVisualStyleBackColor = true;
            // 
            // rbRTSP
            // 
            this.rbRTSP.AutoSize = true;
            this.rbRTSP.Location = new System.Drawing.Point(18, 31);
            this.rbRTSP.Name = "rbRTSP";
            this.rbRTSP.Size = new System.Drawing.Size(54, 17);
            this.rbRTSP.TabIndex = 0;
            this.rbRTSP.Text = "RTSP";
            this.rbRTSP.UseVisualStyleBackColor = true;
            // 
            // txtBaseUrl
            // 
            this.txtBaseUrl.Location = new System.Drawing.Point(103, 13);
            this.txtBaseUrl.Name = "txtBaseUrl";
            this.txtBaseUrl.Size = new System.Drawing.Size(276, 20);
            this.txtBaseUrl.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Base Address:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.encodeOptionsPanel);
            this.groupBox2.Location = new System.Drawing.Point(25, 144);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(371, 133);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Profile";
            // 
            // encodeOptionsPanel
            // 
            this.encodeOptionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encodeOptionsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.encodeOptionsPanel.Location = new System.Drawing.Point(3, 16);
            this.encodeOptionsPanel.Name = "encodeOptionsPanel";
            this.encodeOptionsPanel.Size = new System.Drawing.Size(365, 114);
            this.encodeOptionsPanel.TabIndex = 0;
            this.encodeOptionsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.encodeOptionsPanel_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 284);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Defaults";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(229, 284);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(311, 284);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button3;
            this.ClientSize = new System.Drawing.Size(424, 319);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBaseUrl);
            this.Controls.Add(this.groupBox1);
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBaseUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbHttp;
        private System.Windows.Forms.RadioButton rbRTSP;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel encodeOptionsPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}
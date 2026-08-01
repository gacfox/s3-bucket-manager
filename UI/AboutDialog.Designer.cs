namespace Gacfox.S3BucketManager.UI
{
    partial class AboutDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
            aboutTitleLabel = new Label();
            aboutLinkLabel = new LinkLabel();
            aboutLinkTitleLabel = new Label();
            logoPictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
            SuspendLayout();
            // 
            // aboutTitleLabel
            // 
            aboutTitleLabel.AutoSize = true;
            aboutTitleLabel.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold, GraphicsUnit.Point, 134);
            aboutTitleLabel.Location = new Point(114, 12);
            aboutTitleLabel.Name = "aboutTitleLabel";
            aboutTitleLabel.Size = new Size(144, 19);
            aboutTitleLabel.TabIndex = 0;
            aboutTitleLabel.Text = "S3 Bucket Manager";
            // 
            // aboutLinkLabel
            // 
            aboutLinkLabel.AutoSize = true;
            aboutLinkLabel.Location = new Point(178, 45);
            aboutLinkLabel.Name = "aboutLinkLabel";
            aboutLinkLabel.Size = new Size(278, 17);
            aboutLinkLabel.TabIndex = 1;
            aboutLinkLabel.TabStop = true;
            aboutLinkLabel.Text = "https://github.com/gacfox/s3-bucket-manager";
            aboutLinkLabel.LinkClicked += aboutLinkLabel_LinkClicked;
            // 
            // aboutLinkTitleLabel
            // 
            aboutLinkTitleLabel.AutoSize = true;
            aboutLinkTitleLabel.Location = new Point(114, 45);
            aboutLinkTitleLabel.Name = "aboutLinkTitleLabel";
            aboutLinkTitleLabel.Size = new Size(68, 17);
            aboutLinkTitleLabel.TabIndex = 2;
            aboutLinkTitleLabel.Text = "项目地址：";
            // 
            // logoPictureBox
            // 
            logoPictureBox.Image = Properties.Resources.logo;
            logoPictureBox.Location = new Point(12, 12);
            logoPictureBox.Name = "logoPictureBox";
            logoPictureBox.Size = new Size(96, 96);
            logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            logoPictureBox.TabIndex = 3;
            logoPictureBox.TabStop = false;
            // 
            // AboutDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(481, 122);
            Controls.Add(logoPictureBox);
            Controls.Add(aboutLinkTitleLabel);
            Controls.Add(aboutLinkLabel);
            Controls.Add(aboutTitleLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "关于";
            ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label aboutTitleLabel;
        private LinkLabel aboutLinkLabel;
        private Label aboutLinkTitleLabel;
        private PictureBox logoPictureBox;
    }
}
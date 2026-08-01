namespace Gacfox.S3BucketManager.UI
{
    partial class PropertiesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesDialog));
            configTableLayoutPanel = new TableLayoutPanel();
            uploadMaxConcurrencyLabel = new Label();
            downloadMaxConcurrencyLabel = new Label();
            linkExpireLabel = new Label();
            uploadMaxConcurrencyNumericUpDown = new NumericUpDown();
            downloadMaxConcurrencyNumericUpDown = new NumericUpDown();
            linkExpireNumericUpDown = new NumericUpDown();
            buttonGroupFlowLayoutPanel = new FlowLayoutPanel();
            cancelButton = new Button();
            confirmButton = new Button();
            configTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uploadMaxConcurrencyNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)downloadMaxConcurrencyNumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linkExpireNumericUpDown).BeginInit();
            buttonGroupFlowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // configTableLayoutPanel
            // 
            configTableLayoutPanel.ColumnCount = 2;
            configTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            configTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            configTableLayoutPanel.Controls.Add(uploadMaxConcurrencyLabel, 0, 0);
            configTableLayoutPanel.Controls.Add(downloadMaxConcurrencyLabel, 0, 1);
            configTableLayoutPanel.Controls.Add(linkExpireLabel, 0, 2);
            configTableLayoutPanel.Controls.Add(uploadMaxConcurrencyNumericUpDown, 1, 0);
            configTableLayoutPanel.Controls.Add(downloadMaxConcurrencyNumericUpDown, 1, 1);
            configTableLayoutPanel.Controls.Add(linkExpireNumericUpDown, 1, 2);
            configTableLayoutPanel.Dock = DockStyle.Fill;
            configTableLayoutPanel.Location = new Point(0, 0);
            configTableLayoutPanel.Name = "configTableLayoutPanel";
            configTableLayoutPanel.RowCount = 4;
            configTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            configTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            configTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            configTableLayoutPanel.RowStyles.Add(new RowStyle());
            configTableLayoutPanel.Size = new Size(316, 121);
            configTableLayoutPanel.TabIndex = 0;
            // 
            // uploadMaxConcurrencyLabel
            // 
            uploadMaxConcurrencyLabel.AutoSize = true;
            uploadMaxConcurrencyLabel.Dock = DockStyle.Fill;
            uploadMaxConcurrencyLabel.Location = new Point(3, 0);
            uploadMaxConcurrencyLabel.Name = "uploadMaxConcurrencyLabel";
            uploadMaxConcurrencyLabel.Size = new Size(152, 30);
            uploadMaxConcurrencyLabel.TabIndex = 0;
            uploadMaxConcurrencyLabel.Text = "上传最大并发数";
            uploadMaxConcurrencyLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // downloadMaxConcurrencyLabel
            // 
            downloadMaxConcurrencyLabel.AutoSize = true;
            downloadMaxConcurrencyLabel.Dock = DockStyle.Fill;
            downloadMaxConcurrencyLabel.Location = new Point(3, 30);
            downloadMaxConcurrencyLabel.Name = "downloadMaxConcurrencyLabel";
            downloadMaxConcurrencyLabel.Size = new Size(152, 30);
            downloadMaxConcurrencyLabel.TabIndex = 1;
            downloadMaxConcurrencyLabel.Text = "下载最大并发数";
            downloadMaxConcurrencyLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // linkExpireLabel
            // 
            linkExpireLabel.AutoSize = true;
            linkExpireLabel.Dock = DockStyle.Fill;
            linkExpireLabel.Location = new Point(3, 60);
            linkExpireLabel.Name = "linkExpireLabel";
            linkExpireLabel.Size = new Size(152, 30);
            linkExpireLabel.TabIndex = 4;
            linkExpireLabel.Text = "生成链接有效期（秒）";
            linkExpireLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // uploadMaxConcurrencyNumericUpDown
            // 
            uploadMaxConcurrencyNumericUpDown.Dock = DockStyle.Fill;
            uploadMaxConcurrencyNumericUpDown.Location = new Point(161, 3);
            uploadMaxConcurrencyNumericUpDown.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            uploadMaxConcurrencyNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            uploadMaxConcurrencyNumericUpDown.Name = "uploadMaxConcurrencyNumericUpDown";
            uploadMaxConcurrencyNumericUpDown.Size = new Size(152, 23);
            uploadMaxConcurrencyNumericUpDown.TabIndex = 2;
            uploadMaxConcurrencyNumericUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // downloadMaxConcurrencyNumericUpDown
            // 
            downloadMaxConcurrencyNumericUpDown.Dock = DockStyle.Fill;
            downloadMaxConcurrencyNumericUpDown.Location = new Point(161, 33);
            downloadMaxConcurrencyNumericUpDown.Maximum = new decimal(new int[] { 16, 0, 0, 0 });
            downloadMaxConcurrencyNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            downloadMaxConcurrencyNumericUpDown.Name = "downloadMaxConcurrencyNumericUpDown";
            downloadMaxConcurrencyNumericUpDown.Size = new Size(152, 23);
            downloadMaxConcurrencyNumericUpDown.TabIndex = 3;
            downloadMaxConcurrencyNumericUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // linkExpireNumericUpDown
            // 
            linkExpireNumericUpDown.Dock = DockStyle.Fill;
            linkExpireNumericUpDown.Location = new Point(161, 63);
            linkExpireNumericUpDown.Maximum = new decimal(new int[] { 604800, 0, 0, 0 });
            linkExpireNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            linkExpireNumericUpDown.Name = "linkExpireNumericUpDown";
            linkExpireNumericUpDown.Size = new Size(152, 23);
            linkExpireNumericUpDown.TabIndex = 5;
            linkExpireNumericUpDown.Value = new decimal(new int[] { 3600, 0, 0, 0 });
            // 
            // buttonGroupFlowLayoutPanel
            // 
            buttonGroupFlowLayoutPanel.Controls.Add(cancelButton);
            buttonGroupFlowLayoutPanel.Controls.Add(confirmButton);
            buttonGroupFlowLayoutPanel.Dock = DockStyle.Bottom;
            buttonGroupFlowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonGroupFlowLayoutPanel.Location = new Point(0, 91);
            buttonGroupFlowLayoutPanel.Name = "buttonGroupFlowLayoutPanel";
            buttonGroupFlowLayoutPanel.Size = new Size(316, 30);
            buttonGroupFlowLayoutPanel.TabIndex = 1;
            // 
            // cancelButton
            // 
            cancelButton.Image = Properties.Resources.cancel;
            cancelButton.ImageAlign = ContentAlignment.MiddleLeft;
            cancelButton.Location = new Point(253, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(60, 23);
            cancelButton.TabIndex = 0;
            cancelButton.Text = "取消";
            cancelButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // confirmButton
            // 
            confirmButton.Image = Properties.Resources.ok;
            confirmButton.ImageAlign = ContentAlignment.MiddleLeft;
            confirmButton.Location = new Point(187, 3);
            confirmButton.Name = "confirmButton";
            confirmButton.Size = new Size(60, 23);
            confirmButton.TabIndex = 1;
            confirmButton.Text = "确认";
            confirmButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            confirmButton.UseVisualStyleBackColor = true;
            confirmButton.Click += confirmButton_Click;
            // 
            // PropertiesDialog
            // 
            AcceptButton = confirmButton;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(316, 121);
            Controls.Add(buttonGroupFlowLayoutPanel);
            Controls.Add(configTableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PropertiesDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "选项";
            configTableLayoutPanel.ResumeLayout(false);
            configTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)uploadMaxConcurrencyNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)downloadMaxConcurrencyNumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)linkExpireNumericUpDown).EndInit();
            buttonGroupFlowLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel configTableLayoutPanel;
        private Label uploadMaxConcurrencyLabel;
        private Label downloadMaxConcurrencyLabel;
        private NumericUpDown uploadMaxConcurrencyNumericUpDown;
        private NumericUpDown downloadMaxConcurrencyNumericUpDown;
        private Label linkExpireLabel;
        private FlowLayoutPanel buttonGroupFlowLayoutPanel;
        private Button cancelButton;
        private Button confirmButton;
        private NumericUpDown linkExpireNumericUpDown;
    }
}
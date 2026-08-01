namespace Gacfox.S3BucketManager.UI
{
    partial class RenameDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenameDialog));
            buttonGroupFlowLayoutPanel = new FlowLayoutPanel();
            cancelButton = new Button();
            confirmButton = new Button();
            inputPanel = new Panel();
            inputTextBox = new TextBox();
            buttonGroupFlowLayoutPanel.SuspendLayout();
            inputPanel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonGroupFlowLayoutPanel
            // 
            buttonGroupFlowLayoutPanel.Controls.Add(cancelButton);
            buttonGroupFlowLayoutPanel.Controls.Add(confirmButton);
            buttonGroupFlowLayoutPanel.Dock = DockStyle.Bottom;
            buttonGroupFlowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonGroupFlowLayoutPanel.Location = new Point(0, 23);
            buttonGroupFlowLayoutPanel.Name = "buttonGroupFlowLayoutPanel";
            buttonGroupFlowLayoutPanel.Size = new Size(243, 29);
            buttonGroupFlowLayoutPanel.TabIndex = 0;
            // 
            // cancelButton
            // 
            cancelButton.Image = Properties.Resources.cancel;
            cancelButton.ImageAlign = ContentAlignment.MiddleLeft;
            cancelButton.Location = new Point(180, 3);
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
            confirmButton.Location = new Point(114, 3);
            confirmButton.Name = "confirmButton";
            confirmButton.Size = new Size(60, 23);
            confirmButton.TabIndex = 1;
            confirmButton.Text = "确认";
            confirmButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            confirmButton.UseVisualStyleBackColor = true;
            confirmButton.Click += confirmButton_Click;
            // 
            // inputPanel
            // 
            inputPanel.Controls.Add(inputTextBox);
            inputPanel.Dock = DockStyle.Fill;
            inputPanel.Location = new Point(0, 0);
            inputPanel.Name = "inputPanel";
            inputPanel.Size = new Size(243, 23);
            inputPanel.TabIndex = 1;
            // 
            // inputTextBox
            // 
            inputTextBox.Dock = DockStyle.Fill;
            inputTextBox.Location = new Point(0, 0);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(243, 23);
            inputTextBox.TabIndex = 0;
            // 
            // RenameDialog
            // 
            AcceptButton = confirmButton;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(243, 52);
            Controls.Add(inputPanel);
            Controls.Add(buttonGroupFlowLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RenameDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "重命名";
            buttonGroupFlowLayoutPanel.ResumeLayout(false);
            inputPanel.ResumeLayout(false);
            inputPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private FlowLayoutPanel buttonGroupFlowLayoutPanel;
        private Button cancelButton;
        private Button confirmButton;
        private Panel inputPanel;
        private TextBox inputTextBox;
    }
}
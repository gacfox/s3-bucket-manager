namespace Gacfox.S3BucketManager.UI
{
    partial class EditConnectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditConnectionDialog));
            tableLayoutPanel = new TableLayoutPanel();
            nameLabel = new Label();
            nameTextBox = new TextBox();
            useSecureSslCheckBox = new CheckBox();
            skTextBox = new TextBox();
            akTextBox = new TextBox();
            useSecureSslLabel = new Label();
            skLabel = new Label();
            akLabel = new Label();
            apiEndpointLabel = new Label();
            apiEndpointTextBox = new TextBox();
            buttonGroupFlowLayoutPanel = new FlowLayoutPanel();
            cancelButton = new Button();
            confirmButton = new Button();
            testConnectionButton = new Button();
            tableLayoutPanel.SuspendLayout();
            buttonGroupFlowLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 2;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.49425F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68.50575F));
            tableLayoutPanel.Controls.Add(nameLabel, 0, 0);
            tableLayoutPanel.Controls.Add(nameTextBox, 1, 0);
            tableLayoutPanel.Controls.Add(useSecureSslCheckBox, 1, 4);
            tableLayoutPanel.Controls.Add(skTextBox, 1, 3);
            tableLayoutPanel.Controls.Add(akTextBox, 1, 2);
            tableLayoutPanel.Controls.Add(useSecureSslLabel, 0, 4);
            tableLayoutPanel.Controls.Add(skLabel, 0, 3);
            tableLayoutPanel.Controls.Add(akLabel, 0, 2);
            tableLayoutPanel.Controls.Add(apiEndpointLabel, 0, 1);
            tableLayoutPanel.Controls.Add(apiEndpointTextBox, 1, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 6;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new Size(419, 157);
            tableLayoutPanel.TabIndex = 0;
            // 
            // nameLabel
            // 
            nameLabel.AutoSize = true;
            nameLabel.Dock = DockStyle.Fill;
            nameLabel.Location = new Point(3, 0);
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new Size(125, 30);
            nameLabel.TabIndex = 0;
            nameLabel.Text = "名称";
            nameLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // nameTextBox
            // 
            nameTextBox.Dock = DockStyle.Fill;
            nameTextBox.Location = new Point(134, 3);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(282, 23);
            nameTextBox.TabIndex = 1;
            // 
            // useSecureSslCheckBox
            // 
            useSecureSslCheckBox.Anchor = AnchorStyles.Left;
            useSecureSslCheckBox.AutoSize = true;
            useSecureSslCheckBox.Location = new Point(134, 128);
            useSecureSslCheckBox.Name = "useSecureSslCheckBox";
            useSecureSslCheckBox.Size = new Size(15, 14);
            useSecureSslCheckBox.TabIndex = 7;
            useSecureSslCheckBox.UseVisualStyleBackColor = true;
            // 
            // skTextBox
            // 
            skTextBox.Dock = DockStyle.Fill;
            skTextBox.Location = new Point(134, 93);
            skTextBox.Name = "skTextBox";
            skTextBox.PasswordChar = '*';
            skTextBox.Size = new Size(282, 23);
            skTextBox.TabIndex = 6;
            // 
            // akTextBox
            // 
            akTextBox.Dock = DockStyle.Fill;
            akTextBox.Location = new Point(134, 63);
            akTextBox.Name = "akTextBox";
            akTextBox.Size = new Size(282, 23);
            akTextBox.TabIndex = 5;
            // 
            // useSecureSslLabel
            // 
            useSecureSslLabel.AutoSize = true;
            useSecureSslLabel.Dock = DockStyle.Fill;
            useSecureSslLabel.Location = new Point(3, 120);
            useSecureSslLabel.Name = "useSecureSslLabel";
            useSecureSslLabel.Size = new Size(125, 30);
            useSecureSslLabel.TabIndex = 4;
            useSecureSslLabel.Text = "安全传输（SSL/TLS）";
            useSecureSslLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // skLabel
            // 
            skLabel.AutoSize = true;
            skLabel.Dock = DockStyle.Fill;
            skLabel.Location = new Point(3, 90);
            skLabel.Name = "skLabel";
            skLabel.Size = new Size(125, 30);
            skLabel.TabIndex = 3;
            skLabel.Text = "Secret Key";
            skLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // akLabel
            // 
            akLabel.AutoSize = true;
            akLabel.Dock = DockStyle.Fill;
            akLabel.Location = new Point(3, 60);
            akLabel.Name = "akLabel";
            akLabel.Size = new Size(125, 30);
            akLabel.TabIndex = 2;
            akLabel.Text = "Access Key ID";
            akLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // apiEndpointLabel
            // 
            apiEndpointLabel.AutoSize = true;
            apiEndpointLabel.Dock = DockStyle.Fill;
            apiEndpointLabel.Location = new Point(3, 30);
            apiEndpointLabel.Name = "apiEndpointLabel";
            apiEndpointLabel.Size = new Size(125, 30);
            apiEndpointLabel.TabIndex = 8;
            apiEndpointLabel.Text = "API端点";
            apiEndpointLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // apiEndpointTextBox
            // 
            apiEndpointTextBox.Dock = DockStyle.Fill;
            apiEndpointTextBox.Location = new Point(134, 33);
            apiEndpointTextBox.Name = "apiEndpointTextBox";
            apiEndpointTextBox.Size = new Size(282, 23);
            apiEndpointTextBox.TabIndex = 9;
            // 
            // buttonGroupFlowLayoutPanel
            // 
            buttonGroupFlowLayoutPanel.Controls.Add(cancelButton);
            buttonGroupFlowLayoutPanel.Controls.Add(confirmButton);
            buttonGroupFlowLayoutPanel.Controls.Add(testConnectionButton);
            buttonGroupFlowLayoutPanel.Dock = DockStyle.Bottom;
            buttonGroupFlowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonGroupFlowLayoutPanel.Location = new Point(0, 157);
            buttonGroupFlowLayoutPanel.Name = "buttonGroupFlowLayoutPanel";
            buttonGroupFlowLayoutPanel.Size = new Size(419, 32);
            buttonGroupFlowLayoutPanel.TabIndex = 8;
            // 
            // cancelButton
            // 
            cancelButton.Image = Properties.Resources.cancel;
            cancelButton.ImageAlign = ContentAlignment.MiddleLeft;
            cancelButton.Location = new Point(356, 3);
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
            confirmButton.Location = new Point(290, 3);
            confirmButton.Name = "confirmButton";
            confirmButton.Size = new Size(60, 23);
            confirmButton.TabIndex = 1;
            confirmButton.Text = "确认";
            confirmButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            confirmButton.UseVisualStyleBackColor = true;
            confirmButton.Click += confirmButton_Click;
            // 
            // testConnectionButton
            // 
            testConnectionButton.Image = Properties.Resources.connect;
            testConnectionButton.ImageAlign = ContentAlignment.MiddleLeft;
            testConnectionButton.Location = new Point(204, 3);
            testConnectionButton.Name = "testConnectionButton";
            testConnectionButton.Size = new Size(80, 23);
            testConnectionButton.TabIndex = 2;
            testConnectionButton.Text = "测试连接";
            testConnectionButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            testConnectionButton.UseVisualStyleBackColor = true;
            testConnectionButton.Click += testConnectionButton_Click;
            // 
            // EditConnectionDialog
            // 
            AcceptButton = confirmButton;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(419, 189);
            Controls.Add(tableLayoutPanel);
            Controls.Add(buttonGroupFlowLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditConnectionDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "编辑连接";
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            buttonGroupFlowLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel;
        private Label nameLabel;
        private TextBox nameTextBox;
        private Label akLabel;
        private Label skLabel;
        private Label useSecureSslLabel;
        private TextBox akTextBox;
        private TextBox skTextBox;
        private CheckBox useSecureSslCheckBox;
        private FlowLayoutPanel buttonGroupFlowLayoutPanel;
        private Button cancelButton;
        private Button confirmButton;
        private Button testConnectionButton;
        private Label apiEndpointLabel;
        private TextBox apiEndpointTextBox;
    }
}
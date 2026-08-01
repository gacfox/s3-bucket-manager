namespace Gacfox.S3BucketManager.UI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainMenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newConnectionToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            uploadToolStripMenuItem = new ToolStripMenuItem();
            downloadToolStripMenuItem = new ToolStripMenuItem();
            editToolStripSeparator1 = new ToolStripSeparator();
            selectAllToolStripMenuItem = new ToolStripMenuItem();
            copyToolStripMenuItem = new ToolStripMenuItem();
            cutToolStripMenuItem = new ToolStripMenuItem();
            pasteToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            renameToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            useLargeIconToolStripMenuItem = new ToolStripMenuItem();
            useSmallIconToolStripMenuItem = new ToolStripMenuItem();
            useListToolStripMenuItem = new ToolStripMenuItem();
            useDetailToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripSeparator1 = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            configToolStripMenuItem = new ToolStripMenuItem();
            propertiesToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            mainToolStrip = new ToolStrip();
            newConnectionToolStripButton = new ToolStripButton();
            uploadToolStripButton = new ToolStripButton();
            downloadToolStripButton = new ToolStripButton();
            selectAlltoolStripButton = new ToolStripButton();
            copyToolStripButton = new ToolStripButton();
            cutToolStripButton = new ToolStripButton();
            pasteToolStripButton = new ToolStripButton();
            deleteToolStripButton = new ToolStripButton();
            renameToolStripButton = new ToolStripButton();
            refreshToolStripButton = new ToolStripButton();
            mainStatusStrip = new StatusStrip();
            mainStripStatusLabel = new ToolStripStatusLabel();
            mainSplitContainer = new SplitContainer();
            dataSplitContainer = new SplitContainer();
            bucketTreeView = new TreeView();
            fileStatusStrip = new StatusStrip();
            fileStatusToolStripStatusLabel = new ToolStripStatusLabel();
            fileListView = new ListView();
            fileInfoPanel = new Panel();
            locationTextBox = new TextBox();
            searchTextBox = new TextBox();
            taskTabControl = new TabControl();
            uploadTabPage = new TabPage();
            uploadTabPageListView = new ListView();
            downloadTabPage = new TabPage();
            downloadTabPageListView = new ListView();
            completeTabPage = new TabPage();
            completeTabPageListView = new ListView();
            mainMenuStrip.SuspendLayout();
            mainToolStrip.SuspendLayout();
            mainStatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).BeginInit();
            mainSplitContainer.Panel1.SuspendLayout();
            mainSplitContainer.Panel2.SuspendLayout();
            mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataSplitContainer).BeginInit();
            dataSplitContainer.Panel1.SuspendLayout();
            dataSplitContainer.Panel2.SuspendLayout();
            dataSplitContainer.SuspendLayout();
            fileStatusStrip.SuspendLayout();
            fileInfoPanel.SuspendLayout();
            taskTabControl.SuspendLayout();
            uploadTabPage.SuspendLayout();
            downloadTabPage.SuspendLayout();
            completeTabPage.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, viewToolStripMenuItem, configToolStripMenuItem, helpToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(800, 25);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newConnectionToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(58, 21);
            fileToolStripMenuItem.Text = "文件(&F)";
            // 
            // newConnectionToolStripMenuItem
            // 
            newConnectionToolStripMenuItem.Name = "newConnectionToolStripMenuItem";
            newConnectionToolStripMenuItem.Size = new Size(124, 22);
            newConnectionToolStripMenuItem.Text = "新建连接";
            newConnectionToolStripMenuItem.Click += newConnectionToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(124, 22);
            exitToolStripMenuItem.Text = "退出";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { uploadToolStripMenuItem, downloadToolStripMenuItem, editToolStripSeparator1, selectAllToolStripMenuItem, copyToolStripMenuItem, cutToolStripMenuItem, pasteToolStripMenuItem, deleteToolStripMenuItem, renameToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(59, 21);
            editToolStripMenuItem.Text = "编辑(&E)";
            // 
            // uploadToolStripMenuItem
            // 
            uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            uploadToolStripMenuItem.Size = new Size(112, 22);
            uploadToolStripMenuItem.Text = "上传";
            uploadToolStripMenuItem.Click += uploadToolStripMenuItem_Click;
            // 
            // downloadToolStripMenuItem
            // 
            downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            downloadToolStripMenuItem.Size = new Size(112, 22);
            downloadToolStripMenuItem.Text = "下载";
            downloadToolStripMenuItem.Click += downloadToolStripMenuItem_Click;
            // 
            // editToolStripSeparator1
            // 
            editToolStripSeparator1.Name = "editToolStripSeparator1";
            editToolStripSeparator1.Size = new Size(109, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            selectAllToolStripMenuItem.Size = new Size(112, 22);
            selectAllToolStripMenuItem.Text = "全选";
            selectAllToolStripMenuItem.Click += selectAllToolStripMenuItem_Click;
            // 
            // copyToolStripMenuItem
            // 
            copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            copyToolStripMenuItem.Size = new Size(112, 22);
            copyToolStripMenuItem.Text = "复制";
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            // 
            // cutToolStripMenuItem
            // 
            cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            cutToolStripMenuItem.Size = new Size(112, 22);
            cutToolStripMenuItem.Text = "剪切";
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            // 
            // pasteToolStripMenuItem
            // 
            pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            pasteToolStripMenuItem.Size = new Size(112, 22);
            pasteToolStripMenuItem.Text = "粘贴";
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(112, 22);
            deleteToolStripMenuItem.Text = "删除";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // renameToolStripMenuItem
            // 
            renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            renameToolStripMenuItem.Size = new Size(112, 22);
            renameToolStripMenuItem.Text = "重命名";
            renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { useLargeIconToolStripMenuItem, useSmallIconToolStripMenuItem, useListToolStripMenuItem, useDetailToolStripMenuItem, viewToolStripSeparator1, refreshToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(60, 21);
            viewToolStripMenuItem.Text = "查看(&V)";
            // 
            // useLargeIconToolStripMenuItem
            // 
            useLargeIconToolStripMenuItem.CheckOnClick = true;
            useLargeIconToolStripMenuItem.Name = "useLargeIconToolStripMenuItem";
            useLargeIconToolStripMenuItem.Size = new Size(124, 22);
            useLargeIconToolStripMenuItem.Text = "大图标";
            useLargeIconToolStripMenuItem.Click += useLargeIconToolStripMenuItem_Click;
            // 
            // useSmallIconToolStripMenuItem
            // 
            useSmallIconToolStripMenuItem.CheckOnClick = true;
            useSmallIconToolStripMenuItem.Name = "useSmallIconToolStripMenuItem";
            useSmallIconToolStripMenuItem.Size = new Size(124, 22);
            useSmallIconToolStripMenuItem.Text = "小图标";
            useSmallIconToolStripMenuItem.Click += useSmallIconToolStripMenuItem_Click;
            // 
            // useListToolStripMenuItem
            // 
            useListToolStripMenuItem.CheckOnClick = true;
            useListToolStripMenuItem.Name = "useListToolStripMenuItem";
            useListToolStripMenuItem.Size = new Size(124, 22);
            useListToolStripMenuItem.Text = "列表";
            useListToolStripMenuItem.Click += useListToolStripMenuItem_Click;
            // 
            // useDetailToolStripMenuItem
            // 
            useDetailToolStripMenuItem.CheckOnClick = true;
            useDetailToolStripMenuItem.Name = "useDetailToolStripMenuItem";
            useDetailToolStripMenuItem.Size = new Size(124, 22);
            useDetailToolStripMenuItem.Text = "详细信息";
            useDetailToolStripMenuItem.Click += useDetailToolStripMenuItem_Click;
            // 
            // viewToolStripSeparator1
            // 
            viewToolStripSeparator1.Name = "viewToolStripSeparator1";
            viewToolStripSeparator1.Size = new Size(121, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(124, 22);
            refreshToolStripMenuItem.Text = "刷新";
            refreshToolStripMenuItem.Click += refreshToolStripMenuItem_Click;
            // 
            // configToolStripMenuItem
            // 
            configToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { propertiesToolStripMenuItem });
            configToolStripMenuItem.Name = "configToolStripMenuItem";
            configToolStripMenuItem.Size = new Size(59, 21);
            configToolStripMenuItem.Text = "设置(&T)";
            // 
            // propertiesToolStripMenuItem
            // 
            propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            propertiesToolStripMenuItem.Size = new Size(100, 22);
            propertiesToolStripMenuItem.Text = "选项";
            propertiesToolStripMenuItem.Click += propertiesToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(61, 21);
            helpToolStripMenuItem.Text = "帮助(&H)";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(100, 22);
            aboutToolStripMenuItem.Text = "关于";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // mainToolStrip
            // 
            mainToolStrip.Items.AddRange(new ToolStripItem[] { newConnectionToolStripButton, uploadToolStripButton, downloadToolStripButton, selectAlltoolStripButton, copyToolStripButton, cutToolStripButton, pasteToolStripButton, deleteToolStripButton, renameToolStripButton, refreshToolStripButton });
            mainToolStrip.Location = new Point(0, 25);
            mainToolStrip.Name = "mainToolStrip";
            mainToolStrip.Size = new Size(800, 25);
            mainToolStrip.TabIndex = 1;
            mainToolStrip.Text = "mainToolStrip";
            // 
            // newConnectionToolStripButton
            // 
            newConnectionToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            newConnectionToolStripButton.Image = Properties.Resources.add;
            newConnectionToolStripButton.ImageTransparentColor = Color.Magenta;
            newConnectionToolStripButton.Name = "newConnectionToolStripButton";
            newConnectionToolStripButton.Size = new Size(23, 22);
            newConnectionToolStripButton.Text = "新建连接";
            newConnectionToolStripButton.Click += newConnectionToolStripButton_Click;
            // 
            // uploadToolStripButton
            // 
            uploadToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            uploadToolStripButton.Image = Properties.Resources.arrow_up;
            uploadToolStripButton.ImageTransparentColor = Color.Magenta;
            uploadToolStripButton.Name = "uploadToolStripButton";
            uploadToolStripButton.Size = new Size(23, 22);
            uploadToolStripButton.Text = "上传";
            uploadToolStripButton.Click += uploadToolStripButton_Click;
            // 
            // downloadToolStripButton
            // 
            downloadToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            downloadToolStripButton.Image = Properties.Resources.arrow_down;
            downloadToolStripButton.ImageTransparentColor = Color.Magenta;
            downloadToolStripButton.Name = "downloadToolStripButton";
            downloadToolStripButton.Size = new Size(23, 22);
            downloadToolStripButton.Text = "下载";
            downloadToolStripButton.Click += downloadToolStripButton_Click;
            // 
            // selectAlltoolStripButton
            // 
            selectAlltoolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            selectAlltoolStripButton.Image = Properties.Resources.ok;
            selectAlltoolStripButton.ImageTransparentColor = Color.Magenta;
            selectAlltoolStripButton.Name = "selectAlltoolStripButton";
            selectAlltoolStripButton.Size = new Size(23, 22);
            selectAlltoolStripButton.Text = "全选";
            selectAlltoolStripButton.Click += selectAlltoolStripButton_Click;
            // 
            // copyToolStripButton
            // 
            copyToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            copyToolStripButton.Image = Properties.Resources.page_copy;
            copyToolStripButton.ImageTransparentColor = Color.Magenta;
            copyToolStripButton.Name = "copyToolStripButton";
            copyToolStripButton.Size = new Size(23, 22);
            copyToolStripButton.Text = "复制";
            copyToolStripButton.Click += copyToolStripButton_Click;
            // 
            // cutToolStripButton
            // 
            cutToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            cutToolStripButton.Image = Properties.Resources.cut;
            cutToolStripButton.ImageTransparentColor = Color.Magenta;
            cutToolStripButton.Name = "cutToolStripButton";
            cutToolStripButton.Size = new Size(23, 22);
            cutToolStripButton.Text = "剪切";
            cutToolStripButton.Click += cutToolStripButton_Click;
            // 
            // pasteToolStripButton
            // 
            pasteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pasteToolStripButton.Image = Properties.Resources.page_paste;
            pasteToolStripButton.ImageTransparentColor = Color.Magenta;
            pasteToolStripButton.Name = "pasteToolStripButton";
            pasteToolStripButton.Size = new Size(23, 22);
            pasteToolStripButton.Text = "粘贴";
            pasteToolStripButton.Click += pasteToolStripButton_Click;
            // 
            // deleteToolStripButton
            // 
            deleteToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            deleteToolStripButton.Image = Properties.Resources.page_delete;
            deleteToolStripButton.ImageTransparentColor = Color.Magenta;
            deleteToolStripButton.Name = "deleteToolStripButton";
            deleteToolStripButton.Size = new Size(23, 22);
            deleteToolStripButton.Text = "删除";
            deleteToolStripButton.Click += deleteToolStripButton_Click;
            // 
            // renameToolStripButton
            // 
            renameToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            renameToolStripButton.Image = Properties.Resources.page_edit;
            renameToolStripButton.ImageTransparentColor = Color.Magenta;
            renameToolStripButton.Name = "renameToolStripButton";
            renameToolStripButton.Size = new Size(23, 22);
            renameToolStripButton.Text = "重命名";
            renameToolStripButton.Click += renameToolStripButton_Click;
            // 
            // refreshToolStripButton
            // 
            refreshToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            refreshToolStripButton.Image = Properties.Resources.arrow_refresh;
            refreshToolStripButton.ImageTransparentColor = Color.Magenta;
            refreshToolStripButton.Name = "refreshToolStripButton";
            refreshToolStripButton.Size = new Size(23, 22);
            refreshToolStripButton.Text = "刷新";
            refreshToolStripButton.Click += refreshToolStripButton_Click;
            // 
            // mainStatusStrip
            // 
            mainStatusStrip.Items.AddRange(new ToolStripItem[] { mainStripStatusLabel });
            mainStatusStrip.Location = new Point(0, 428);
            mainStatusStrip.Name = "mainStatusStrip";
            mainStatusStrip.Size = new Size(800, 22);
            mainStatusStrip.TabIndex = 2;
            mainStatusStrip.Text = "statusStrip1";
            // 
            // mainStripStatusLabel
            // 
            mainStripStatusLabel.Name = "mainStripStatusLabel";
            mainStripStatusLabel.Size = new Size(32, 17);
            mainStripStatusLabel.Text = "就绪";
            // 
            // mainSplitContainer
            // 
            mainSplitContainer.Dock = DockStyle.Fill;
            mainSplitContainer.Location = new Point(0, 50);
            mainSplitContainer.Name = "mainSplitContainer";
            mainSplitContainer.Orientation = Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            mainSplitContainer.Panel1.Controls.Add(dataSplitContainer);
            // 
            // mainSplitContainer.Panel2
            // 
            mainSplitContainer.Panel2.Controls.Add(taskTabControl);
            mainSplitContainer.Size = new Size(800, 378);
            mainSplitContainer.SplitterDistance = 262;
            mainSplitContainer.TabIndex = 3;
            // 
            // dataSplitContainer
            // 
            dataSplitContainer.Dock = DockStyle.Fill;
            dataSplitContainer.Location = new Point(0, 0);
            dataSplitContainer.Name = "dataSplitContainer";
            // 
            // dataSplitContainer.Panel1
            // 
            dataSplitContainer.Panel1.Controls.Add(bucketTreeView);
            // 
            // dataSplitContainer.Panel2
            // 
            dataSplitContainer.Panel2.Controls.Add(fileStatusStrip);
            dataSplitContainer.Panel2.Controls.Add(fileListView);
            dataSplitContainer.Panel2.Controls.Add(fileInfoPanel);
            dataSplitContainer.Size = new Size(800, 262);
            dataSplitContainer.SplitterDistance = 266;
            dataSplitContainer.TabIndex = 0;
            // 
            // bucketTreeView
            // 
            bucketTreeView.Dock = DockStyle.Fill;
            bucketTreeView.Location = new Point(0, 0);
            bucketTreeView.Name = "bucketTreeView";
            bucketTreeView.Size = new Size(266, 262);
            bucketTreeView.TabIndex = 0;
            bucketTreeView.NodeMouseClick += bucketTreeView_NodeMouseClick;
            bucketTreeView.NodeMouseDoubleClick += bucketTreeView_NodeMouseDoubleClick;
            // 
            // fileStatusStrip
            // 
            fileStatusStrip.Items.AddRange(new ToolStripItem[] { fileStatusToolStripStatusLabel });
            fileStatusStrip.Location = new Point(0, 240);
            fileStatusStrip.Name = "fileStatusStrip";
            fileStatusStrip.Size = new Size(530, 22);
            fileStatusStrip.TabIndex = 2;
            fileStatusStrip.Text = "statusStrip2";
            // 
            // fileStatusToolStripStatusLabel
            // 
            fileStatusToolStripStatusLabel.Name = "fileStatusToolStripStatusLabel";
            fileStatusToolStripStatusLabel.Size = new Size(63, 17);
            fileStatusToolStripStatusLabel.Text = "已选中0项";
            // 
            // fileListView
            // 
            fileListView.CheckBoxes = true;
            fileListView.Dock = DockStyle.Fill;
            fileListView.Location = new Point(0, 23);
            fileListView.Name = "fileListView";
            fileListView.Size = new Size(530, 239);
            fileListView.TabIndex = 1;
            fileListView.UseCompatibleStateImageBehavior = false;
            fileListView.DoubleClick += fileListView_DoubleClick;
            // 
            // fileInfoPanel
            // 
            fileInfoPanel.Controls.Add(locationTextBox);
            fileInfoPanel.Controls.Add(searchTextBox);
            fileInfoPanel.Dock = DockStyle.Top;
            fileInfoPanel.Location = new Point(0, 0);
            fileInfoPanel.Name = "fileInfoPanel";
            fileInfoPanel.Size = new Size(530, 23);
            fileInfoPanel.TabIndex = 0;
            // 
            // locationTextBox
            // 
            locationTextBox.Dock = DockStyle.Fill;
            locationTextBox.Location = new Point(0, 0);
            locationTextBox.Name = "locationTextBox";
            locationTextBox.Size = new Size(430, 23);
            locationTextBox.TabIndex = 0;
            locationTextBox.Text = "/";
            locationTextBox.KeyDown += locationTextBox_KeyDown;
            // 
            // searchTextBox
            // 
            searchTextBox.Dock = DockStyle.Right;
            searchTextBox.Location = new Point(430, 0);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.PlaceholderText = "按名称前缀过滤...";
            searchTextBox.Size = new Size(100, 23);
            searchTextBox.TabIndex = 1;
            searchTextBox.KeyDown += searchTextBox_KeyDown;
            // 
            // taskTabControl
            // 
            taskTabControl.Controls.Add(uploadTabPage);
            taskTabControl.Controls.Add(downloadTabPage);
            taskTabControl.Controls.Add(completeTabPage);
            taskTabControl.Dock = DockStyle.Fill;
            taskTabControl.Location = new Point(0, 0);
            taskTabControl.Name = "taskTabControl";
            taskTabControl.SelectedIndex = 0;
            taskTabControl.Size = new Size(800, 112);
            taskTabControl.TabIndex = 0;
            // 
            // uploadTabPage
            // 
            uploadTabPage.Controls.Add(uploadTabPageListView);
            uploadTabPage.Location = new Point(4, 26);
            uploadTabPage.Name = "uploadTabPage";
            uploadTabPage.Padding = new Padding(3);
            uploadTabPage.Size = new Size(792, 82);
            uploadTabPage.TabIndex = 1;
            uploadTabPage.Text = "上传任务列表";
            uploadTabPage.UseVisualStyleBackColor = true;
            // 
            // uploadTabPageListView
            // 
            uploadTabPageListView.Dock = DockStyle.Fill;
            uploadTabPageListView.Location = new Point(3, 3);
            uploadTabPageListView.Name = "uploadTabPageListView";
            uploadTabPageListView.Size = new Size(786, 76);
            uploadTabPageListView.TabIndex = 0;
            uploadTabPageListView.UseCompatibleStateImageBehavior = false;
            // 
            // downloadTabPage
            // 
            downloadTabPage.Controls.Add(downloadTabPageListView);
            downloadTabPage.Location = new Point(4, 26);
            downloadTabPage.Name = "downloadTabPage";
            downloadTabPage.Padding = new Padding(3);
            downloadTabPage.Size = new Size(792, 82);
            downloadTabPage.TabIndex = 2;
            downloadTabPage.Text = "下载任务列表";
            downloadTabPage.UseVisualStyleBackColor = true;
            // 
            // downloadTabPageListView
            // 
            downloadTabPageListView.Dock = DockStyle.Fill;
            downloadTabPageListView.Location = new Point(3, 3);
            downloadTabPageListView.Name = "downloadTabPageListView";
            downloadTabPageListView.Size = new Size(786, 76);
            downloadTabPageListView.TabIndex = 0;
            downloadTabPageListView.UseCompatibleStateImageBehavior = false;
            // 
            // completeTabPage
            // 
            completeTabPage.Controls.Add(completeTabPageListView);
            completeTabPage.Location = new Point(4, 26);
            completeTabPage.Name = "completeTabPage";
            completeTabPage.Padding = new Padding(3);
            completeTabPage.Size = new Size(792, 82);
            completeTabPage.TabIndex = 3;
            completeTabPage.Text = "已完成任务列表";
            completeTabPage.UseVisualStyleBackColor = true;
            // 
            // completeTabPageListView
            // 
            completeTabPageListView.Dock = DockStyle.Fill;
            completeTabPageListView.Location = new Point(3, 3);
            completeTabPageListView.Name = "completeTabPageListView";
            completeTabPageListView.Size = new Size(786, 76);
            completeTabPageListView.TabIndex = 0;
            completeTabPageListView.UseCompatibleStateImageBehavior = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainSplitContainer);
            Controls.Add(mainStatusStrip);
            Controls.Add(mainToolStrip);
            Controls.Add(mainMenuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenuStrip;
            Name = "MainForm";
            Text = "S3 Bucket Manager";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            mainToolStrip.ResumeLayout(false);
            mainToolStrip.PerformLayout();
            mainStatusStrip.ResumeLayout(false);
            mainStatusStrip.PerformLayout();
            mainSplitContainer.Panel1.ResumeLayout(false);
            mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)mainSplitContainer).EndInit();
            mainSplitContainer.ResumeLayout(false);
            dataSplitContainer.Panel1.ResumeLayout(false);
            dataSplitContainer.Panel2.ResumeLayout(false);
            dataSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataSplitContainer).EndInit();
            dataSplitContainer.ResumeLayout(false);
            fileStatusStrip.ResumeLayout(false);
            fileStatusStrip.PerformLayout();
            fileInfoPanel.ResumeLayout(false);
            fileInfoPanel.PerformLayout();
            taskTabControl.ResumeLayout(false);
            uploadTabPage.ResumeLayout(false);
            downloadTabPage.ResumeLayout(false);
            completeTabPage.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newConnectionToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem useLargeIconToolStripMenuItem;
        private ToolStripMenuItem useSmallIconToolStripMenuItem;
        private ToolStripMenuItem useListToolStripMenuItem;
        private ToolStripMenuItem useDetailToolStripMenuItem;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripMenuItem configToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStrip mainToolStrip;
        private ToolStripButton newConnectionToolStripButton;
        private StatusStrip mainStatusStrip;
        private SplitContainer mainSplitContainer;
        private SplitContainer dataSplitContainer;
        private TreeView bucketTreeView;
        private ListView fileListView;
        private Panel fileInfoPanel;
        private TextBox locationTextBox;
        private TextBox searchTextBox;
        private StatusStrip fileStatusStrip;
        private TabControl taskTabControl;
        private TabPage uploadTabPage;
        private TabPage downloadTabPage;
        private ListView uploadTabPageListView;
        private ListView downloadTabPageListView;
        private ToolStripStatusLabel mainStripStatusLabel;
        private ToolStripSeparator viewToolStripSeparator1;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripButton uploadToolStripButton;
        private ToolStripButton downloadToolStripButton;
        private ToolStripMenuItem uploadToolStripMenuItem;
        private ToolStripMenuItem downloadToolStripMenuItem;
        private ToolStripSeparator editToolStripSeparator1;
        private ToolStripButton selectAlltoolStripButton;
        private ToolStripButton copyToolStripButton;
        private ToolStripButton cutToolStripButton;
        private ToolStripButton pasteToolStripButton;
        private ToolStripButton deleteToolStripButton;
        private ToolStripMenuItem renameToolStripMenuItem;
        private ToolStripButton renameToolStripButton;
        private ToolStripButton refreshToolStripButton;
        private ToolStripStatusLabel fileStatusToolStripStatusLabel;
        private TabPage completeTabPage;
        private ListView completeTabPageListView;
    }
}
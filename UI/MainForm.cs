using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon.S3;
using Amazon.S3.Model;
using Gacfox.S3BucketManager.Models;
using Gacfox.S3BucketManager.Services;

namespace Gacfox.S3BucketManager.UI
{
    public partial class MainForm : Form
    {
        private const int ConnectionImageIndex = 0;
        private const int BucketImageIndex = 1;
        private const int FolderImageIndex = 0;
        private const int FileImageIndex = 1;

        private ConnectionStore _store = null!;
        private readonly HashSet<Guid> _loadingConnections = new();

        private ConnectionProfile? _currentProfile;
        private string? _currentBucket;
        private string _currentPrefix = "";
        private string? _activeSearch;
        private int _loadVersion;

        private const int TaskProgressColumnIndex = 2;
        private const int TaskActionColumnIndex = 4;
        private TransferManager _transferManager = null!;
        private readonly Dictionary<Guid, ListViewItem> _taskRows = new();
        private ImageList _actionImageList = null!;

        private ClipboardBuffer? _clipboard;
        private bool _actionsEnabled;
        private ListViewItem? _contextItem;
        private TreeNode? _contextConnectionNode;

        private static readonly object ParentDirectoryTag = new();

        private class ClipboardItem
        {
            public required string Name { get; init; }
            public required string SourceKey { get; init; }
            public required bool IsFolder { get; init; }
        }

        private class ClipboardBuffer
        {
            public required ConnectionProfile Profile { get; init; }
            public required string BucketName { get; init; }
            public required bool IsCut { get; init; }
            public required List<ClipboardItem> Items { get; init; }
        }

        public MainForm()
        {
            InitializeComponent();
            var imageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            imageList.Images.Add(Properties.Resources.database_yellow);
            imageList.Images.Add(Properties.Resources.package_white);
            bucketTreeView.ImageList = imageList;

            fileListView.Columns.Add("名称", 280);
            fileListView.Columns.Add("大小", 80, HorizontalAlignment.Right);
            fileListView.Columns.Add("修改时间", 150);
            var smallImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            smallImageList.Images.Add(Properties.Resources.folder);
            smallImageList.Images.Add(Properties.Resources.page_white);
            fileListView.SmallImageList = smallImageList;
            var largeImageList = new ImageList { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
            foreach (var source in new Image[] { Properties.Resources.folder, Properties.Resources.page_white })
                largeImageList.Images.Add(new Bitmap(source, largeImageList.ImageSize));
            fileListView.LargeImageList = largeImageList;
            SetFileView(View.Details);

            _actionImageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            _actionImageList.Images.Add(Properties.Resources.pause_blue);
            _actionImageList.Images.Add(Properties.Resources.resultset_next);
            _actionImageList.Images.Add(Properties.Resources.stop_blue);
            SetupTaskListView(uploadTabPageListView);
            SetupTaskListView(downloadTabPageListView);
            completeTabPageListView.View = View.Details;
            completeTabPageListView.FullRowSelect = true;
            completeTabPageListView.ShowItemToolTips = true;
            completeTabPageListView.Columns.Add("方向", 50);
            completeTabPageListView.Columns.Add("名称", 240);
            completeTabPageListView.Columns.Add("大小", 80, HorizontalAlignment.Right);
            completeTabPageListView.Columns.Add("状态", 70);
            completeTabPageListView.Columns.Add("完成时间", 130);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                _store = ConnectionStore.Load();
            }
            catch (Exception ex)
            {
                _store = new ConnectionStore();
                MessageBox.Show(this, $"读取连接配置失败：{ex.Message}", "警告",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            foreach (var profile in _store.Connections)
                bucketTreeView.Nodes.Add(CreateConnectionNode(profile));
            _transferManager = new TransferManager(_store);
            _transferManager.TaskAdded += t => BeginInvoke(new Action(() => OnTransferTaskAdded(t)));
            _transferManager.TaskUpdated += t => BeginInvoke(new Action(() => OnTransferTaskUpdated(t)));
            _transferManager.TaskFinished += t => BeginInvoke(new Action(() => OnTransferTaskFinished(t)));
            _transferManager.PersistRequested += () => BeginInvoke(new Action(PersistActiveTasks));
            RestorePersistedTasks();
            SetActionButtonsEnabled(false);
        }

        private void RestorePersistedTasks()
        {
            var restored = 0;
            foreach (var snapshot in TransferStore.Load())
            {
                var profile = _store.Connections.FirstOrDefault(c => c.Id == snapshot.ConnectionId);
                if (profile == null) continue;
                _transferManager.Restore(TransferTask.FromSnapshot(snapshot, profile));
                restored++;
            }
            if (restored > 0)
                mainStripStatusLabel.Text = $"已恢复 {restored} 个未完成的传输任务，可手动继续";
        }

        private void PersistActiveTasks()
        {
            var snapshots = _taskRows.Values
                .Select(item => item.Tag as TransferTask)
                .Where(task => task != null
                    && task.Status is TransferStatus.Pending or TransferStatus.Running or TransferStatus.Paused)
                .Select(task => task!.ToSnapshot())
                .ToList();
            TransferStore.Save(snapshots);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            PersistActiveTasks();
            base.OnFormClosing(e);
        }

        private void newConnectionToolStripMenuItem_Click(object sender, EventArgs e) => AddConnection();

        private void newConnectionToolStripButton_Click(object sender, EventArgs e) => AddConnection();

        private void AddConnection()
        {
            using var dialog = new EditConnectionDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            var profile = dialog.Profile!;
            _store.Add(profile, dialog.Credentials!);
            bucketTreeView.Nodes.Add(CreateConnectionNode(profile));
        }

        private static TreeNode CreateConnectionNode(ConnectionProfile profile)
            => new(profile.Name, ConnectionImageIndex, ConnectionImageIndex) { Tag = profile };

        private async void bucketTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is not ConnectionProfile profile) return;
            if (e.Node.Nodes.Count > 0)
            {
                e.Node.Toggle();
                return;
            }
            await LoadConnectionBucketsAsync(e.Node, profile);
        }

        private async Task LoadConnectionBucketsAsync(TreeNode node, ConnectionProfile profile)
        {
            if (!_loadingConnections.Add(profile.Id)) return;
            var credentials = _store.GetCredentials(profile.Id);
            if (credentials == null)
            {
                _loadingConnections.Remove(profile.Id);
                MessageBox.Show(this, $"连接“{profile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mainStripStatusLabel.Text = $"正在连接 {profile.Name}...";
            UseWaitCursor = true;
            try
            {
                using var client = S3ClientFactory.Create(profile, credentials);
                var response = await client.ListBucketsAsync();
                var buckets = response.Buckets ?? new List<S3Bucket>();
                foreach (var bucket in buckets.OrderBy(b => b.BucketName))
                    node.Nodes.Add(new TreeNode(bucket.BucketName, BucketImageIndex, BucketImageIndex)
                    { Tag = bucket.BucketName });
                node.Expand();
                mainStripStatusLabel.Text = $"{profile.Name}：共 {buckets.Count} 个存储桶";
            }
            catch (Exception ex)
            {
                mainStripStatusLabel.Text = "就绪";
                MessageBox.Show(this, $"获取 {profile.Name} 的存储桶列表失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                _loadingConnections.Remove(profile.Id);
            }
        }

        private async void bucketTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.Node.Tag is not ConnectionProfile) return;
                _contextConnectionNode = e.Node;
                bucketTreeView.SelectedNode = e.Node;
                var connected = e.Node.Nodes.Count > 0;
                connectContextStripMenuItem.Enabled = !connected;
                editContextStripMenuItem.Enabled = !connected;
                disconnectContextStripMenuItem.Enabled = connected;
                reconnectContextStripMenuItem.Enabled = connected;
                connectionContextMenuStrip.Show(bucketTreeView, e.Location);
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            if (e.Node.Tag is not string bucketName) return;
            if (e.Node.Parent?.Tag is not ConnectionProfile profile) return;
            _currentProfile = profile;
            _currentBucket = bucketName;
            await LoadObjectsAsync("", null);
        }

        private async void fileListView_DoubleClick(object sender, EventArgs e)
        {
            if (fileListView.SelectedItems.Count == 0) return;
            var tag = fileListView.SelectedItems[0].Tag;
            if (ReferenceEquals(tag, ParentDirectoryTag))
                await LoadObjectsAsync(ParentPrefix(_currentPrefix), null);
            else if (tag is string folderPrefix)
                await LoadObjectsAsync(folderPrefix, null);
        }

        private static string ParentPrefix(string prefix)
        {
            var trimmed = prefix.TrimEnd('/');
            var index = trimmed.LastIndexOf('/');
            return index < 0 ? "" : trimmed[..(index + 1)];
        }

        private async void locationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                locationTextBox.Text = DisplayPath(_currentPrefix);
                e.SuppressKeyPress = true;
                return;
            }
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (_currentBucket == null)
            {
                locationTextBox.Text = DisplayPath(_currentPrefix);
                mainStripStatusLabel.Text = "请先在左侧选择一个存储桶";
                return;
            }
            await LoadObjectsAsync(NormalizePrefix(locationTextBox.Text), null, requireExisting: true);
        }

        private async void searchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (_currentBucket == null)
            {
                mainStripStatusLabel.Text = "请先在左侧选择一个存储桶";
                return;
            }
            var text = searchTextBox.Text.Trim();
            await LoadObjectsAsync(_currentPrefix, text.Length == 0 ? null : text);
        }

        private async void refreshToolStripMenuItem_Click(object sender, EventArgs e) => await RefreshCurrentViewAsync();

        private async void refreshToolStripButton_Click(object sender, EventArgs e) => await RefreshCurrentViewAsync();

        private async Task RefreshCurrentViewAsync()
        {
            if (_currentBucket == null) return;
            await LoadObjectsAsync(_currentPrefix, _activeSearch);
        }

        private async Task LoadObjectsAsync(string prefix, string? search, bool requireExisting = false)
        {
            if (_currentBucket == null || _currentProfile == null) return;
            var credentials = _store.GetCredentials(_currentProfile.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{_currentProfile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var version = ++_loadVersion;
            UseWaitCursor = true;
            mainStripStatusLabel.Text = "正在加载...";
            try
            {
                using var client = S3ClientFactory.Create(_currentProfile, credentials);
                var request = new ListObjectsV2Request
                {
                    BucketName = _currentBucket,
                    MaxKeys = 1000,
                    Delimiter = "/",
                    Prefix = search == null ? prefix : prefix + search
                };
                var response = await client.ListObjectsV2Async(request);
                if (version != _loadVersion) return;
                var commonPrefixes = response.CommonPrefixes ?? new List<string>();
                var s3Objects = response.S3Objects ?? new List<S3Object>();
                if (requireExisting && search == null
                    && commonPrefixes.Count == 0 && s3Objects.Count == 0)
                {
                    locationTextBox.Text = DisplayPath(_currentPrefix);
                    MessageBox.Show(this, $"路径 “{DisplayPath(prefix)}” 不存在。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _currentPrefix = prefix;
                _activeSearch = search;
                fileListView.BeginUpdate();
                fileListView.Items.Clear();
                if (search == null)
                {
                    searchTextBox.Clear();
                    locationTextBox.Text = DisplayPath(prefix);
                    if (prefix.Length > 0)
                        fileListView.Items.Add(new ListViewItem("..", FolderImageIndex)
                        { Tag = ParentDirectoryTag });
                }
                foreach (var commonPrefix in commonPrefixes)
                    fileListView.Items.Add(new ListViewItem(commonPrefix[prefix.Length..].TrimEnd('/'), FolderImageIndex)
                    { Tag = commonPrefix });
                foreach (var obj in s3Objects)
                {
                    if (obj.Key == prefix) continue;
                    fileListView.Items.Add(CreateFileItem(obj, obj.Key[prefix.Length..]));
                }
                fileListView.EndUpdate();
                var shownCount = fileListView.Items.Count - (search == null && prefix.Length > 0 ? 1 : 0);
                fileStatusToolStripStatusLabel.Text = $"共 {shownCount} 项"
                    + (response.IsTruncated == true ? "（结果过多，仅显示前1000项）" : "");
                SetActionButtonsEnabled(true);
            }
            catch (Exception ex)
            {
                if (version != _loadVersion) return;
                SetActionButtonsEnabled(false);
                MessageBox.Show(this, $"加载失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (version == _loadVersion)
                {
                    UseWaitCursor = false;
                    mainStripStatusLabel.Text = "就绪";
                }
            }
        }

        private static ListViewItem CreateFileItem(S3Object obj, string displayName)
        {
            var item = new ListViewItem(displayName, FileImageIndex) { Tag = obj };
            item.SubItems.Add(FormatSize(obj.Size ?? 0));
            item.SubItems.Add(obj.LastModified.HasValue
                ? obj.LastModified.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") : "");
            return item;
        }

        private static string NormalizePrefix(string input)
        {
            var p = input.Trim().Replace('\\', '/').Trim('/');
            return p.Length == 0 ? "" : p + "/";
        }

        private static string DisplayPath(string prefix) => "/" + prefix;

        private static string FormatSize(long size) => size switch
        {
            < 1024 => $"{size} B",
            < 1024 * 1024 => $"{size / 1024.0:F1} KB",
            < 1024L * 1024 * 1024 => $"{size / 1024.0 / 1024:F1} MB",
            _ => $"{size / 1024.0 / 1024 / 1024:F2} GB"
        };

        private void useLargeIconToolStripMenuItem_Click(object sender, EventArgs e) => SetFileView(View.LargeIcon);

        private void useSmallIconToolStripMenuItem_Click(object sender, EventArgs e) => SetFileView(View.SmallIcon);

        private void useListToolStripMenuItem_Click(object sender, EventArgs e) => SetFileView(View.List);

        private void useDetailToolStripMenuItem_Click(object sender, EventArgs e) => SetFileView(View.Details);

        private void SetFileView(View view)
        {
            fileListView.View = view;
            useLargeIconToolStripMenuItem.Checked = view == View.LargeIcon;
            useSmallIconToolStripMenuItem.Checked = view == View.SmallIcon;
            useListToolStripMenuItem.Checked = view == View.List;
            useDetailToolStripMenuItem.Checked = view == View.Details;
        }

        private void StartUpload()
        {
            if (_currentBucket == null || _currentProfile == null) return;
            using var dialog = new OpenFileDialog { Title = "选择要上传的文件", Multiselect = true };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            EnqueueUploads(dialog.FileNames);
        }

        private void EnqueueUploads(IEnumerable<string> files)
        {
            if (_currentBucket == null || _currentProfile == null) return;
            var any = false;
            foreach (var file in files)
            {
                any = true;
                _transferManager.Enqueue(new TransferTask
                {
                    Direction = TransferDirection.Upload,
                    Profile = _currentProfile,
                    BucketName = _currentBucket,
                    DisplayName = Path.GetFileName(file),
                    LocalFilePath = file,
                    Key = _currentPrefix + Path.GetFileName(file),
                    TotalBytes = new FileInfo(file).Length
                });
            }
            if (any) taskTabControl.SelectedTab = uploadTabPage;
        }

        private void StartDownload()
        {
            if (_currentBucket == null || _currentProfile == null) return;
            if (fileListView.CheckedItems.Count == 0)
            {
                mainStripStatusLabel.Text = "请先勾选要下载的文件或文件夹";
                return;
            }
            using var dialog = new FolderBrowserDialog { Description = "选择下载保存位置" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            foreach (ListViewItem item in fileListView.CheckedItems)
            {
                item.Checked = false;
                EnqueueDownloadItem(item, dialog.SelectedPath);
            }
            taskTabControl.SelectedTab = downloadTabPage;
        }

        private void EnqueueDownloadItem(ListViewItem item, string targetDirectory)
        {
            if (_currentBucket == null || _currentProfile == null) return;
            TransferTask? task = null;
            if (item.Tag is string folderPrefix)
            {
                var folderName = folderPrefix.TrimEnd('/').Split('/').Last();
                task = new TransferTask
                {
                    Direction = TransferDirection.Download,
                    Profile = _currentProfile,
                    BucketName = _currentBucket,
                    DisplayName = folderName,
                    SourcePrefix = folderPrefix,
                    LocalTargetPath = Path.Combine(targetDirectory, folderName)
                };
            }
            else if (item.Tag is S3Object obj)
            {
                var fileName = obj.Key.Split('/').Last();
                task = new TransferTask
                {
                    Direction = TransferDirection.Download,
                    Profile = _currentProfile,
                    BucketName = _currentBucket,
                    DisplayName = fileName,
                    Key = obj.Key,
                    TotalBytes = obj.Size ?? 0,
                    LocalTargetPath = Path.Combine(targetDirectory, fileName)
                };
            }
            if (task != null) _transferManager.Enqueue(task);
        }

        private void SetActionButtonsEnabled(bool enabled)
        {
            _actionsEnabled = enabled;
            uploadToolStripButton.Enabled = uploadToolStripMenuItem.Enabled = enabled;
            downloadToolStripButton.Enabled = downloadToolStripMenuItem.Enabled = enabled;
            selectAlltoolStripButton.Enabled = selectAllToolStripMenuItem.Enabled = enabled;
            copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = enabled;
            cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled = enabled;
            deleteToolStripButton.Enabled = deleteToolStripMenuItem.Enabled = enabled;
            renameToolStripButton.Enabled = renameToolStripMenuItem.Enabled = enabled;
            UpdatePasteButton();
        }

        private void UpdatePasteButton()
        {
            pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = _actionsEnabled && _clipboard != null;
        }

        private void SetupTaskListView(ListView listView)
        {
            listView.View = View.Details;
            listView.FullRowSelect = true;
            listView.ShowItemToolTips = true;
            listView.OwnerDraw = true;
            listView.Columns.Add("名称", 240);
            listView.Columns.Add("大小", 80, HorizontalAlignment.Right);
            listView.Columns.Add("进度", 110);
            listView.Columns.Add("状态", 70);
            listView.Columns.Add("操作", 50);
            listView.DrawColumnHeader += taskListView_DrawColumnHeader;
            listView.DrawItem += taskListView_DrawItem;
            listView.MouseClick += taskListView_MouseClick;
        }

        private void taskListView_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
            => e.DrawDefault = true;

        private void taskListView_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = false;
            var listView = (ListView)sender!;
            var task = e.Item.Tag as TransferTask;
            var selected = e.Item.Selected;
            var textColor = selected ? SystemColors.HighlightText : SystemColors.WindowText;
            var bounds = new Rectangle[listView.Columns.Count];
            var x = e.Bounds.X;
            for (var i = 0; i < bounds.Length; i++)
            {
                bounds[i] = new Rectangle(x, e.Bounds.Y, listView.Columns[i].Width, e.Bounds.Height);
                x += listView.Columns[i].Width;
            }
            using (var brush = new SolidBrush(selected ? SystemColors.Highlight : SystemColors.Window))
                e.Graphics.FillRectangle(brush,
                    new Rectangle(e.Bounds.X, e.Bounds.Y, x - e.Bounds.X, e.Bounds.Height));
            TextRenderer.DrawText(e.Graphics, e.Item.Text, listView.Font, bounds[0], textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
                | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[1].Text, listView.Font, bounds[1], textColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            TextRenderer.DrawText(e.Graphics, e.Item.SubItems[3].Text, listView.Font, bounds[3], textColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            if (task == null) return;
            var percent = task.TotalBytes > 0
                ? Math.Clamp((int)(task.TransferredBytes * 100 / task.TotalBytes), 0, 100)
                : 0;
            var barBounds = new Rectangle(bounds[TaskProgressColumnIndex].X + 2,
                bounds[TaskProgressColumnIndex].Y + 3,
                bounds[TaskProgressColumnIndex].Width - 4,
                bounds[TaskProgressColumnIndex].Height - 6);
            if (ProgressBarRenderer.IsSupported)
            {
                ProgressBarRenderer.DrawHorizontalBar(e.Graphics, barBounds);
                if (percent > 0)
                    ProgressBarRenderer.DrawHorizontalChunks(e.Graphics,
                        new Rectangle(barBounds.X, barBounds.Y,
                            barBounds.Width * percent / 100, barBounds.Height));
            }
            TextRenderer.DrawText(e.Graphics, $"{percent}%", listView.Font,
                bounds[TaskProgressColumnIndex], textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
            var iconY = bounds[TaskActionColumnIndex].Y
                + (bounds[TaskActionColumnIndex].Height - 16) / 2;
            _actionImageList.Draw(e.Graphics, bounds[TaskActionColumnIndex].X + 4, iconY,
                task.Status == TransferStatus.Paused ? 1 : 0);
            _actionImageList.Draw(e.Graphics, bounds[TaskActionColumnIndex].X + 24, iconY, 2);
        }

        private void taskListView_MouseClick(object? sender, MouseEventArgs e)
        {
            var listView = (ListView)sender!;
            var hit = listView.HitTest(e.Location);
            if (hit.Item?.Tag is not TransferTask task) return;
            if (hit.SubItem != hit.Item.SubItems[TaskActionColumnIndex]) return;
            if (e.X - hit.SubItem.Bounds.X < 20)
            {
                if (task.Status is TransferStatus.Running or TransferStatus.Pending)
                    _transferManager.Pause(task);
                else if (task.Status == TransferStatus.Paused)
                    _transferManager.Resume(task);
            }
            else
            {
                _transferManager.Stop(task);
            }
        }

        private void OnTransferTaskAdded(TransferTask task)
        {
            var listView = task.Direction == TransferDirection.Upload
                ? uploadTabPageListView : downloadTabPageListView;
            var item = new ListViewItem(task.DisplayName) { Tag = task };
            item.SubItems.Add(FormatSize(task.TotalBytes));
            item.SubItems.Add("");
            item.SubItems.Add(StatusText(task));
            item.SubItems.Add("");
            _taskRows[task.Id] = item;
            listView.Items.Add(item);
        }

        private void OnTransferTaskUpdated(TransferTask task)
        {
            if (!_taskRows.TryGetValue(task.Id, out var item)) return;
            item.SubItems[1].Text = FormatSize(task.TotalBytes);
            item.SubItems[3].Text = StatusText(task);
            item.ToolTipText = task.ErrorMessage ?? "";
            item.ListView?.Invalidate(item.GetBounds(ItemBoundsPortion.Entire));
        }

        private async void OnTransferTaskFinished(TransferTask task)
        {
            if (_taskRows.TryGetValue(task.Id, out var item))
            {
                item.Remove();
                _taskRows.Remove(task.Id);
            }
            var done = new ListViewItem(task.Direction == TransferDirection.Upload ? "上传" : "下载") { Tag = task };
            done.SubItems.Add(task.DisplayName);
            done.SubItems.Add(FormatSize(task.TotalBytes));
            done.SubItems.Add(StatusText(task));
            done.SubItems.Add(task.FinishTime.ToString("yyyy-MM-dd HH:mm:ss"));
            done.ToolTipText = task.ErrorMessage ?? "";
            completeTabPageListView.Items.Add(done);
            if (task.Direction == TransferDirection.Upload
                && uploadTabPageListView.Items.Count == 0
                && task.BucketName == _currentBucket)
            {
                await RefreshCurrentViewAsync();
            }
        }

        private static string StatusText(TransferTask task) => task.Status switch
        {
            TransferStatus.Pending => "等待中",
            TransferStatus.Running => task.PauseRequested ? "暂停中…"
                : task.StopRequested ? "停止中…"
                : task.Direction == TransferDirection.Upload ? "上传中" : "下载中",
            TransferStatus.Paused => "已暂停",
            TransferStatus.Completed => "已完成",
            TransferStatus.Stopped => "已停止",
            TransferStatus.Failed => "失败",
            _ => ""
        };

        private void CheckAllItems()
        {
            foreach (ListViewItem item in fileListView.Items)
                item.Checked = true;
        }

        private void BufferCheckedItems(bool isCut)
        {
            if (_currentBucket == null || _currentProfile == null) return;
            var items = new List<ClipboardItem>();
            foreach (ListViewItem item in fileListView.CheckedItems)
            {
                var entry = CreateClipboardItem(item);
                if (entry != null) items.Add(entry);
            }
            if (items.Count == 0)
            {
                mainStripStatusLabel.Text = "请先勾选要操作的文件或文件夹";
                return;
            }
            BufferItems(items, isCut);
        }

        private void BufferContextItem(bool isCut)
        {
            if (_contextItem == null || _currentBucket == null || _currentProfile == null) return;
            var entry = CreateClipboardItem(_contextItem);
            if (entry == null) return;
            BufferItems(new List<ClipboardItem> { entry }, isCut);
        }

        private void BufferItems(List<ClipboardItem> items, bool isCut)
        {
            _clipboard = new ClipboardBuffer
            {
                Profile = _currentProfile!,
                BucketName = _currentBucket!,
                IsCut = isCut,
                Items = items
            };
            UpdatePasteButton();
            mainStripStatusLabel.Text = isCut ? $"已剪切 {items.Count} 项" : $"已复制 {items.Count} 项";
        }

        private static ClipboardItem? CreateClipboardItem(ListViewItem item)
        {
            if (item.Tag is string folderPrefix)
                return new ClipboardItem
                {
                    Name = folderPrefix.TrimEnd('/').Split('/').Last(),
                    SourceKey = folderPrefix,
                    IsFolder = true
                };
            if (item.Tag is S3Object obj)
                return new ClipboardItem
                {
                    Name = obj.Key.Split('/').Last(),
                    SourceKey = obj.Key,
                    IsFolder = false
                };
            return null;
        }

        private async Task PasteAsync(string targetPrefix)
        {
            if (_clipboard == null || _currentBucket == null || _currentProfile == null) return;
            if (_clipboard.Profile.Id != _currentProfile.Id)
            {
                MessageBox.Show(this, "不支持跨连接粘贴。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var credentials = _store.GetCredentials(_currentProfile.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{_currentProfile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UseWaitCursor = true;
            mainStripStatusLabel.Text = "正在粘贴...";
            var errors = new List<string>();
            var pasted = 0;
            try
            {
                using var client = S3ClientFactory.Create(_currentProfile, credentials);
                foreach (var entry in _clipboard.Items)
                {
                    try
                    {
                        pasted += await PasteEntryAsync(client, _clipboard, entry, targetPrefix);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{entry.Name}：{ex.Message}");
                    }
                }
            }
            finally
            {
                UseWaitCursor = false;
                mainStripStatusLabel.Text = "就绪";
            }
            if (_clipboard.IsCut && errors.Count == 0)
            {
                _clipboard = null;
                UpdatePasteButton();
            }
            if (pasted > 0) await RefreshCurrentViewAsync();
            if (errors.Count > 0)
                MessageBox.Show(this, "部分项目粘贴失败：\n" + string.Join("\n", errors), "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task<int> PasteEntryAsync(AmazonS3Client client, ClipboardBuffer buffer, ClipboardItem entry, string targetPrefix)
        {
            var destName = entry.Name;
            var suffix = entry.IsFolder ? "/" : "";
            if (buffer.IsCut)
            {
                if (buffer.BucketName == _currentBucket && targetPrefix + destName + suffix == entry.SourceKey)
                    return 0;
                if (entry.IsFolder && buffer.BucketName == _currentBucket
                    && (targetPrefix + destName + "/").StartsWith(entry.SourceKey))
                    throw new InvalidOperationException("不能将文件夹移动到其自身内部");
            }
            else if (await ExistsAsync(client, _currentBucket!, targetPrefix + destName + suffix, entry.IsFolder))
            {
                destName = CopyName(destName, entry.IsFolder);
            }
            if (!entry.IsFolder)
            {
                await client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = buffer.BucketName,
                    SourceKey = entry.SourceKey,
                    DestinationBucket = _currentBucket,
                    DestinationKey = targetPrefix + destName
                });
                if (buffer.IsCut)
                    await client.DeleteObjectAsync(buffer.BucketName, entry.SourceKey);
                return 1;
            }
            var sourcePrefix = entry.SourceKey;
            var destPrefix = targetPrefix + destName + "/";
            var keys = await ListAllKeysAsync(client, buffer.BucketName, sourcePrefix);
            foreach (var key in keys)
            {
                await client.CopyObjectAsync(new CopyObjectRequest
                {
                    SourceBucket = buffer.BucketName,
                    SourceKey = key,
                    DestinationBucket = _currentBucket,
                    DestinationKey = destPrefix + key[sourcePrefix.Length..]
                });
            }
            if (buffer.IsCut)
                foreach (var key in keys)
                    await client.DeleteObjectAsync(buffer.BucketName, key);
            return keys.Count;
        }

        private async Task DeleteCheckedItemsAsync()
        {
            if (_currentBucket == null || _currentProfile == null) return;
            var fileKeys = new List<string>();
            var folderPrefixes = new List<string>();
            foreach (ListViewItem item in fileListView.CheckedItems)
            {
                if (item.Tag is string folderPrefix) folderPrefixes.Add(folderPrefix);
                else if (item.Tag is S3Object obj) fileKeys.Add(obj.Key);
            }
            if (fileKeys.Count + folderPrefixes.Count == 0)
            {
                mainStripStatusLabel.Text = "请先勾选要删除的文件或文件夹";
                return;
            }
            await DeleteItemsAsync(fileKeys, folderPrefixes);
        }

        private async Task DeleteItemsAsync(List<string> fileKeys, List<string> folderPrefixes)
        {
            var message = $"将删除 {fileKeys.Count} 个文件";
            if (folderPrefixes.Count > 0)
                message += $"、{folderPrefixes.Count} 个文件夹及其全部内容";
            if (MessageBox.Show(this, message + "，是否继续？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            var credentials = _store.GetCredentials(_currentProfile!.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{_currentProfile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UseWaitCursor = true;
            mainStripStatusLabel.Text = "正在删除...";
            var errors = new List<string>();
            try
            {
                using var client = S3ClientFactory.Create(_currentProfile, credentials);
                foreach (var key in fileKeys)
                {
                    try { await client.DeleteObjectAsync(_currentBucket, key); }
                    catch (Exception ex) { errors.Add($"{key}：{ex.Message}"); }
                }
                foreach (var prefix in folderPrefixes)
                {
                    try
                    {
                        foreach (var key in await ListAllKeysAsync(client, _currentBucket!, prefix))
                            await client.DeleteObjectAsync(_currentBucket, key);
                    }
                    catch (Exception ex) { errors.Add($"{prefix}：{ex.Message}"); }
                }
            }
            finally
            {
                UseWaitCursor = false;
                mainStripStatusLabel.Text = "就绪";
            }
            await RefreshCurrentViewAsync();
            if (errors.Count > 0)
                MessageBox.Show(this, "部分项目删除失败：\n" + string.Join("\n", errors), "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async Task RenameCheckedItemAsync()
        {
            if (_currentBucket == null || _currentProfile == null) return;
            if (fileListView.CheckedItems.Count != 1)
            {
                mainStripStatusLabel.Text = "请先勾选一个要重命名的文件或文件夹";
                return;
            }
            await RenameItemAsync(fileListView.CheckedItems[0]);
        }

        private async Task RenameItemAsync(ListViewItem item)
        {
            if (_currentBucket == null || _currentProfile == null) return;
            string oldName, sourceKey;
            bool isFolder;
            if (item.Tag is string folderPrefix)
            {
                isFolder = true;
                sourceKey = folderPrefix;
                oldName = folderPrefix.TrimEnd('/').Split('/').Last();
            }
            else if (item.Tag is S3Object obj)
            {
                isFolder = false;
                sourceKey = obj.Key;
                oldName = obj.Key.Split('/').Last();
            }
            else return;
            string newName;
            using (var dialog = new RenameDialog(oldName))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                newName = dialog.NewName;
            }
            if (newName.Length == 0 || newName == oldName) return;
            if (newName.Contains('/'))
            {
                MessageBox.Show(this, "名称不能包含“/”。", "提示",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var credentials = _store.GetCredentials(_currentProfile.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{_currentProfile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            UseWaitCursor = true;
            mainStripStatusLabel.Text = "正在重命名...";
            try
            {
                using var client = S3ClientFactory.Create(_currentProfile, credentials);
                var targetKey = _currentPrefix + newName + (isFolder ? "/" : "");
                if (await ExistsAsync(client, _currentBucket, targetKey, isFolder))
                {
                    MessageBox.Show(this, "当前路径下已存在同名文件或文件夹。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (isFolder)
                {
                    var keys = await ListAllKeysAsync(client, _currentBucket, sourceKey);
                    foreach (var key in keys)
                    {
                        await client.CopyObjectAsync(new CopyObjectRequest
                        {
                            SourceBucket = _currentBucket,
                            SourceKey = key,
                            DestinationBucket = _currentBucket,
                            DestinationKey = targetKey + key[sourceKey.Length..]
                        });
                    }
                    foreach (var key in keys)
                        await client.DeleteObjectAsync(_currentBucket, key);
                }
                else
                {
                    await client.CopyObjectAsync(new CopyObjectRequest
                    {
                        SourceBucket = _currentBucket,
                        SourceKey = sourceKey,
                        DestinationBucket = _currentBucket,
                        DestinationKey = targetKey
                    });
                    await client.DeleteObjectAsync(_currentBucket, sourceKey);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"重命名失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                UseWaitCursor = false;
                mainStripStatusLabel.Text = "就绪";
            }
            item.Checked = false;
            await RefreshCurrentViewAsync();
        }

        private static async Task<bool> ExistsAsync(AmazonS3Client client, string bucket, string keyOrPrefix, bool isFolder)
        {
            if (isFolder)
            {
                var response = await client.ListObjectsV2Async(new ListObjectsV2Request
                { BucketName = bucket, Prefix = keyOrPrefix, MaxKeys = 1 });
                return (response.CommonPrefixes?.Count ?? 0) + (response.S3Objects?.Count ?? 0) > 0;
            }
            try
            {
                await client.GetObjectMetadataAsync(bucket, keyOrPrefix);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        private static async Task<List<string>> ListAllKeysAsync(AmazonS3Client client, string bucket, string prefix)
        {
            var keys = new List<string>();
            string? token = null;
            do
            {
                var response = await client.ListObjectsV2Async(new ListObjectsV2Request
                { BucketName = bucket, Prefix = prefix, ContinuationToken = token });
                foreach (var obj in response.S3Objects ?? new List<S3Object>())
                    keys.Add(obj.Key);
                token = response.IsTruncated == true ? response.NextContinuationToken : null;
            } while (token != null);
            return keys;
        }

        private static string CopyName(string name, bool isFolder)
        {
            if (isFolder) return name + "_copy";
            var dot = name.LastIndexOf('.');
            return dot > 0 ? name[..dot] + "_copy" + name[dot..] : name + "_copy";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e) => StartUpload();

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e) => StartDownload();

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) => CheckAllItems();

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) => BufferCheckedItems(false);

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) => BufferCheckedItems(true);

        private async void pasteToolStripMenuItem_Click(object sender, EventArgs e) => await PasteAsync(_currentPrefix);

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e) => await DeleteCheckedItemsAsync();

        private async void renameToolStripMenuItem_Click(object sender, EventArgs e) => await RenameCheckedItemAsync();

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new PropertiesDialog(_store.Settings);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            _store.SaveSettings();
            _transferManager.UpdateConcurrency(
                _store.Settings.UploadConcurrency, _store.Settings.DownloadConcurrency);
            mainStripStatusLabel.Text = "设置已保存";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new AboutDialog();
            dialog.ShowDialog(this);
        }

        private void uploadToolStripButton_Click(object sender, EventArgs e) => StartUpload();

        private void downloadToolStripButton_Click(object sender, EventArgs e) => StartDownload();

        private void selectAlltoolStripButton_Click(object sender, EventArgs e) => CheckAllItems();

        private void copyToolStripButton_Click(object sender, EventArgs e) => BufferCheckedItems(false);

        private void cutToolStripButton_Click(object sender, EventArgs e) => BufferCheckedItems(true);

        private async void pasteToolStripButton_Click(object sender, EventArgs e) => await PasteAsync(_currentPrefix);

        private async void deleteToolStripButton_Click(object sender, EventArgs e) => await DeleteCheckedItemsAsync();

        private async void renameToolStripButton_Click(object sender, EventArgs e) => await RenameCheckedItemAsync();

        private void fileListView_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            var hit = fileListView.HitTest(e.Location);
            if (hit.Item == null || ReferenceEquals(hit.Item.Tag, ParentDirectoryTag)) return;
            _contextItem = hit.Item;
            fileListView.SelectedItems.Clear();
            hit.Item.Selected = true;
            pasteContextStripMenuItem.Enabled = _clipboard != null;
            genLinkContextStripMenuItem.Enabled = hit.Item.Tag is S3Object;
            fileContextMenuStrip.Show(fileListView, e.Location);
        }

        private void copyContextStripMenuItem_Click(object sender, EventArgs e) => BufferContextItem(false);

        private void cutContextStripMenuItem_Click(object sender, EventArgs e) => BufferContextItem(true);

        private async void pasteContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextItem == null) return;
            var targetPrefix = _contextItem.Tag is string folderPrefix ? folderPrefix : _currentPrefix;
            await PasteAsync(targetPrefix);
        }

        private async void deleteContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextItem == null) return;
            if (_contextItem.Tag is string folderPrefix)
                await DeleteItemsAsync(new List<string>(), new List<string> { folderPrefix });
            else if (_contextItem.Tag is S3Object obj)
                await DeleteItemsAsync(new List<string> { obj.Key }, new List<string>());
        }

        private async void renameContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextItem != null) await RenameItemAsync(_contextItem);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void downloadContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextItem == null || _currentBucket == null || _currentProfile == null) return;
            using var dialog = new FolderBrowserDialog { Description = "选择下载保存位置" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            EnqueueDownloadItem(_contextItem, dialog.SelectedPath);
            taskTabControl.SelectedTab = downloadTabPage;
        }

        private void genLinkContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextItem?.Tag is not S3Object obj) return;
            if (_currentBucket == null || _currentProfile == null) return;
            var credentials = _store.GetCredentials(_currentProfile.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{_currentProfile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using var client = S3ClientFactory.Create(_currentProfile, credentials);
                var url = client.GetPreSignedURL(new GetPreSignedUrlRequest
                {
                    BucketName = _currentBucket,
                    Key = obj.Key,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddSeconds(_store.Settings.LinkExpirationSeconds)
                });
                Clipboard.SetText(url);
                mainStripStatusLabel.Text =
                    $"下载链接已复制到剪贴板（有效期 {FormatDuration(_store.Settings.LinkExpirationSeconds)}）";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"生成链接失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string FormatDuration(int seconds) => seconds switch
        {
            >= 86400 => $"{seconds / 86400.0:0.#} 天",
            >= 3600 => $"{seconds / 3600.0:0.#} 小时",
            >= 60 => $"{seconds / 60} 分钟",
            _ => $"{seconds} 秒"
        };

        private async void connectContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextConnectionNode?.Tag is not ConnectionProfile profile) return;
            await LoadConnectionBucketsAsync(_contextConnectionNode, profile);
        }

        private void disconnectContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextConnectionNode?.Tag is not ConnectionProfile profile) return;
            _contextConnectionNode.Nodes.Clear();
            _contextConnectionNode.Collapse();
            if (_currentProfile?.Id == profile.Id)
                ResetCurrentView();
            mainStripStatusLabel.Text = $"已断开 {profile.Name}";
        }

        private async void reconnectContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextConnectionNode?.Tag is not ConnectionProfile profile) return;
            _contextConnectionNode.Nodes.Clear();
            await LoadConnectionBucketsAsync(_contextConnectionNode, profile);
        }

        private void deleteConnectContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextConnectionNode?.Tag is not ConnectionProfile profile) return;
            if (MessageBox.Show(this, $"将删除连接“{profile.Name}”及其保存的凭据，是否继续？", "确认删除",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            if (_currentProfile?.Id == profile.Id)
                ResetCurrentView();
            _store.Remove(profile.Id);
            _contextConnectionNode.Remove();
        }

        private void ResetCurrentView()
        {
            _loadVersion++;
            _currentProfile = null;
            _currentBucket = null;
            _currentPrefix = "";
            _activeSearch = null;
            fileListView.Items.Clear();
            locationTextBox.Text = "/";
            searchTextBox.Clear();
            fileStatusToolStripStatusLabel.Text = "共 0 项";
            SetActionButtonsEnabled(false);
        }

        private void editContextStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_contextConnectionNode?.Tag is not ConnectionProfile profile) return;
            if (_contextConnectionNode.Nodes.Count > 0) return;
            var credentials = _store.GetCredentials(profile.Id);
            if (credentials == null)
            {
                MessageBox.Show(this, $"连接“{profile.Name}”缺少凭据，请删除后重新添加。", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            using var dialog = new EditConnectionDialog(profile, credentials);
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            _store.Update(dialog.Profile!, dialog.Credentials!);
            _contextConnectionNode.Text = dialog.Profile!.Name;
            _contextConnectionNode.Tag = dialog.Profile!;
        }

        private void fileListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = _currentBucket != null && e.Data?.GetDataPresent(DataFormats.FileDrop) == true
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void fileListView_DragDrop(object sender, DragEventArgs e)
        {
            if (_currentBucket == null) return;
            if (e.Data?.GetData(DataFormats.FileDrop) is not string[] paths) return;
            var files = paths.Where(p => !Directory.Exists(p)).ToList();
            var skipped = paths.Length - files.Count;
            EnqueueUploads(files);
            if (skipped > 0)
                mainStripStatusLabel.Text = $"已跳过 {skipped} 个文件夹（拖拽仅支持上传文件）";
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        }

        private void newConnectionToolStripMenuItem_Click(object sender, EventArgs e) => AddConnection();

        private void newConnectionToolStripButton_Click(object sender, EventArgs e) => AddConnection();

        private void AddConnection()
        {
            using var dialog = new AddConnectionDialog();
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
                    e.Node.Nodes.Add(new TreeNode(bucket.BucketName, BucketImageIndex, BucketImageIndex)
                    { Tag = bucket.BucketName });
                e.Node.Expand();
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
            if (fileListView.SelectedItems[0].Tag is string folderPrefix)
                await LoadObjectsAsync(folderPrefix, null);
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
                fileStatusToolStripStatusLabel.Text = $"共 {fileListView.Items.Count} 项"
                    + (response.IsTruncated == true ? "（结果过多，仅显示前1000项）" : "");
            }
            catch (Exception ex)
            {
                if (version != _loadVersion) return;
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void uploadToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void downloadToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void selectAlltoolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void renameToolStripButton_Click(object sender, EventArgs e)
        {

        }
    }
}

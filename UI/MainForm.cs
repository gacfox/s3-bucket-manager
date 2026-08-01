using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gacfox.S3BucketManager.Models;
using Gacfox.S3BucketManager.Services;

namespace Gacfox.S3BucketManager.UI
{
    public partial class MainForm : Form
    {
        private const int ConnectionImageIndex = 0;
        private const int BucketImageIndex = 1;

        private ConnectionStore _store = null!;
        private readonly HashSet<Guid> _loadingConnections = new();

        public MainForm()
        {
            InitializeComponent();
            var imageList = new ImageList();
            imageList.Images.Add(Properties.Resources.database_yellow);
            imageList.Images.Add(Properties.Resources.package_white);
            bucketTreeView.ImageList = imageList;
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
                foreach (var bucket in response.Buckets.OrderBy(b => b.BucketName))
                    e.Node.Nodes.Add(new TreeNode(bucket.BucketName, BucketImageIndex, BucketImageIndex)
                    { Tag = bucket.BucketName });
                e.Node.Expand();
                mainStripStatusLabel.Text = $"{profile.Name}：共 {response.Buckets.Count} 个存储桶";
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

        private void useLargeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void useSmallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void useListToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void useDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void refreshToolStripButton_Click(object sender, EventArgs e)
        {

        }
    }
}

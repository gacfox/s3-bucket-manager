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
    public partial class EditConnectionDialog : Form
    {
        public ConnectionProfile? Profile { get; private set; }
        public ConnectionCredentials? Credentials { get; private set; }

        private readonly Guid? _editingId;

        public EditConnectionDialog()
        {
            InitializeComponent();
            useSecureSslCheckBox.Checked = true;
        }

        public EditConnectionDialog(ConnectionProfile profile, ConnectionCredentials credentials) : this()
        {
            _editingId = profile.Id;
            Text = "编辑连接";
            nameTextBox.Text = profile.Name;
            apiEndpointTextBox.Text = profile.Endpoint;
            useSecureSslCheckBox.Checked = profile.UseSsl;
            akTextBox.Text = credentials.AccessKey;
            skTextBox.Text = credentials.SecretKey;
        }

        private async void testConnectionButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            UseWaitCursor = true;
            testConnectionButton.Enabled = confirmButton.Enabled = cancelButton.Enabled = false;
            try
            {
                using var client = S3ClientFactory.Create(BuildProfile(), BuildCredentials());
                var response = await client.ListBucketsAsync();
                MessageBox.Show(this, $"连接成功，共 {response.Buckets?.Count ?? 0} 个存储桶。", "测试连接",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"连接失败：{ex.Message}", "测试连接",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                testConnectionButton.Enabled = confirmButton.Enabled = cancelButton.Enabled = true;
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            Profile = BuildProfile();
            Credentials = BuildCredentials();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateInput()
        {
            var invalid = new (TextBox Box, string Label)[]
            {
                (nameTextBox, "名称"),
                (apiEndpointTextBox, "API端点"),
                (akTextBox, "Access Key ID"),
                (skTextBox, "Secret Key")
            }.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Box.Text));
            if (invalid.Box != null)
            {
                MessageBox.Show(this, $"请填写{invalid.Label}。", "输入不完整",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                invalid.Box.Focus();
                return false;
            }
            return true;
        }

        private ConnectionProfile BuildProfile() => new()
        {
            Id = _editingId ?? Guid.NewGuid(),
            Name = nameTextBox.Text.Trim(),
            Endpoint = apiEndpointTextBox.Text.Trim(),
            UseSsl = useSecureSslCheckBox.Checked
        };

        private ConnectionCredentials BuildCredentials() => new()
        {
            AccessKey = akTextBox.Text.Trim(),
            SecretKey = skTextBox.Text.Trim()
        };
    }
}

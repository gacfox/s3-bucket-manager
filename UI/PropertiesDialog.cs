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

namespace Gacfox.S3BucketManager.UI
{
    public partial class PropertiesDialog : Form
    {
        private readonly AppSettings _settings;

        public PropertiesDialog(AppSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            uploadMaxConcurrencyNumericUpDown.Value = settings.UploadConcurrency;
            downloadMaxConcurrencyNumericUpDown.Value = settings.DownloadConcurrency;
            linkExpireNumericUpDown.Value = settings.LinkExpirationSeconds;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            _settings.UploadConcurrency = (int)uploadMaxConcurrencyNumericUpDown.Value;
            _settings.DownloadConcurrency = (int)downloadMaxConcurrencyNumericUpDown.Value;
            _settings.LinkExpirationSeconds = (int)linkExpireNumericUpDown.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

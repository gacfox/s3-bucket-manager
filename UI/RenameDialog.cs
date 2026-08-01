using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gacfox.S3BucketManager.UI
{
    public partial class RenameDialog : Form
    {
        public string NewName => inputTextBox.Text.Trim();

        public RenameDialog(string currentName)
        {
            InitializeComponent();
            inputTextBox.Text = currentName;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            inputTextBox.SelectAll();
            inputTextBox.Focus();
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
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

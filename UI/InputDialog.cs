namespace Gacfox.S3BucketManager.UI
{
    public static class InputDialog
    {
        public static string? Show(IWin32Window owner, string title, string prompt, string initialValue)
        {
            using var form = new Form
            {
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent,
                ClientSize = new Size(360, 110)
            };
            var label = new Label { Text = prompt, Left = 12, Top = 12, Width = 336 };
            var textBox = new TextBox { Left = 12, Top = 34, Width = 336, Text = initialValue };
            var okButton = new Button { Text = "确定", DialogResult = DialogResult.OK, Left = 188, Top = 70, Width = 75 };
            var cancelButton = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Left = 273, Top = 70, Width = 75 };
            form.Controls.AddRange(new Control[] { label, textBox, okButton, cancelButton });
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;
            form.Shown += (s, e) => textBox.SelectAll();
            return form.ShowDialog(owner) == DialogResult.OK ? textBox.Text.Trim() : null;
        }
    }
}

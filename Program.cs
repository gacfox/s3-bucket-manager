using Gacfox.S3BucketManager.UI;

namespace Gacfox.S3BucketManager
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}

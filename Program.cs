using System.Diagnostics;

namespace SingSelector
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 这么做并不严谨，但Good Enough To Me
            using Mutex mutex = new(true, Process.GetCurrentProcess().ProcessName, out bool createdNew);
            if (createdNew) // 成功创建了互斥体，即没有已经运行的进程
            {
                Application.EnableVisualStyles();
                ApplicationConfiguration.Initialize();
                Application.Run(new MainPage());
            }
            else
            {
                MessageBox.Show("请勿重复运行");
            }
        }
    }
}
using System.IO;
using System.Diagnostics;
using System.Security.Principal;

namespace SingSelector
{
    public partial class MainPage : Form
    {
        public MainPage()
        {
            InitializeComponent();
        }

        bool isSingOn = false;

        readonly string CurrentPath = Environment.CurrentDirectory + "\\";

        readonly Process SingProc = new();

        private void MainPage_Load(object sender, EventArgs e)
        {
            // 判断管理员权限
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("请以管理员身份运行");
                Environment.Exit(0);
            }

            // 判断有无singbox.exe
            if (!File.Exists(CurrentPath + "sing-box.exe"))
            {
                MessageBox.Show("未检测到sing-box.exe");
                Environment.Exit(0);
            }

            // 此处的文件带路径
            string[] LisFiles = Directory.GetFiles(CurrentPath + "Config\\");
            for (int i = 0; i < LisFiles.Length; i++)
            {
                if (LisFiles[i].EndsWith(".json"))
                {
                    this.ComboBox_Selector.Items.Add(LisFiles[i].Split('\\')[^1]);
                }
            }
            this.ComboBox_Selector.SelectedItem = ComboBox_Selector.Items[0];

            SingProc.StartInfo.WorkingDirectory = CurrentPath;
            SingProc.StartInfo.FileName = "sing-box.exe";
            SingProc.StartInfo.Verb = "runas";

            SingProc.StartInfo.CreateNoWindow = true;
            SingProc.StartInfo.UseShellExecute = false;

            SingProc.StartInfo.RedirectStandardError = true;
            SingProc.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            SingProc.ErrorDataReceived += (sender, e) => this.RichTextBox_Log.AppendText(e.Data + "\n");

            SingProc.EnableRaisingEvents = true;
            SingProc.Exited += (sender, e) =>
            {
                isSingOn = false;
                SingProc.CancelErrorRead();
                this.Button_Switch.Text = "启动";
            };
        }

        #region 按钮点击事件
        private void Button_Switch_Click(object sender, EventArgs e)
        {
            if (isSingOn)
            {
                SingProc.Kill();
            }
            else
            {
                isSingOn = true;

                this.Button_Switch.Text = "停止";
                this.RichTextBox_Log.Clear();
                this.RichTextBox_Log.AppendText("启动: " + this.ComboBox_Selector.SelectedItem + "\n");

                SingProc.StartInfo.Arguments = "run --disable-color -c .\\Config\\" + this.ComboBox_Selector.SelectedItem;
                SingProc.Start();
                SingProc.BeginErrorReadLine();
            }
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            this.ComboBox_Selector.Items.Clear();

            // 注意这里和初始化时的路径不同
            string[] LisFiles = Directory.GetFiles(CurrentPath + "Config\\");
            for (int i = 0; i < LisFiles.Length; i++)
            {
                if (LisFiles[i].EndsWith(".json"))
                {
                    this.ComboBox_Selector.Items.Add(LisFiles[i].Split('\\')[^1]);
                }
            }
            this.ComboBox_Selector.SelectedItem = ComboBox_Selector.Items[0];
        }

        private void Button_EditProfile_Click(object sender, EventArgs e)
        {
            Process p = new();
            p.StartInfo.FileName = CurrentPath + "Config\\" + this.ComboBox_Selector.SelectedItem;
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }
        #endregion

        #region 缩小至托盘与展开
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void TrayMenu_Exit_Click(object sender, EventArgs e)
        {
            foreach (var sp in Process.GetProcessesByName("sing-box")) sp.Kill();
            Environment.Exit(0);
        }

        private void TrayMenu_MainPage_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
        #endregion

        // 文本框自动滚动
        private void RichTextBox_Log_TextChanged(object sender, EventArgs e)
        {
            RichTextBox_Log.SelectionStart = RichTextBox_Log.Text.Length;
            RichTextBox_Log.ScrollToCaret();
        }

        // 退出时杀死SingBox
        private void MainPage_Closing(object sender, EventArgs e)
        {
            if (isSingOn) SingProc.Kill();
        }

    }
}

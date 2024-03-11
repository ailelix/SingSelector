using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Collections;

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

            // 参见下文
            Update_Profiles();

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
                this.TrayMenu_Switch.Text = "启动";
            };
        }


        #region 按钮点击事件
        private void Button_Switch_Click(object sender, EventArgs e)
        {
            if (isSingOn) SingProc.Kill();
            else TurnOn_SingBox();
        }

        private void TrayMenu_Switch_Click(object sender, EventArgs e)
        {
            if (isSingOn) SingProc.Kill();
            else TurnOn_SingBox();
        }

        private void Button_Refresh_Click(object sender, EventArgs e)
        {
            if (isSingOn) SingProc.Kill();
            Update_Profiles();
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
                // 非常的银性
                if (isSingOn)
                {
                    e.Cancel = true;
                    this.Hide();
                }
                else
                {
                    Environment.Exit(0);
                }
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


        #region 复用的一些函数

        // 切换启动&停止
        private void TurnOn_SingBox()
        {
            isSingOn = true;

            // 所有需要变化文字的按钮
            this.Button_Switch.Text = "停止";
            this.TrayMenu_Switch.Text = "停止";

            this.RichTextBox_Log.Clear();
            this.RichTextBox_Log.AppendText("启动: " + this.ComboBox_Selector.SelectedItem + "\n");

            SingProc.StartInfo.Arguments = "run --disable-color -c .\\Config\\" + this.ComboBox_Selector.SelectedItem + ".json";
            SingProc.Start();
            SingProc.BeginErrorReadLine();
        }

        // 刷新配置文件列表
        private void Update_Profiles()
        {
            List<string> profiles = [];
            this.ComboBox_Selector.Items.Clear();
            this.TrayMenu_ChangeProfile.DropDownItems.Clear();

            // LisFiles中的文件string带路径
            string[] LisFiles = Directory.GetFiles(CurrentPath + "Config\\");
            for (int i = 0; i < LisFiles.Length; i++)
            {
                if (LisFiles[i].EndsWith(".json"))
                {
                    // 去除路径和后缀
                    profiles.Add(LisFiles[i].Split('\\')[^1].Split('.')[0]);
                }
            }

            // 这里需要检测至少有一个配置文件
            if (profiles.Count == 0)
            {
                MessageBox.Show("未检测到配置文件，请先添加配置文件");

                this.Button_EditProfile.Enabled = false;
                this.Button_Switch.Enabled = false;
                this.TrayMenu_ChangeProfile.Enabled = false;
                this.TrayMenu_Switch.Enabled = false;
            }
            else
            {
                this.Button_EditProfile.Enabled = true;
                this.Button_Switch.Enabled = true;
                this.TrayMenu_ChangeProfile.Enabled = true;
                this.TrayMenu_Switch.Enabled = true;

                foreach (string item in profiles)
                {
                    this.ComboBox_Selector.Items.Add(item);
                    this.TrayMenu_ChangeProfile.DropDownItems.Add(item);
                }
                this.ComboBox_Selector.SelectedItem = ComboBox_Selector.Items[0];
            }
        }

        // 从TrayMenu中切换配置时，需要一并修改ComboBox的SelectedItem
        private void Update_TrayMenu_Selection(object sender, ToolStripItemClickedEventArgs e)
        {
            // 为什么这里ClickedItem可能为null啊
            if (e.ClickedItem != null) this.ComboBox_Selector.SelectedItem = e.ClickedItem.Text;

            // 如果已经启动就重启
            if (isSingOn)
            {
                // 小坑，需要先等待Exited的事件执行完再启动
                SingProc.Kill();
                Task.Run(() => {
                    Thread.Sleep(500);
                    TurnOn_SingBox();
                });
            }
        }

        // ComboBox切换配置时无需考虑其它
        private void Update_ComboBox_Selection(object sender, EventArgs e)
        {
            if (isSingOn)
            {
                SingProc.Kill();
                Task.Run(() => {
                    Thread.Sleep(500);
                    TurnOn_SingBox();
                });
            }
        }

        #endregion
    }
}

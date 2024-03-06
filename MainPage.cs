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

        public string CurrentPath = "";

        public string[] Profiles = new string[100];

        public Process SingProc = new();

        public bool isSingOn = false;

        private void MainPage_Load(object sender, EventArgs e)
        {
            // 判断是否正在运行
            Process[] process = Process.GetProcesses();
            foreach (Process p in process)
            {
                if (p.ProcessName == "SingSelector" && p.Id != Process.GetCurrentProcess().Id)
                {
                    MessageBox.Show("SingSelector已运行");
                    System.Environment.Exit(0);
                }
            }
            // 判断管理员权限
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("请以管理员身份运行");
                System.Environment.Exit(0);
            }
            // 判断有无singbox.exe
            CurrentPath = System.Environment.CurrentDirectory + "\\";
            if (!File.Exists(CurrentPath + "sing-box.exe"))
            {
                MessageBox.Show("未检测到sing-box.exe");
                System.Environment.Exit(0);
            }
            // 此处的文件带路径
            string[] LisFiles = Directory.GetFiles(CurrentPath);
            for (int i = 0, j = 0; i < LisFiles.Length; i++)
            {
                // 没有爆数组的检测 Lmao
                if (LisFiles[i].EndsWith(".json"))
                {
                    string profile = LisFiles[i].Split('\\')[^1];
                    Profiles[j] = profile;
                    j++;
                    this.ComboBox_Selector.Items.Add(profile);
                }
            }
            // 判断有无至少一个json配置
            if (!Profiles[0].EndsWith(".json"))
            {
                MessageBox.Show("请至少添加一个配置");
                System.Environment.Exit(0);
            }
            // 都好了，初始化
            this.ComboBox_Selector.SelectedItem = Profiles[0];

            SingProc.StartInfo.WorkingDirectory = CurrentPath;
            SingProc.StartInfo.FileName = "sing-box.exe";
            SingProc.StartInfo.Arguments = "run --disable-color -c " + Profiles[0];
            SingProc.StartInfo.Verb = "runas";
            SingProc.StartInfo.CreateNoWindow = true;
            SingProc.StartInfo.UseShellExecute = false;
            SingProc.StartInfo.RedirectStandardOutput = true;
            SingProc.StartInfo.RedirectStandardError = true;
            SingProc.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            SingProc.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            SingProc.OutputDataReceived += (sender, e) => this.RichTextBox_Log.AppendText(e.Data + "\n");
            SingProc.ErrorDataReceived += (sender, e) => this.RichTextBox_Log.AppendText(e.Data + "\n");

        }

        private void MainPage_Closing(object sender, EventArgs e)
        {
            foreach (var sp in Process.GetProcessesByName("sing-box")) sp.Kill();
        }

        private void Button_Switch_Click(object sender, EventArgs e)
        {
            /* 以下四种情况
             * 正常启动：flag：on singbox：on
             * 启动失败：flag：on singbox：off
             * 正常关闭：flag：off singbox：off
             * 额外进程：flag：off singbox：on
             */
            if (isSingOn)
            // 如果已启动（无论是否成功），则停止
            {
                // 如果启动成功，则停止并改变flag
                if (!SingProc.HasExited)
                {
                    SingProc.CancelOutputRead();
                    SingProc.CancelErrorRead();
                    SingProc.Kill();
                }
                // 如果启动失败，则只改变flag
                this.Button_Switch.Text = "启动";
                isSingOn = false;
            }
            else
            // 如果未启动，则杀死可能的singbox进程后启动
            {
                foreach (var sp in Process.GetProcessesByName("sing-box")) sp.Kill();
                try
                {
                    SingProc.CancelOutputRead();
                    SingProc.CancelErrorRead();
                }
                catch { }
                /*
                Thread SingProc_ReadLog = new(() =>
                {
                    while (SingProc.HasExited == false)
                    {
                        var line = SingProc.StandardOutput.ReadLine();
                        //var err = SingProc.StandardError.ReadLine();
                        if (line != null) this.RichTextBox_Log.AppendText(line + "\n");
                        //if (err != null) this.RichTextBox_Log.AppendText(err + "\n");
                    }
                });
                */
                this.RichTextBox_Log.Clear();
                SingProc.StartInfo.Arguments = "run --disable-color -c " + this.ComboBox_Selector.SelectedItem;
                this.RichTextBox_Log.AppendText("启动: " + SingProc.StartInfo.Arguments + "\n");
                SingProc.Start();
                // this.RichTextBox_Log.Text = SingProc.StandardOutput.ReadToEnd();
                // SingProc_ReadLog.Start();
                SingProc.BeginOutputReadLine();
                SingProc.BeginErrorReadLine();
                this.Button_Switch.Text = "停止";
                isSingOn = true;
            }
        }

        private void RichTextBox_Log_TextChanged(object sender, EventArgs e)
        {
            RichTextBox_Log.SelectionStart = RichTextBox_Log.Text.Length;
            RichTextBox_Log.ScrollToCaret();
        }

        private void TrayMenu_Exit_Click(object sender, EventArgs e)
        {
            foreach (var sp in Process.GetProcessesByName("sing-box")) sp.Kill();
            System.Environment.Exit(0);
        }

        private void TrayMenu_MainPage_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
    }
}

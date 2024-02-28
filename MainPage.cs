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
            // �ж��Ƿ���������
            Process[] process = Process.GetProcesses();
            foreach (Process p in process)
            {
                if (p.ProcessName == "SingSelector" && p.Id != Process.GetCurrentProcess().Id)
                {
                    MessageBox.Show("SingSelector������");
                    System.Environment.Exit(0);
                }
            }
            // �жϹ���ԱȨ��
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("���Թ���Ա�������");
                System.Environment.Exit(0);
            }
            // �ж�����singbox.exe
            CurrentPath = System.Environment.CurrentDirectory + "\\";
            if (!File.Exists(CurrentPath + "sing-box.exe"))
            {
                MessageBox.Show("δ��⵽sing-box.exe");
                System.Environment.Exit(0);
            }
            // �˴����ļ���·��
            string[] LisFiles = Directory.GetFiles(CurrentPath);
            for (int i = 0, j = 0; i < LisFiles.Length; i++)
            {
                // û�б�����ļ�� Lmao
                if (LisFiles[i].EndsWith(".json"))
                {
                    string profile = LisFiles[i].Split('\\')[^1];
                    Profiles[j] = profile;
                    j++;
                    this.ComboBox_Selector.Items.Add(profile);
                }
            }
            // �ж���������һ��json����
            if (!Profiles[0].EndsWith(".json"))
            {
                MessageBox.Show("���������һ������");
                System.Environment.Exit(0);
            }
            // �����ˣ���ʼ��
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
            /* �����������
             * ����������flag��on singbox��on
             * ����ʧ�ܣ�flag��on singbox��off
             * �����رգ�flag��off singbox��off
             * ������̣�flag��off singbox��on
             */
            if (isSingOn)
            // ����������������Ƿ�ɹ�������ֹͣ
            {
                // ��������ɹ�����ֹͣ���ı�flag
                if (!SingProc.HasExited)
                {
                    SingProc.CancelOutputRead();
                    SingProc.CancelErrorRead();
                    SingProc.Kill();
                }
                // �������ʧ�ܣ���ֻ�ı�flag
                this.Button_Switch.Text = "����";
                isSingOn = false;
            }
            else
            // ���δ��������ɱ�����ܵ�singbox���̺�����
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
                this.RichTextBox_Log.AppendText("����: " + SingProc.StartInfo.Arguments + "\n");
                SingProc.Start();
                // this.RichTextBox_Log.Text = SingProc.StandardOutput.ReadToEnd();
                // SingProc_ReadLog.Start();
                SingProc.BeginOutputReadLine();
                SingProc.BeginErrorReadLine();
                this.Button_Switch.Text = "ֹͣ";
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
        }
    }
}

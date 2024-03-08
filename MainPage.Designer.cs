namespace SingSelector
{
    partial class MainPage
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            Button_Switch = new Button();
            ComboBox_Selector = new ComboBox();
            RichTextBox_Log = new RichTextBox();
            TrayIcon = new NotifyIcon(components);
            TrayMenu = new ContextMenuStrip(components);
            TrayMenu_MainPage = new ToolStripMenuItem();
            TrayMenu_Exit = new ToolStripMenuItem();
            Button_EditProfile = new Button();
            Button_Refresh = new Button();
            TrayMenu.SuspendLayout();
            SuspendLayout();
            // 
            // Button_Switch
            // 
            Button_Switch.Font = new Font("Microsoft YaHei UI", 8F);
            Button_Switch.Location = new Point(681, 12);
            Button_Switch.Name = "Button_Switch";
            Button_Switch.Size = new Size(91, 25);
            Button_Switch.TabIndex = 0;
            Button_Switch.Text = "启动";
            Button_Switch.UseVisualStyleBackColor = true;
            Button_Switch.Click += Button_Switch_Click;
            // 
            // ComboBox_Selector
            // 
            ComboBox_Selector.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox_Selector.FormattingEnabled = true;
            ComboBox_Selector.Location = new Point(12, 12);
            ComboBox_Selector.Name = "ComboBox_Selector";
            ComboBox_Selector.Size = new Size(535, 25);
            ComboBox_Selector.TabIndex = 1;
            // 
            // RichTextBox_Log
            // 
            RichTextBox_Log.BackColor = SystemColors.ButtonHighlight;
            RichTextBox_Log.BorderStyle = BorderStyle.None;
            RichTextBox_Log.Font = new Font("Microsoft YaHei UI", 8F);
            RichTextBox_Log.Location = new Point(12, 43);
            RichTextBox_Log.Name = "RichTextBox_Log";
            RichTextBox_Log.ReadOnly = true;
            RichTextBox_Log.ScrollBars = RichTextBoxScrollBars.Vertical;
            RichTextBox_Log.Size = new Size(760, 706);
            RichTextBox_Log.TabIndex = 2;
            RichTextBox_Log.Text = "";
            RichTextBox_Log.TextChanged += RichTextBox_Log_TextChanged;
            // 
            // TrayIcon
            // 
            TrayIcon.ContextMenuStrip = TrayMenu;
            TrayIcon.Icon = (Icon)resources.GetObject("TrayIcon.Icon");
            TrayIcon.Text = "SingSelector";
            TrayIcon.Visible = true;
            TrayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;
            // 
            // TrayMenu
            // 
            TrayMenu.Items.AddRange(new ToolStripItem[] { TrayMenu_MainPage, TrayMenu_Exit });
            TrayMenu.Name = "TrayMenu";
            TrayMenu.Size = new Size(149, 48);
            // 
            // TrayMenu_MainPage
            // 
            TrayMenu_MainPage.Name = "TrayMenu_MainPage";
            TrayMenu_MainPage.Size = new Size(148, 22);
            TrayMenu_MainPage.Text = "SingSelector";
            TrayMenu_MainPage.Click += TrayMenu_MainPage_Click;
            // 
            // TrayMenu_Exit
            // 
            TrayMenu_Exit.Name = "TrayMenu_Exit";
            TrayMenu_Exit.Size = new Size(148, 22);
            TrayMenu_Exit.Text = "退出";
            TrayMenu_Exit.Click += TrayMenu_Exit_Click;
            // 
            // Button_EditProfile
            // 
            Button_EditProfile.Font = new Font("Microsoft YaHei UI", 8F);
            Button_EditProfile.Location = new Point(584, 12);
            Button_EditProfile.Name = "Button_EditProfile";
            Button_EditProfile.Size = new Size(91, 25);
            Button_EditProfile.TabIndex = 3;
            Button_EditProfile.Text = "编辑";
            Button_EditProfile.UseVisualStyleBackColor = true;
            Button_EditProfile.Click += Button_EditProfile_Click;
            // 
            // Button_Refresh
            // 
            Button_Refresh.BackgroundImage = Properties.Resources.Icon_UpdateProfiles;
            Button_Refresh.BackgroundImageLayout = ImageLayout.Zoom;
            Button_Refresh.Location = new Point(553, 12);
            Button_Refresh.Name = "Button_Refresh";
            Button_Refresh.Size = new Size(25, 25);
            Button_Refresh.TabIndex = 4;
            Button_Refresh.UseVisualStyleBackColor = true;
            Button_Refresh.Click += Button_Refresh_Click;
            // 
            // MainPage
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(784, 761);
            Controls.Add(Button_Refresh);
            Controls.Add(Button_EditProfile);
            Controls.Add(RichTextBox_Log);
            Controls.Add(ComboBox_Selector);
            Controls.Add(Button_Switch);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainPage";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SingSelector";
            FormClosing += MainPage_Closing;
            Load += MainPage_Load;
            TrayMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button Button_Switch;
        private ComboBox ComboBox_Selector;
        private RichTextBox RichTextBox_Log;
        private NotifyIcon TrayIcon;
        private ContextMenuStrip TrayMenu;
        private ToolStripMenuItem TrayMenu_MainPage;
        private ToolStripMenuItem TrayMenu_Exit;
        private Button Button_EditProfile;
        private Button Button_Refresh;
    }
}

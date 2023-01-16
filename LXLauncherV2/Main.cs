﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LXLauncher
{
    public partial class Main : Form
    {
        int mov, movX, movY;
        readonly Functions functions = new Functions();

        public Main()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
        string discordserver;
        string MenuBanner1;
        string MenuIconFont;
        string MenuTitleFont;
        private void Main_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> Data = functions.GetData();

            label10.Text = Data["Status"];
            label14.Text = Data["MenuVersion"];
            label16.Text = Data["Discord"];
            label15.Text = Data["GameVersion"];
            discordserver = Data["DiscordServer"];
            MenuBanner1 = Data["MenuBanner"];
            MenuIconFont = Data["IconFont"];
            MenuTitleFont = Data["TitleFont"];

            label5.Text = "V" + Data["MenuVersion"];

            WebClient webc = new WebClient();
            string changelog = webc.DownloadString("https://pastebin.com/raw/yLKKhF2s");
            richTextBox1.Text = changelog;

            switch (label10.Text)
            {
                case "ONLINE": 
                    label10.ForeColor = Color.Lime; break;

                case "UPDATING": 
                    label10.ForeColor = Color.Yellow; break;

                default: 
                    label10.ForeColor = Color.Red; break;
            }

            AnimateWindow(Handle, 500, AnimateWindowFlags.AW_BLEND);
            timer1.Start();
            label7.Text = DateTime.Now.ToLongTimeString();
            label8.Text = DateTime.Now.ToString("d. MMM yyyy");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            AnimateWindow(Handle, 500, AnimateWindowFlags.AW_BLEND | AnimateWindowFlags.AW_HIDE);
            Application.Exit();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            panel2.BackColor = Color.FromArgb(190, 14, 14, 14);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label7.Text = DateTime.Now.ToLongTimeString();
            label8.Text = DateTime.Now.ToString("d. MMM yyyy");
        }

        private void Main_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1) SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mov == 1) SetDesktopLocation(MousePosition.X - movX, MousePosition.Y - movY);
        }

        private void label_mousedown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mov = 1;
            movX = e.X;
            movY = e.Y;
        }

        private void Main_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mov = 0;
        }

        // Install
        private async void button2_Click(object sender, EventArgs e)
        {
            string MainPath = "C:/LightningFiles";
            string DllFolder = MainPath + "/Dll";
            string HeaderPath = MainPath + "/Headers";
            string FontPath = MainPath + "/Fonts";

            try
            {
                Directory.CreateDirectory(MainPath);
                Directory.CreateDirectory(DllFolder);
                Directory.CreateDirectory(FontPath);
                Directory.CreateDirectory(HeaderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create directories\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            await DownloadDLL(DllFolder + "/LX.dll");

            try
            {
                //connected to the pastebin
                WebClient wc = new WebClient();
                wc.DownloadFile(MenuBanner1, HeaderPath + "/Best.gif");
                wc.DownloadFile(MenuIconFont, FontPath + "/IconFont.ttf"); 
                wc.DownloadFile(MenuTitleFont, FontPath + "/TitleFont.ttf");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download fonts/header\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            button2.BackColor = Color.FromArgb(76, 146, 186);
            button2.ForeColor = Color.FromArgb(255, 255, 255);
            button2.Text = "Installed";
            InformationLabel.Text = "Lightning X Files Installed!";

            await Task.Delay(5000);

            button2.BackColor = Color.FromArgb(255, 255, 255);
            button2.ForeColor = Color.FromArgb(0, 0, 0);
            button2.Text = "Install";
        }

        // Join Discord
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(discordserver);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Failed to open link\nLink: " + discordserver + "\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Download DLL
        async Task<bool> DownloadDLL(string Path)
        {
            WebClient wc = new WebClient();
            wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            Dictionary<string, string> Data = functions.GetData();

            bool b1 = Data.TryGetValue("Status", out string MenuOnline);
            bool b2 = true; Data.TryGetValue("dll", out string DllLink);

            if (!b1 || !b2)
                MessageBox.Show("An error occured, please try again", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);


            if (File.Exists(Path)) File.Delete(Path);

            if (MenuOnline == "false")
            {
                MessageBox.Show("Menu is currently offline", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!DllLink.StartsWith("https://"))
            {
                MessageBox.Show("The menu is not available at the moment", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                await wc.DownloadFileTaskAsync(new Uri(DllLink), Path);
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Failed to download dll\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            if (File.Exists(Path))
            {
                return true;
            }

            MessageBox.Show("Failed to download latest dll", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        // Inject
        private async void button4_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> Data = functions.GetData();

            if (Data["Status"] != "ONLINE")
            {
                MessageBox.Show("The menu is currently offline", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int pid = functions.GetPid();
            IntPtr intPtr = OpenProcess(1082U, false, pid);
            if (intPtr == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open GTA process\nMake sure you have GTA started and you ran this with admin rights", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");
            if (procAddress == IntPtr.Zero)
            {
                MessageBox.Show("Couldnt find LoadLibraryW", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string Path = "C:/LightningFiles/Dll";
            string dllpath = Path + "/LX.dll";

            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            if (!File.Exists(dllpath))
            {
                MessageBox.Show("Files do not exist. Press install before injecting!", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(dllpath);
                IntPtr intPtr2 = VirtualAllocEx(intPtr, (IntPtr)null, (IntPtr)bytes.Length, 12288U, 64U);

                if (intPtr2 == IntPtr.Zero)
                    MessageBox.Show("Failed at VirtualAllocEx", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);

                else if (WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0)
                    MessageBox.Show("Failed at WriteProcessMemory", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);

                else if (CreateRemoteThread(intPtr, IntPtr.Zero, 0, procAddress, intPtr2, 0, IntPtr.Zero) == IntPtr.Zero)
                    MessageBox.Show("Failed at CreateRemoteThread", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);

                else
                {
                    button4.BackColor = Color.FromArgb(76, 146, 186); //darkercolor
                    button4.ForeColor = Color.FromArgb(255, 255, 255);
                    button4.Text = "Injected";
                    InformationLabel.Text = "Lightning X Injected!";

                    await Task.Delay(5000);

                    button3.BackColor = Color.FromArgb(255, 255, 255); //darkercolor
                    button3.ForeColor = Color.FromArgb(0, 0, 0);
                    button3.Text = "Inject";
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("The injection was most likely blocked by your anti virus\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to inject\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CloseHandle(intPtr);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string Path = @"C:\LightningFiles";
            if (Directory.Exists(Path))
            {
                try
                {
                    functions.DeleteDirectory(Path, true);

                    button3.BackColor = Color.FromArgb(76, 146, 186);
                    button3.ForeColor = Color.FromArgb(255, 255, 255);
                    button3.Text = "Uninstalled";
                    InformationLabel.Text = "Lightning X Files Uninstalled!";

                    await Task.Delay(5000);

                    button3.BackColor = Color.FromArgb(255, 255, 255);
                    button3.ForeColor = Color.FromArgb(0, 0, 0);
                    button3.Text = "Uninstall";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to uninstall\nError: " + ex.Message, "LXLauncherV2");
                }
            }
            else
            {
                MessageBox.Show("Files do not exist!", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Change Lob
        private void button5_Click(object sender, EventArgs e)
        {
            label2.Text = "Change Log";
            button2.Hide();
            button3.Hide();
            button4.Hide();
            button5.Hide();
            button6.Hide();
            label3.Hide();
            button8.Show();
            richTextBox1.Show();
        }

        // Main Menu
        private void button8_Click(object sender, EventArgs e)
        {
            label2.Text = "Main Menu";
            richTextBox1.Hide();
            button8.Hide();
            label3.Show();
            button6.Show();
            button5.Show();
            button4.Show();
            button3.Show();
            button2.Show();
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            label3.Text = "1/5";
            InformationLabel.Text = "Inject LX into GTA5";
        }

        private void button2_MouseHover(object sender, EventArgs e)
        {
            label3.Text = "2/5";
            InformationLabel.Text = "Download/Install LX Files";
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            label3.Text = "3/5";
            InformationLabel.Text = "Uninstall LX Files";
        }

        private void button5_MouseHover(object sender, EventArgs e)
        {
            label3.Text = "4/5";
            InformationLabel.Text = "Display Change Log";
        }

        private void button6_MouseHover(object sender, EventArgs e)
        {
            label3.Text = "5/5";
            InformationLabel.Text = "Open LX discord server";
        }



        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        [DllImport("user32.dll")]
        static extern bool AnimateWindow(IntPtr hWnd, int time, AnimateWindowFlags flags);
        enum AnimateWindowFlags
        {
            AW_HOR_POSITIVE = 0x00000001,
            AW_HOR_NEGATIVE = 0x00000002,
            AW_VER_POSITIVE = 0x00000004,
            AW_VER_NEGATIVE = 0x00000008,
            AW_CENTER = 0x00000010,
            AW_HIDE = 0x00010000,
            AW_ACTIVATE = 0x00020000,
            AW_SLIDE = 0x00040000,
            AW_BLEND = 0x00080000
        }
    }
}
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using HtmlAgilityPack;
using System.Globalization;
using Newtonsoft.Json;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Linq;

namespace LXLauncher
{
    public partial class Main : Form
    {
        //top panel
        public Color PanelColor1 { get; set; } = Color.White;

        public Color PanelColor2 { get; set; } = Color.Black;

        //Colors Tab 1
        public Color Color1 { get; set; } = Color.LightSkyBlue;

        public Color Color2 { get; set; } = Color.Black;

        //Color Tab 2
        public Color Color3 { get; set; } = Color.Black;

        public Color Color4 { get; set; } = Color.LightSeaGreen;

        //Color Tab 3
        public Color Color5 { get; set; } = Color.Red;

        public Color Color6 { get; set; } = Color.Black;

        //Color Tab 4
        public Color Color7 { get; set; } = Color.Purple;

        public Color Color8 { get; set; } = Color.Black;

        //other
        private int mov, movX, movY;

        private readonly Functions functions = new Functions();

        private readonly WebClient WebClient = new WebClient
        {
            CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        };

        public Main()
        {
            InitializeComponent();
            CreateLabelAndFetchNumber();
        }

        private string discordserver;

        private static HttpClient httpClient = new HttpClient();

        private async Task<string> FetchNumberFromWebsite(string url, string pathq)
        {
            try
            {
                string htmlContent = await httpClient.GetStringAsync(url);

                HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
                htmlDocument.LoadHtml(htmlContent);

                HtmlNode numberNode = htmlDocument.DocumentNode.SelectSingleNode(pathq);

                if (numberNode != null)
                {
                    return numberNode.InnerText.Trim();
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request-related errors
                Console.WriteLine("HTTP request error: " + ex.Message);
            }
            catch (TaskCanceledException ex)
            {
                // Handle task cancellation (timeout) errors
                Console.WriteLine("Task cancelled error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("Error while fetching data: " + ex.Message);
            }

            return "X";
        }

        private async void CreateLabelAndFetchNumber()
        {
            string GTAONLINENUMBER = await FetchNumberFromWebsite("https://gta.fandom.com/wiki/Grand_Theft_Auto_V/Title_Update_Notes", "//*[@id=\"mw-content-text\"]/div/table[1]/tbody/tr[6]/td[2]");
            label15.Text = GTAONLINENUMBER;
            string GTAONLINEUPDATE = await FetchNumberFromWebsite("https://gta.fandom.com/wiki/Grand_Theft_Auto_V/Title_Update_Notes", "//*[@id=\"mw-content-text\"]/div/table[1]/tbody/tr[6]/td[3]/text()");
            label18.Text = GTAONLINEUPDATE;
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            SetButtonLabelText();

            await InitializeDataAndUIAsync();

            timer1.Start();
        }

        private void SetButtonLabelText()
        {
            int pid = functions.GetPid();
            button4.Text = (pid == -1) ? "Launch GTA" : "Inject";
        }

        private async Task InitializeDataAndUIAsync()
        {
            Dictionary<string, string> data = functions.GetData();

            label10.Text = data["Status"];
            label14.Text = data["MenuVersion"];
            discordserver = data["DiscordServer"];
            label19.Text = data["LastUpdate"];

            string richTextBox1Content = await WebClient.DownloadStringTaskAsync("https://pastebin.com/raw/yLKKhF2s");
            richTextBox1.Text = richTextBox1Content.Replace("â€¢", "•");

            string richTextBox2Content = await WebClient.DownloadStringTaskAsync("https://pastebin.com/raw/DhgCw37u");
            richTextBox2.Text = richTextBox2Content.Replace("â€¢", "•");

            switch (label10.Text)
            {
                case "ONLINE":
                    label10.ForeColor = Color.Lime; break;
                case "UPDATING":
                    label10.ForeColor = Color.Yellow; break;
                default:
                    label10.ForeColor = Color.Red; break;
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color3, Color4, 115F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int pid = functions.GetPid();
            button4.Text = (pid == -1) ? "Launch GTA" : "Inject";
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

        // Join Discord
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (Uri.TryCreate(discordserver, UriKind.Absolute, out Uri uri))
                {
                    Process.Start(uri.ToString());
                }
                else
                {
                    MessageBox.Show("Invalid Discord link: " + discordserver, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open link\nLink: " + discordserver + "\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Inject
        private void LaunchGTA()
        {
            switch (comboBox1.SelectedItem)
            {
                case "Epic Games":
                    Process.Start("com.epicgames.launcher://apps/9d2d0eb64d5c44529cece33fe2a46482?action=launch&silent=true");
                    break;

                case "Rockstar Games":
                    try
                    {
                        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Rockstar Games\\Launcher"))
                        {
                            string text = (string)(registryKey?.GetValue("InstallFolder"));
                            if (text != null)
                            {
                                Process.Start(text + "\\Launcher.exe", "-minmodeApp=gta5");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to launch GTA with Rockstar Games\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case "Steam":
                    Process.Start("steam://run/271590");
                    break;
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (button4.Text == "Launch GTA")
            {
                LaunchGTA();
                return;
            }

            Dictionary<string, string> data = functions.GetData();

            if (data["Status"] == "UPDATING")
            {
                MessageBox.Show("The menu is currently updating", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (data["Status"] == "OFFLINE")
            {
                MessageBox.Show("The menu is currently offline", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int pid = functions.GetPid();
            IntPtr intPtr = OpenProcess(1082U, false, pid);
            if (intPtr == IntPtr.Zero)
            {
                if (GetLastError() == 87 || pid == -1)
                {
                    MessageBox.Show("Failed to open GTA process\nMake sure you have GTA started", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show("Failed to open GTA process\nMake sure you have GTA started and you are running this with admin rights", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");
            if (procAddress == IntPtr.Zero)
            {
                MessageBox.Show("Couldn't find LoadLibraryW", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string dllpath = Path.Combine("C:/Program Files/LXV/Dll", "LX.dll");

            if (!Directory.Exists(Path.GetDirectoryName(dllpath))) Directory.CreateDirectory(Path.GetDirectoryName(dllpath));
            if (!File.Exists(dllpath))
            {
                MessageBox.Show("Files do not exist. Press install before injecting!", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(dllpath);
                IntPtr intPtr2 = VirtualAllocEx(intPtr, IntPtr.Zero, (IntPtr)bytes.Length, 12288U, 64U);

                if (intPtr2 == IntPtr.Zero)
                {
                    MessageBox.Show("Failed at VirtualAllocEx", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (WriteProcessMemory(intPtr, intPtr2, bytes, (uint)bytes.Length, 0) == 0)
                {
                    MessageBox.Show("Failed at WriteProcessMemory", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (CreateRemoteThread(intPtr, IntPtr.Zero, 0, procAddress, intPtr2, 0, IntPtr.Zero) == IntPtr.Zero)
                {
                    MessageBox.Show("Failed at CreateRemoteThread", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    button4.BackColor = Color.FromArgb(76, 146, 186);
                    button4.ForeColor = Color.FromArgb(255, 255, 255);
                    button4.Text = "Injected";
                    await Task.Delay(5000);
                    button4.BackColor = Color.FromArgb(255, 255, 255);
                    button4.ForeColor = Color.FromArgb(0, 0, 0);
                    button4.Text = "Inject";
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("The injection was most likely blocked by your antivirus\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to inject\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            CloseHandle(intPtr);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string lightningFilesPath = @"C:\LightningFiles";
            string lxvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LXV");

            if (Directory.Exists(lightningFilesPath) && Directory.Exists(lxvPath))
            {
                try
                {
                    functions.DeleteDirectory(lightningFilesPath, true);
                    functions.DeleteDirectory(lxvPath, true);

                    UpdateButtonAfterUninstall();
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

        private void UpdateButtonAfterUninstall()
        {
            button3.BackColor = Color.FromArgb(76, 146, 186);
            button3.ForeColor = Color.FromArgb(255, 255, 255);
            button3.Text = "Uninstalled";
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            // label3.Text = "1/6";
            // InformationLabel.Text = (button4.Text == "Inject" ? "Inject LX into GTA5" : "Start GTA");
        }

        private void button3_MouseHover(object sender, EventArgs e)
        {
            // label3.Text = "3/6";
            // InformationLabel.Text = "Uninstall LX Files";
        }

        private void button6_MouseHover(object sender, EventArgs e)
        {
            // label3.Text = "5/6";
            //InformationLabel.Text = "Open LX discord server";
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

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

        private void panel8_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color2, Color1, 115F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, PanelColor2, PanelColor1, 230F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color5, Color6, 125F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, Color7, Color8, 125F);
            e.Graphics.FillRectangle(brush, this.ClientRectangle);
            base.OnPaint(e);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox2.Cursor = Cursors.Hand;
        }

        private void panel9_Click(object sender, EventArgs e)
        {
            panel5.Hide();
            panel3.Show();
        }

        private void panel10_Click(object sender, EventArgs e)
        {
            panel3.Hide();
            panel5.Show();
        }

        private void label8_Click(object sender, EventArgs e)
        {
            label8.MouseClick += panel10_Click;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            pictureBox4.MouseClick += panel10_Click;
        }

        private void label7_MouseClick(object sender, MouseEventArgs e)
        {
            label7.MouseClick += panel9_Click;
        }

        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox3.MouseClick += panel9_Click;
        }
    }
}
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Linq;

namespace LXLauncher
{
    public partial class SplashScreen : Form
    {
        private readonly Functions functions = new Functions();

        public SplashScreen()
        {
            InitializeComponent();
            timer1.Start();
        }

        //defines
        private async Task<bool> DownloadDLL(string filePath)
        {
            Dictionary<string, string> data = functions.GetData();

            if (!data.TryGetValue("Status", out string menuOnline) || !data.TryGetValue("dll", out string dllLink))
            {
                throw new Exception("An error occurred while getting the menu data. Please try again.");
            }

            if (File.Exists(filePath))
                File.Delete(filePath);

            if (menuOnline == "false")
            {
                throw new Exception("The menu is currently offline.");
            }

            if (!Uri.TryCreate(dllLink, UriKind.Absolute, out Uri uri) || !uri.Scheme.Equals(Uri.UriSchemeHttps))
            {
                throw new Exception("The menu is not available at the moment.");
            }

            try
            {
                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(uri, filePath);
                }

                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download DLL.\nError: " + ex.Message, ex);
            }
        }

        private string WEB_URL = "https://pastebin.com/raw/ZS7fhCbY";
        private string LightningFiles = "C:\\LightningFiles";
        private string LXVPath = "C:\\Program Files\\LXV";
        private string VersionTXTPath = "C:\\Program Files\\LXV\\Version.txt";
        private string DllFolder = "C:\\Program Files\\LXV\\Dll";
        private string HeaderPath = "C:\\LightningFiles\\Headers";
        private string FontPath = "C:\\Program Files\\LXV\\Fonts";
        private string TranslationPath = "C:\\Program Files\\LXV\\Translations";

        //actions
        private void CreateTXTFile()
        {
            // Combine the directory path and the file name
            string filePath = Path.Combine(LXVPath, "Version.txt");

            // Download the web text only if the file doesn't exist
            if (!File.Exists(filePath))
            {
                // Ensure the directory exists
                Directory.CreateDirectory(LXVPath);

                // Download the web text
                string webText;
                using (WebClient client = new WebClient())
                {
                    webText = client.DownloadString(WEB_URL);
                }

                // Write the downloaded text to the file
                File.WriteAllText(filePath, webText);
            }
        }

        private void CheckForTXTVersion()
        {
            string webTextToComp;
            using (WebClient client = new WebClient())
            {
                webTextToComp = client.DownloadString(WEB_URL);
            }

            string fileTextToComp = File.Exists(VersionTXTPath) ? File.ReadAllText(VersionTXTPath) : string.Empty;

            if (webTextToComp == fileTextToComp)
            {
                fullLXFilesDownload();
            }
            else
            {
                MessageBox.Show("Outdated Lightning X Files Found!\nDownloading the latest version now!\nPlease wait...");

                // Delete the directories and their contents
                Directory.Delete(LXVPath, true);
                Directory.Delete(LightningFiles, true);

                // Recreate the necessary files
                CreateTXTFile();
                fullLXFilesDownload();
            }
        }

        private async void fullLXFilesDownload()
        {
            WebClient client = new WebClient();
            Dictionary<string, string> DATA_FOR_DOWNLOAD = functions.GetData();
            string[] languages = { "English", "Russian", "German", "French", "Italian", "Spanish", "Portugues", "Romanian" };

            string[] menuBanners = {
    "Best.gif", "Lighting_X.png", "LightningXRemake_1_1.gif",
    "LX_Banner.png", "GIF-220414_232057.gif", "LX_header_2.gif",
    "LX_4.gif", "LX_header.gif", "Lighting_X_JOJO.png", "LX_gif.gif"
};

            string[] fonts = {
    "TitleFont.ttf", "IconFont.ttf", "chinese_rocks_rg.ttf",
    "Font_Awesome.otf", "GemunuLibre-SemiBold.ttf", "fa-solid-900.ttf"
};

            string lightningFiles = LightningFiles;
            string lXVPath = LXVPath;
            string dllFile = DllFolder + "/LX.dll";
            string headerPath = HeaderPath;
            string fontPath = FontPath;

            if (!Directory.Exists(lightningFiles))
                Directory.CreateDirectory(lightningFiles);

            if (!Directory.Exists(lXVPath))
                Directory.CreateDirectory(lXVPath);

            if (!File.Exists(dllFile))
            {
                Directory.CreateDirectory(DllFolder);
                await DownloadDLL(dllFile);
            }

            if (!Directory.Exists(headerPath))
            {
                Directory.CreateDirectory(headerPath);
                foreach (string banner in menuBanners)
                {
                    string menuBannerUrl = DATA_FOR_DOWNLOAD["MenuBanner" + (Array.IndexOf(menuBanners, banner) + 1)];
                    client.DownloadFile(menuBannerUrl, headerPath + "/" + banner);
                }
            }

            if (!fonts.All(font => File.Exists(fontPath + "/" + font)))
            {
                Directory.CreateDirectory(fontPath);
                string[] fontNames = { "TitleFont", "IconFont", "Rd2Font", "FontAwesome", "GemunuLibreSemiBold", "FaSolid900" };

                for (int i = 0; i < fontNames.Length; i++)
                {
                    string fontUrl = DATA_FOR_DOWNLOAD[fontNames[i]];
                    client.DownloadFile(fontUrl, fontPath + "/" + fonts[i]);
                }
            }

            if (!Directory.Exists(TranslationPath))
                Directory.CreateDirectory(TranslationPath);

            bool allTranslationsExist = true;

            foreach (string language in languages)
            {
                string translationFile = $"{TranslationPath}/{language}.json";

                if (!File.Exists(translationFile))
                {
                    allTranslationsExist = false;
                    break;
                }
            }

            if (!allTranslationsExist)
            {
                foreach (string language in languages)
                {
                    string translationFile = $"{TranslationPath}/{language}.json";
                    string translationUrl = DATA_FOR_DOWNLOAD[$"{language}_Translation"];

                    if (!File.Exists(translationFile))
                    {
                        client.DownloadFile(translationUrl, translationFile);
                    }
                }
            }
        }

        private void CheckAndUpdateVersion()
        {
            try
            {
                if (!File.Exists(VersionTXTPath))
                {
                    Directory.CreateDirectory(LXVPath);
                    CreateTXTFile();
                    fullLXFilesDownload();
                    return; // Exit the method here if there's no Version.txt file
                }

                CheckForTXTVersion();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Version.txt file not found!\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Directory not found!\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while checking for update!\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SplashScreen_Load_1(object sender, EventArgs e)
        {
            CheckAndUpdateVersion();
        }

        private const int TargetPanelWidth = 362;

        private void ShowMainForm()
        {
            Main mainForm = new Main();
            mainForm.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            const int increaseStep = 2;
            panel2.Width += increaseStep;

            if (panel2.Width >= TargetPanelWidth)
            {
                timer1.Stop(); // Stop the timer when the progress bar has reached the maximum value
                ShowMainForm();
                this.Hide();
            }
        }
    }
}
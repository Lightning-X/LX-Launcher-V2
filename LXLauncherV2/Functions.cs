using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace LXLauncher
{
    internal class Functions
    {
        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> Data = new Dictionary<string, string>();

            try
            {
                WebClient wc = new WebClient();
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                string[] seperator = { ": ", "\n" };
                string[] TempData = wc.DownloadString("https://pastebin.com/raw/sYUdeNqv").Split(seperator, StringSplitOptions.RemoveEmptyEntries);


                for (int i = 0; i < TempData.Length; i += 2)
                {
                    if (TempData[i].Trim().StartsWith("//") || TempData[i].Trim().StartsWith("#") || string.IsNullOrEmpty(TempData[i].Trim()))
                    {
                        i--;
                        continue;
                    }
                    if (string.IsNullOrEmpty(TempData[i].Trim())) i--;

                    string Key = TempData[i].Trim();
                    string Value = TempData[i + 1].Trim();

                    Data.Add(Key, Value);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to receive data\nMake sure you have a internet connection\nError: " + ex.Message, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            return Data;
        }

        public int GetPid()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "GTA5") return p.Id;
            }

            return -1;
        }
    }
}

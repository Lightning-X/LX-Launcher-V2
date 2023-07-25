using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace LXLauncher
{
    internal class Functions
    {
        private readonly WebClient WebClient = new WebClient
        {
            CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)
        };

        public Dictionary<string, string> GetData()
        {
            Dictionary<string, string> Data = new Dictionary<string, string>();

            try
            {
                string[] seperator = { ": ", "\n" };
                string[] TempData = WebClient.DownloadString("https://pastebin.com/raw/sYUdeNqv").Split(seperator, StringSplitOptions.RemoveEmptyEntries);

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
                Application.Exit();
            }

            return Data;
        }

        public int GetPid()
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.Equals("GTA5", StringComparison.OrdinalIgnoreCase))
                {
                    return p.Id;
                }
            }

            return -1;
        }

        //private string Hash(string s, bool l = false)
        //{
        //    string r = "";

        //    try
        //    {
        //        byte[] d = Encoding.UTF8.GetBytes(s);

        //        using (SHA512 a = new SHA512Managed())
        //        {
        //            byte[] h = a.ComputeHash(d);
        //            r = BitConverter.ToString(h).Replace("-", "");
        //        }

        //        r = (l ? r.ToLowerInvariant() : r);
        //    }
        //    catch { }

        //    return r;
        //}

        public void DeleteDirectory(string path, bool recursive)
        {
            if (recursive)
            {
                foreach (var subdirectory in Directory.GetDirectories(path))
                {
                    DeleteDirectory(subdirectory, recursive);
                }
            }

            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (IOException ex)
                {
                    // Handle the exception or rethrow it if needed
                    throw ex;
                }
            }

            Directory.Delete(path);
        }

        //public void isBanned()
        //{
        //    try
        //    {
        //        bool banned = false;

        //        string[] seperator = { "\n" };
        //        string[] List = WebClient.DownloadString("https://pastebin.com/raw/ZkhR8n0A").Split(seperator, StringSplitOptions.RemoveEmptyEntries);
        //        string HWID = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
        //        string Hashed = Hash(HWID);

        //        foreach (string User in List)
        //        {
        //            if (User.StartsWith("//") || User.StartsWith("#")) continue;
        //            if (Hashed.Trim() == User.Trim()) banned = true;
        //        }

        //        if (banned)
        //        {
        //            int random = new Random().Next(0, 6);
        //            string msg = "";

        //            if (Directory.Exists("C:/LightingFiles")) DeleteDirectory("C:/LightningFiles", true);

        //            switch (random)
        //            {
        //                case 0: msg = "Imagine being banned ahhahahha"; break;
        //                case 1: msg = "Hmmm, seems like you are banned :p"; break;
        //                case 2: msg = "Looser got banned lmao"; break;
        //                case 3: msg = "Garbage is not allowed to use this menu"; break;
        //                case 4: msg = "Wahh wahh you are banned go cry to your mom"; break;
        //                case 5: msg = "You are banned from using this awesome menu"; break;
        //            }

        //            MessageBox.Show(msg, "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            Environment.Exit(0);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("An error occured", "LXLauncherV2", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        Environment.Exit(0);
        //    }
        //}

        //public void LogInfo()
        //{
        //}
    }
}
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Management;
using System.IO;
/*-----------------------------------------------------------------------
-- Company of Heroes: Eastern Front Steam Starter
--
-- (c) ARCH Ltd. - Archaic Entertainment Ltd. 2016
--
-- Programmers: Walentin 'Walki' L.
-----------------------------------------------------------------------
-- To-Do: Update images
-----------------------------------------------------------------------
INFO: 
-- Remember that this executable is located in the RelicCoH folder
-- This program will update our promotional images and texts
-- Then it will call Steam to start RelicCoH with the Eastern Front mod
-- After that it will wait for the RelicCoH process to end
-----------------------------------------------------------------------
*/

namespace start_eastern_front
{
    class Start_Entry
    {
        // Variables
        private static int timeouttime = 10;
        private static int timeouttime_end = 5;
        private static string steamfilename = "Steam";
        private static string cohfilename = "RelicCoH";
        private static string cohfilename2 = "RelicCoH.exe";
        private static string bugsplat_report = "BsSndRpt";
        private static string ef_version_locale_key = "18030216";
        private static string auth_key = "wd3DEcjqgeYqFWTUWsS6r37ja62F2EzZBGZnar9hSfNULp3hXXvUJdxY9qKrG7a9eEqAvmr6ucPRAaU5LD99ep6huyUysV7YuYBpmwWASVajk3g9JUqdtRhQbN8kCgueZnNWxQ8LhbSAF4JeUSSAvU4P4PmgGkkpzXUtnYwqt55FbJ9LyNJvc9WWNnbWz3WVpvpHvaufdrKAkUtgvvuAjpvPu3qAtRxjCGVkVeGpXJmenzcCRKCfmcZuYBBVZxJ32Xa87m45mwEjCQvaKmGf5qtkZcgvCpSdNqKWtsFCgcJSFZg6mgWfuVUFfZ3VP6D47NQAwVrdL9AYLHtSwQvv2VSQTzjEfz6J9TssE4gw2bvmugEkWGKGBDX6m6cPGv4v2AhGR4xjwkky25HYz7D7j4y2AYwqZQ8uYa2pnLj2qT3Wwx9zrk8F4czSBzpDhz6kfqZJvnXRXBCWSZsaKsuykSSs2BjAKvfnfTwVwYYz3pBCwBdWeVrrSmX34Xv26ZQT";

        // NOTE: -unqiuetoken is only a "marker" to find the right process in case of more than one RelicCoH Process
        private static string steamarguments = "-applaunch 228200 -dev -mod EF_beta -uniquetoken";

        static void Main(string[] args)
        {
            // Make sure we can only have one running at a time
            if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) Environment.Exit(0);

            // Get Eastern Front version
            string path_to_dir;
            // Get the executable's directory
            path_to_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            // Cut the path:/
            path_to_dir = path_to_dir.Substring(6);
            // Append the path to the locale
            string path_to_locale = path_to_dir + "\\EF_beta\\Locale\\English\\Eastern_Front.English.ucs";

            string version_string = "";
            string line;
            // Open file and read lines
            using (StreamReader file = new StreamReader(path_to_locale))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(ef_version_locale_key))
                    {
                        // We found our version line
                        version_string = line;

                        // Cut the version
                        version_string = version_string.Substring(version_string.Length - 7);
                        break;
                    }
                    else
                    {
                        // If we failed to find then it's 0.0.0.0 = undefined
                        version_string = "0.0.0.0";
                    }
                }
            }

            // Remove dots
            version_string = version_string.Replace(".", "");

            // Tell server that we started the game
            var webClient = new System.Net.WebClient();
            string url = "http://me2stats.eu:5020/join?version=" + version_string + "&key=" + auth_key;
            webClient.DownloadStringAsync(new Uri(url));

            Process[] steam_processes;
            
            // Try to find our Steam process
            steam_processes = Process.GetProcessesByName(steamfilename);

            if (steam_processes.Length == 0)
            {
                // Message box "Steam not running!"
                MessageBox.Show("Steam not running!", "Company of Heroes: Eastern Front", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            else if (steam_processes.Length > 1)
            {
                // Message box "Multiple Steam processes found!"
                if (MessageBox.Show("Multiple Steam processes found! This may lead to unexpected behaviour! Continue?", "Company of Heroes: Eastern Front", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    // User chose to abort
                    Environment.Exit(0);
                }
            }

            string steam_fullPath = steam_processes[0].MainModule.FileName;

            // Setup our Steam process 
            Process steam_process = new Process();

            // Set our Steam exe path
            steam_process.StartInfo.FileName = steam_fullPath;
 
            // Did the starter get arguments supplied?
            if (args.Length > 0)
            {
                // Yes, append them to our other options
                foreach (string s in args)
                {
                    // Add whitespace
                    steamarguments += " ";
                    // Append argument
                    steamarguments = steamarguments + s;
                }
            }

            // Set starting arguments
            steam_process.StartInfo.Arguments = steamarguments;

            // Start CoH:EF over Steam
            steam_process.Start();

            // Stuff we need for the loop
            int timer = 0;
            Process[] coh_processes;
            Process[] bugsplat_processes;
            Process Unique_Process = null;

            // Prepare in case we have more than two processes
            string wmiQuery = string.Format("select ProcessId, Name, CommandLine from Win32_Process where Name='{0}'", cohfilename);

            // Start scanning for RelicCOH.exe
            while (true)
            {
                // Can only be one really, because Company of Heroes will only allow one
                coh_processes = Process.GetProcessesByName(cohfilename);

                if (coh_processes.Length > 0)
                {
                    ManagementClass mgmtClass = new ManagementClass("Win32_Process");

                    foreach (ManagementObject process in mgmtClass.GetInstances())
                    {
                        // Basics - process name & pid
                        string processName = process["Name"].ToString().ToLower();

                        if ((cohfilename2.ToLower()) == processName)
                        {
                            UInt32 pid = (UInt32)process["ProcessId"];
                            Unique_Process = Process.GetProcessById((int) pid);
                            // Get the command line - can be null if we don't have permissions
                            string cmdLine = null;
                            if (process["CommandLine"] != null)
                            {
                                cmdLine = process["CommandLine"].ToString();

                                if (cmdLine.Contains("-uniquetoken"))
                                {
                                    // Yes... fucking goto
                                    // Come back if double break is invented
                                    goto endOfLoop;
                                }
                            }
                        }
                    }
                }

                // Wait 2000 ticks
                Thread.Sleep(2000);
                timer++;

                // We failed to find the process... Exit
                if (timer >= timeouttime)
                {
                    Environment.Exit(1);
                }
            }

            endOfLoop:
            // Wait for CoH to close
            Unique_Process.WaitForExit();

            // Tell the server we shutdown the game
            url = "http://me2stats.eu:5020/leave?key=" + auth_key;
            webClient.DownloadStringAsync(new Uri(url));

            // Reset timer
            timer = 0;

            // CoH has closed... Did it crash?
            // If yes we have to keep running until the BugReport is sent
            // Start scanning for BsSndRpt.exe
            while (true)
            {
                bugsplat_processes = Process.GetProcessesByName(bugsplat_report);

                // There can only be one
                if (bugsplat_processes.Length > 0)
                {
                    // Tell the server that the game crashed
                    url = "http://me2stats.eu:5020/reportcrash?version=" + version_string + "&key=" + auth_key;
                    webClient.DownloadStringAsync(new Uri(url));
                    bugsplat_processes[0].WaitForExit();
                    break;
                }

                // Wait 2000 Ticks
                Thread.Sleep(2000);
                timer++;

                // We failed to find the process... So no crash
                if (timer >= timeouttime_end)
                {
                    Environment.Exit(0);
                }
            }
            Environment.Exit(0);
        }
    }
}

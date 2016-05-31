using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Management;
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
        private static string steamfilename = "Steam";
        private static string cohfilename = "RelicCoH";
        private static string cohfilename2 = "RelicCoH.exe";

        // NOTE: -unqiuetoken is only a "marker" to find the right process in case of more than one RelicCoH Process
        private static string steamarguments = "-applaunch 228200 -dev -mod EF_beta -uniquetoken";

        static void Main(string[] args)
        {
            // Make sure we can only have one running at a time
            if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) Environment.Exit(0);

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

            /*WebClient webClient = new WebClient();

            webClient.DownloadFile("http://mysite.com/myfile.txt", @"c:\myfile.txt");*/

            // Start CoH:EF over Steam
            steam_process.Start();

            // Stuff we need for the loop
            int timer = 0;
            Process[] coh_processes;
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

                // Wait 1 second
                Thread.Sleep(1000);
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

            Environment.Exit(0);
        }
    }
}

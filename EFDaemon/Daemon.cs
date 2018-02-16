using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Threading;

namespace EFDaemon
{
    /// <summary>
    /// Second background exe, responsible for restarting CoHEF.exe in daemonmode
    /// </summary>
    class Daemon
    {
        /// <summary>
        /// NOTE: -unqiuetoken is only a "marker" to find the right process in case of more than one RelicCoH Process
        /// </summary>
        static string steamarguments = "-applaunch 317600 -daemonmode";
        /// <summary>
        /// Steam name, later used in GetProcessesByName()
        /// </summary>
        static string steamfilename = "Steam";
        /// <summary>
        /// CoH name, later used in GetProcessesByName()
        /// </summary>
        static string cohfilename = "RelicCoH";
        /// <summary>
        /// CoH name, later used to compare against results from wmi query
        /// </summary>
        static string cohfilename2 = "RelicCoH.exe";
        static int timeouttime = 60;
        static uint pid;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        static void Main(string[] args)
        {
            // Make sure we can only have one running at a time
            if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) Environment.Exit(0);

            foreach (string arg in args)
            {
                // Did the other exe start us?
                if (arg == "-daemonmode")
                {
                    int timer = 0;
                    Process[] coh_processes;
                    Process Unique_Process = null;

                    // Prepare in case we have more than two processes
                    string wmiQuery = string.Format("select ProcessId, Name, CommandLine from Win32_Process where Name='{0}'", cohfilename);
                    // Start scanning for RelicCOH.exe
                    while (true)
                    {
                        coh_processes = Process.GetProcessesByName(cohfilename);
                        // Can only be one really, because Company of Heroes will only allow one
                        if (coh_processes.Length > 0)
                        {
                            ManagementClass mgmtClass = new ManagementClass("Win32_Process");

                            foreach (ManagementObject process in mgmtClass.GetInstances())
                            {
                                // Basics - process name & pid
                                string processName = process["Name"].ToString().ToLower();
                                if ((cohfilename2.ToLower()) == processName)
                                {
                                    pid = (uint)process["ProcessId"];
                                    Unique_Process = Process.GetProcessById((int)pid);
                                    if (ProcessHasWindow(pid))
                                        goto endOfLoop;
                                }
                            }
                        }
                        // Wait 2000 ticks
                        Thread.Sleep(2000);
                        timer++;

                        // We failed to find the process... pause then break
                        if (timer >= timeouttime)
                        {
                            break;
                        }
                    }

                    endOfLoop:
                    Thread.Sleep(2000);
                    Process[] steam_processes = Process.GetProcessesByName(steamfilename);

                    if (steam_processes.Length == 0)
                    {
                        // Message box "Steam not running!"
                        MessageBox.Show("Steam is not running!", "Company of Heroes: Eastern Front", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                    else if (steam_processes.Length > 1)
                    {
                        // Message box "Multiple Steam processes found!"
                        if (MessageBox.Show("Multiple Steam processes found! This may lead to unexpected behaviour! Continue?", "Company of Heroes: Eastern Front", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                            Environment.Exit(0); // User chose to abort
                    }
                    // Setup our Steam process 
                    string steam_fullPath = steam_processes[0].MainModule.FileName;
                    Process steam_process = new Process();

                    // Set our Steam exe path and starting arguments then start over Steam
                    steam_process.StartInfo.FileName = steam_fullPath;
                    steam_process.StartInfo.Arguments = steamarguments;
                    steam_process.Start();
                }
            }
        }
        /// <summary>
        /// Check if the process had created a window yet
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        static bool ProcessHasWindow(uint pid)
        {
            return (Process.GetProcessById((int)pid).MainWindowHandle.ToInt32() != 0); // if the handle is 0; no window has spawned yet!
        }
    }
}

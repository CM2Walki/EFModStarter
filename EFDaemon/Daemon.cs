using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;
using System.Threading;

namespace EFDaemon
{
    class Daemon
    {
        // NOTE: -unqiuetoken is only a "marker" to find the right process in case of more than one RelicCoH Process
        static string steamarguments = "-applaunch 317600 -daemonmode";
        static string steamfilename = "Steam";
        static string cohfilename = "RelicCoH";
        static string cohfilename2 = "RelicCoH.exe";
        static int timeouttime = 60;
        static uint pid;

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
                                    if (ProcessHadWindow(pid) && ApplicationIsActivated(pid))
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
        static bool ProcessHadWindow(uint pid)
        {
            return (Process.GetProcessById((int)pid).MainWindowHandle.ToInt32() != 0); // if the handle is 0; no window has spawned yet!
        }

        static bool ApplicationIsActivated(uint pid)
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
                return false; // No window is currently activated

            uint activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == pid;
        }
    }
}

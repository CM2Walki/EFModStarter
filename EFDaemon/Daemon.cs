using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace EFDaemon
{
    static class Daemon
    {
        // NOTE: -unqiuetoken is only a "marker" to find the right process in case of more than one RelicCoH Process
        private static string steamarguments = "-applaunch 317600 -daemonmode";
        private static string steamfilename = "Steam";

        static void Main(string[] args)
        {
            // Make sure we can only have one running at a time
            if (Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1) Environment.Exit(0);

            foreach (string arg in args)
            {
                if (arg == "-daemonmode")
                {
                    // Did the other exe start us?
                    // This delay might seem weird, 
                    // but we don't know how long Steam takes to change game states
                    System.Threading.Thread.Sleep(30000);

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
    }
}

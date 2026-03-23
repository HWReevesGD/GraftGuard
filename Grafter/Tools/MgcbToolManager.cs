using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter.Tools
{
    internal static class MgcbToolManager
    {
        public static async Task<bool> EnsureMgcbReady()
        {
            // Check if it's already working
            if (IsMgcbInstalled())
            {
                return true;
            }

            // If not, trigger the installation/path-finding workflow
            return await AttemptMgcbInstall();
        }
        public static bool IsMgcbInstalled()
        {
            // If the path isn't set, we can't verify a local tool installation
            if (string.IsNullOrEmpty(DataManager.ProjectRootPath))
            {
                return false;
            }

            string manifestPath = Path.Combine(DataManager.ProjectRootPath, ".config", "dotnet-tools.json");
            if (!File.Exists(manifestPath))
            {
                // If no manifest, it's definitely not "installed" locally
                return false;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "tool list",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WorkingDirectory = DataManager.ProjectRootPath
                };

                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output.Contains("dotnet-mgcb-editor");
                }
            }
            catch { return false; }
        }
        public static async Task<bool> AttemptMgcbInstall()
        {
            // Explain why we need a path if we don't have it
            if (string.IsNullOrEmpty(DataManager.ProjectRootPath))
            {
                MessageBox.Show("MGCB tools are missing. Please select your Project Root (the folder containing your .csproj) to attempt a local repair.");

                // Get the path immediately
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the Root Folder of your MonoGame Project";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        DataManager.ProjectRootPath = fbd.SelectedPath;

                        if (string.IsNullOrEmpty(DataManager.ProjectContentPath))
                        {
                            DataManager.ProjectContentPath = Path.Combine(fbd.SelectedPath, "Content");
                        }
                    }
                    else
                    {
                        return false; // User cancelled
                    }
                }
            }
            //  Attempt the fix
            var confirmResult = MessageBox.Show(
                "MGCB tools need to be initialized for this project. Proceed?",
                "Setup MonoGame Tools",
                MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                string configPath = Path.Combine(DataManager.ProjectRootPath, ".config");
                if (!Directory.Exists(configPath))
                {
                    await RunDotnetCommand("new tool-manifest");
                }

                // Now install/restore
                bool success = await RunDotnetCommand("tool install dotnet-mgcb-editor");

                // If it was already installed, 'install' might fail, so try restore
                if (!success) success = await RunDotnetCommand("tool restore");

                if (success)
                {
                    MessageBox.Show("MGCB Tools configured.");
                    return true;
                }
            }
            return false;
        }
        private static async Task<bool> RunDotnetCommand(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = DataManager.ProjectRootPath
            };

            using (Process process = Process.Start(startInfo))
            {
                await Task.Run(() => process?.WaitForExit());
                return process?.ExitCode == 0;
            }
        }



    }
}

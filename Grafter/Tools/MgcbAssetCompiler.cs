using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter.Tools
{
    public static class MgcbAssetCompiler
    {
        public static void RegisterTexture(string fileName)
        {
            string mgcbPath = Path.Combine(DataManager.ProjectRootPath, "Content", "Content.mgcb");
            if (!File.Exists(mgcbPath)) return;

            string relativePath = $"Parts/{fileName}";

            string content = File.ReadAllText(mgcbPath);
            if (content.Contains($"/build:{relativePath}")) return;

            StringBuilder entry = new StringBuilder();
            entry.AppendLine($"#begin {relativePath}");
            entry.AppendLine($"/importer:TextureImporter");
            entry.AppendLine($"/processor:TextureProcessor");
            entry.AppendLine($"/processorParam:ColorKeyColor=255,0,255,255");
            entry.AppendLine($"/processorParam:ColorKeyEnabled=True");
            entry.AppendLine($"/processorParam:GenerateMipmaps=False");
            entry.AppendLine($"/processorParam:PremultiplyAlpha=True");
            entry.AppendLine($"/processorParam:TextureFormat=Color");
            entry.AppendLine($"/build:{relativePath}");
            entry.AppendLine();

            File.AppendAllText(mgcbPath, entry.ToString());
        }
        public static async Task BuildWithProgress()
        {
            await Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "mgcb /rebuild /@:Content/Content.mgcb",
                    WorkingDirectory = DataManager.ProjectRootPath,
                    CreateNoWindow = true, 
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                using (Process p = Process.Start(startInfo))
                {
                    string output = p.StandardOutput.ReadToEnd();
                    string error = p.StandardError.ReadToEnd();
                    p.WaitForExit();

                    if (p.ExitCode != 0)
                    {
                        Debug.WriteLine("MGCB Build Failed!");
                        Debug.WriteLine($"Error: {error}");
                    }

                    Debug.WriteLine($"Output: {output}");
                }
            });
        }
    }
}

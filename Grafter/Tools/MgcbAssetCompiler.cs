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
            string mgcbPath = Path.Combine(DataManager.ProjectContentPath, "Content.mgcb");
            if (!File.Exists(mgcbPath)) return;

            string content = File.ReadAllText(mgcbPath);
            if (content.Contains($"/build:{fileName}")) return;

            StringBuilder entry = new StringBuilder();
            entry.AppendLine($"#begin {fileName}");
            entry.AppendLine($"/importer:TextureImporter");
            entry.AppendLine($"/processor:TextureProcessor");
            // Your specific settings for Graft Guard or other projects
            entry.AppendLine($"/processorParam:ColorKeyColor=255,0,255,255");
            entry.AppendLine($"/processorParam:ColorKeyEnabled=True");
            entry.AppendLine($"/processorParam:GenerateMipmaps=False");
            entry.AppendLine($"/processorParam:PremultiplyAlpha=True");
            entry.AppendLine($"/processorParam:TextureFormat=Color");
            entry.AppendLine($"/build:{fileName}");
            entry.AppendLine();

            File.AppendAllText(mgcbPath, entry.ToString());
        }
        public static async Task BuildWithProgress(string fileName)
        {
           await Task.Run(() =>
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "mgcb /@:Content.mgcb",
                    WorkingDirectory = DataManager.ProjectContentPath,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                using (Process p = Process.Start(startInfo)) p.WaitForExit();
            });
        }
    }
}

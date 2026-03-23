using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter.Tools
{
    public static class FileSystemHelper
    {
        public static string CopyToContent(string sourcePath)
        {
            if (string.IsNullOrEmpty(DataManager.ProjectContentPath)) return null;

            string fileName = Path.GetFileName(sourcePath);
            string destinationPath = Path.Combine(DataManager.ProjectContentPath, fileName);

            // Copy the file only if it's not already there to save time
            if (!File.Exists(destinationPath))
            {
                File.Copy(sourcePath, destinationPath, true);
            }

            return destinationPath;
        }

        public static string GetUserFileSelection()
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.tga" })
            {
                return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
            }
        }
    }
}

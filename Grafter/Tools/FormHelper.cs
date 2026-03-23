using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Grafter.Tools
{
    public static class FormHelper
    {
        static Random rng = new Random();

        public static int GetRandomArgb()
        {
            // Generate components between 0 and 255
            int a = 255;
            int r = rng.Next(256);
            int g = rng.Next(256);
            int b = rng.Next(256);

            return Color.FromArgb(a, r, g, b).ToArgb();
        }

        public static int HexToArgb(string hex)
        {
            // Clean the string
            hex = hex.Replace("#", "");

            // Handle 6-digit hex by adding full Alpha (FF)
            if (hex.Length == 6)
            {
                hex = "FF" + hex;
            }

            // Parse the hex string to an unsigned integer
            if (uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint result))
            {
                return (int)result;
            }

            // Return a fallback (e.g., Red) if parsing fails
            return Color.Red.ToArgb();
        }

        public static void SyncTextureToPreview(ITextureEditor data, PictureBox pic, Button btn)
        {
            if (!string.IsNullOrEmpty(data.FullImagePath) && File.Exists(data.FullImagePath))
            {
                pic.Image?.Dispose();
                pic.Image = Image.FromFile(data.FullImagePath);
                btn.Text = data.TextureName;
            }
            else
            {
                pic.Image = null;
                btn.Text = "Select Texture";
            }
        }

         public static bool ConfirmDeletion(string itemName)
         {
            var result = MessageBox.Show(
                $"Are you sure you want to remove '{itemName}'?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            return result == DialogResult.Yes;
         }

        public static void RequestGlobalSave(Form parent, Action saveUiToObject)
        {
            saveUiToObject?.Invoke();

            using (SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON Files|*.json", FileName = DataManager.CurrentFilePath })
            {

                if (!string.IsNullOrEmpty(DataManager.ProjectContentPath))
                {
                    sfd.InitialDirectory = DataManager.ProjectContentPath;
                }

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    DataManager.Save(sfd.FileName);
                    MessageBox.Show("Library Saved!");
                }
            }
        }

        public static void RequestGlobalLoad(Form parent, Action value)
        {

            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "JSON Files|*.json" })
            {
                if (!string.IsNullOrEmpty(DataManager.ProjectContentPath))
                {
                    ofd.InitialDirectory = DataManager.ProjectContentPath;
                }

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    DataManager.Load(ofd.FileName);
                }
            }
        }
    }
}

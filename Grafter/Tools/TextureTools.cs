using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafter.Tools
{
    public interface ITextureEditor
    {
        string TextureName { get; set; }
        string FullImagePath { get; set; }
    }

    public static class TextureTools
    {
        public static async void ProcessNewTexture(ITextureEditor target, Action<string> onUpdate)
        {
            // Environmental Check
            if (!await MgcbToolManager.EnsureMgcbReady()) return;

            // User Input
            string selectedFile = FileSystemHelper.GetUserFileSelection();
            if (selectedFile == null) return;

            // File Ops
            string destPath = FileSystemHelper.CopyToContent(selectedFile);
            if (destPath == null) return;

            // Update the Interface object immediately
            target.TextureName = Path.GetFileNameWithoutExtension(destPath);
            target.FullImagePath = destPath;

            // Build Pipeline
            MgcbAssetCompiler.RegisterTexture(Path.GetFileName(destPath));
            await MgcbAssetCompiler.BuildWithProgress(Path.GetFileName(destPath));

            // 6. Callback for any specific UI refreshes (like reloading a PictureBox)
            onUpdate?.Invoke(destPath);
        }

    }
}

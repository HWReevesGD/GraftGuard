using Grafter.Tools;
using System.ComponentModel;
using System.Diagnostics;

namespace Grafter
{
    public class EditorController<T> where T : class, IGameDefinition, new()
    {
        private readonly ListBox listBox;
        private readonly PictureBox preview;
        private readonly Button textureBtn;
        private readonly Action<T> onLoadToUi;

        public T CurrentlyEditing { get; private set; }

        public EditorController(ListBox listBox, PictureBox preview, Button textureBtn, Action<T> onLoadToUi)
        {
            this.listBox = listBox;
            this.preview = preview;
            this.textureBtn = textureBtn;
            this.onLoadToUi = onLoadToUi;
        }

        public void HandleSelectionChanged(Action onSaveCurrent)
        {

            onSaveCurrent?.Invoke();

            if (listBox.SelectedItem is T selected)
            {
                CurrentlyEditing = selected;
                onLoadToUi(selected);
                FormHelper.SyncTextureToPreview(selected, preview, textureBtn);
            }
            else
            {
                CurrentlyEditing = null;
                preview.Image = null;
                textureBtn.Text = "Select Texture";
            }
        }

        public void HandleDeletion(KeyEventArgs e, BindingList<T> sourceList, Action onClearUi)
        {
            if ((e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) && listBox.SelectedItem is T selected)
            {
                if (FormHelper.ConfirmDeletion(selected.Name))
                {
                    sourceList.Remove(selected);
                    if (CurrentlyEditing == selected)
                    {
                        CurrentlyEditing = null;
                        onClearUi?.Invoke();
                        preview.Image?.Dispose();
                        preview.Image = null;
                    }
                }
            }
        }
        public void CreateNewItem(BindingList<T> sourceList)
        {
            // Create the new instance
            T newItem = new();

            newItem.InitializeDefault();

            // Add to the DataManager's list
            sourceList.Add(newItem);

            // Auto-select in the UI
            listBox.SelectedItem = newItem;
        }

        public async Task ChangeTexture(Action<string> onUpdate = null)
        {
            if (CurrentlyEditing == null) return;

            // Get the file from the user
            string selectedFile = FileSystemHelper.GetUserFileSelection();
            if (selectedFile == null) return;

            // Move to project folder and update the object's data
            string destPath = FileSystemHelper.CopyToContent(selectedFile);
            if (destPath == null) return;

            CurrentlyEditing.FullImagePath = destPath;
            CurrentlyEditing.TextureName = Path.GetFileNameWithoutExtension(destPath);

            // Register and Build in the background
            MgcbAssetCompiler.RegisterTexture(Path.GetFileName(destPath));

            // We update the UI immediately so it feels responsive
            FormHelper.SyncTextureToPreview(CurrentlyEditing, preview, textureBtn);

            // Run the actual build
            await MgcbAssetCompiler.BuildWithProgress();

            // Final callback (e.g., if you need to force a redraw)
            onUpdate?.Invoke(destPath);
        }

    }
}

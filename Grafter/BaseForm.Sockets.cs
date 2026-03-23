using Grafter.Tools;

namespace Grafter
{
    
    // This file handles ONLY the socket/context menu logic and ui
    public partial class BaseForm
    {
        private ContextMenuStrip socketContextMenu;

        #region Logic
        private void InitializeSocketMenu()
        {
            socketContextMenu = new ContextMenuStrip();
            socketContextMenu.Items.Add("Rename Socket", null, RenameSocket_Click);
            socketContextMenu.Items.Add("Edit Color (Hex)", null, ChangeSocketColor_Click);

            socketContextMenu.Items.Add(new ToolStripSeparator());

            socketContextMenu.Items.Add("Remove Socket", null, (s, e) => RemoveSelectedSocket());

            lstSockets.ContextMenuStrip = socketContextMenu;

            // Handle right-click selection
            lstSockets.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Right)
                {
                    int index = lstSockets.IndexFromPoint(e.Location);
                    if (index != ListBox.NoMatches) lstSockets.SelectedIndex = index;
                }
            };
        }

        private void RenameSocket_Click(object sender, EventArgs e)
        {
            if (currentlyEditingSocket == null) return;

            string name = Microsoft.VisualBasic.Interaction.InputBox("Rename:", "Socket", currentlyEditingSocket.Name);
            if (!string.IsNullOrWhiteSpace(name))
            {
                currentlyEditingSocket.Name = name;
                RefreshSocketList();
            }
        }

        private void ChangeSocketColor_Click(object sender, EventArgs e)
        {
            if (currentlyEditingSocket == null) return;

            string hexColor = Microsoft.VisualBasic.Interaction.InputBox("Enter new color Hex Value:", "Socket", currentlyEditingSocket.Name);
            if (!string.IsNullOrWhiteSpace(hexColor))
            {
                currentlyEditingSocket.ColorArgb = FormHelper.HexToArgb(hexColor);
                picPreview.Invalidate();
            }

        }

        private void RemoveSelectedSocket()
        {
            if (controller.CurrentlyEditing == null || currentlyEditingSocket == null) return;

            if (FormHelper.ConfirmDeletion(currentlyEditingSocket.Name))
            {
                // This automatically triggers the ListBox update
                controller.CurrentlyEditing.AttachPoints.Remove(currentlyEditingSocket);
                currentlyEditingSocket = null;
                picPreview.Invalidate();
            }
        }

        private void RefreshSocketList()
        {
            // Forces the owner-draw list to repaint the new names/colors
            lstSockets.BeginUpdate();
            var temp = lstSockets.DataSource;
            lstSockets.DataSource = null;
            lstSockets.DataSource = temp;
            lstSockets.EndUpdate();
        }

        #endregion

        #region UI
        private void addSocket_Click(object sender, EventArgs e)
        {
            if (currentlyEditing == null) return;

            var newSocket = new AttachPoint
            {
                Name = $"Socket_{currentlyEditing.AttachPoints.Count}",
                ColorArgb = FormHelper.GetRandomArgb()
            };

            // This immediately shows up in the ListBox now
            currentlyEditing.AttachPoints.Add(newSocket);

            // Auto-select the new one for the user
            lstSockets.SelectedItem = newSocket;

            lstSockets.Invalidate();
        }

        private void lstSockets_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentlyEditingSocket = lstSockets.SelectedItem as AttachPoint;
            picPreview.Invalidate();
        }
        private void LstSockets_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Check if the index is valid for the current collection size
            if (e.Index < 0 || currentlyEditing == null || e.Index >= currentlyEditing.AttachPoints.Count)
                return;

            var ap = currentlyEditing.AttachPoints[e.Index];
            e.DrawBackground();

            using (Brush b = new SolidBrush(Color.FromArgb(ap.ColorArgb)))
            {
                // Use a null-safe check for the socket name
                string socketName = ap.Name ?? "Unnamed Socket";
                string text = (ap == currentlyEditingSocket) ? $"> {socketName}" : socketName;

                e.Graphics.DrawString(text, e.Font, b, e.Bounds);
            }
            e.DrawFocusRectangle();
        }
        private void lstSockets_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete) && lstSockets.SelectedItem is AttachPoint selected)
            {
                RemoveSelectedSocket();
            }
        }
        #endregion
    }

}

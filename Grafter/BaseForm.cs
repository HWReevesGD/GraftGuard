using Grafter.Tools;
using System.Diagnostics;

namespace Grafter
{
    public partial class BaseForm : Form
    {
        private EditorController<BaseDefinition> controller;

        private BaseDefinition currentlyEditing;
        private AttachPoint currentlyEditingSocket;

        public BaseForm()
        {
            InitializeComponent();

            // Initialize the controller
            controller = new EditorController<BaseDefinition>(lstBases, picPreview, btnSelectTexture, LoadObjectToUi);

            // Setup UI behavior
            InitializeSocketMenu();
            lstSockets.DrawMode = DrawMode.OwnerDrawFixed;
            lstSockets.DrawItem += LstSockets_DrawItem;
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;

            lstBases.DataSource = DataManager.Bases;

            Debug.WriteLine("Working Directory: " + Directory.GetCurrentDirectory());
        }


        #region Save / Load
        private void btnSave_Click(object sender, EventArgs e) => FormHelper.RequestGlobalSave(this, SaveUiToObject);

        private void btnLoad_Click(object sender, EventArgs e) => FormHelper.RequestGlobalLoad(this, () =>
        {
            if (DataManager.Bases.Count > 0)
            {
                lstBases.SelectedIndex = 0;
            }
        });

        #endregion

        #region List Interaction
        private void lstBases_SelectedIndexChanged(object sender, EventArgs e)
            => controller.HandleSelectionChanged(SaveUiToObject);

        private void lstBases_KeyDown(object sender, KeyEventArgs e)
            => controller.HandleDeletion(e, DataManager.Bases, () => txtName.Text = "");
        #endregion

        #region Socket Point Placement
        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            if (controller.CurrentlyEditing == null || picPreview.Image == null) return;

            foreach (var socket in controller.CurrentlyEditing.AttachPoints)
            {
                Point pt = GeometryHelper.ImageToControl(picPreview, socket.PivotX, socket.PivotY);
                bool isSelected = (socket == currentlyEditingSocket);

                using (Pen p = new Pen(Color.FromArgb(socket.ColorArgb), isSelected ? 3 : 1))
                {
                    // Draw Crosshair
                    e.Graphics.DrawLine(p, pt.X - 10, pt.Y, pt.X + 10, pt.Y);
                    e.Graphics.DrawLine(p, pt.X, pt.Y - 10, pt.X, pt.Y + 10);

                    if (isSelected)
                        e.Graphics.DrawRectangle(p, pt.X - 5, pt.Y - 5, 10, 10);
                }
            }
        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentlyEditingSocket == null || picPreview.Image == null) return;

            // Convert the mouse click (control space) to 0.0-1.0 (image space)
            PointF relativePos = GeometryHelper.ControlToImage(picPreview, e.Location);

            // Save the normalized coordinates
            currentlyEditingSocket.PivotX = relativePos.X;
            currentlyEditingSocket.PivotY = relativePos.Y;

            // Refresh the view
            picPreview.Invalidate();
        }
        #endregion

        #region UI Object Representation
        private void SaveUiToObject()
        {
            if (currentlyEditing != null)
            {
                currentlyEditing.Name = txtName.Text;
                currentlyEditing.IsTorso = isTorso.Checked;


                DataManager.Bases.ResetBindings();
            }
        }

        private void LoadObjectToUi(BaseDefinition part)
        {
            currentlyEditing = part;
            txtName.Text = part.Name;
            isTorso.Checked = part.IsTorso;

            lstSockets.DataSource = part.AttachPoints;

            FormHelper.SyncTextureToPreview(part, picPreview, btnSelectTexture);

            //wake up the pic preview so it shows any new sockets
            picPreview.Invalidate();
        }
        #endregion

        private void btnAdd_Click(object sender, EventArgs e) => controller.CreateNewItem(DataManager.Bases);

        private async void btnSelectTexture_Click(object sender, EventArgs e) { await controller.ChangeTexture(); }

    }
}

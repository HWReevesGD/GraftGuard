using Grafter.Tools;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Forms.Design.Behavior;
using System.Xml.Linq;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Grafter
{
    public partial class Form1 : Form
    {
        private EditorController<PartDefinition> controller;


        public Form1()
        {
            InitializeComponent();
            controller = new EditorController<PartDefinition>(lstParts, picPreview, btnSelectTexture, LoadObjectToUi);
            lstParts.DataSource = DataManager.Parts;
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            Debug.WriteLine("Working Directory: " + Directory.GetCurrentDirectory());
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            txtContentPath.Text = DataManager.ProjectContentPath;

            bool isReady = await MgcbToolManager.EnsureMgcbReady();

            if (!isReady)
            {
                MessageBox.Show("Grafter requires MGCB tools to function. The application will now close.",
                                "Initialization Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            txtProjDirPath.Text = DataManager.ProjectRootPath;

            string lastPath = Properties.Settings.Default.LastLibraryPath;

            if (!string.IsNullOrEmpty(lastPath) && File.Exists(lastPath))
            {
                // Load the data into the manager
                DataManager.Load(lastPath);

                // Sync the UI if data was found
                if (DataManager.Parts.Count > 0)
                {
                    lstParts.SelectedIndex = 0;
                }
            }
        }

        #region Save/ Load
        private void btnSave_Click(object sender, EventArgs e) => FormHelper.RequestGlobalSave(this, SaveUiToObject);

        private void btnLoad_Click(object sender, EventArgs e) => FormHelper.RequestGlobalLoad(this, () => {
            if (DataManager.Parts.Count > 0)
            {
                lstParts.SelectedIndex = 0;
            }
        });

        #endregion

        #region List Interaction
        private void lstParts_SelectedIndexChanged(object sender, EventArgs e)
            => controller.HandleSelectionChanged(SaveUiToObject);

        private void lstParts_KeyDown(object sender, KeyEventArgs e)
            => controller.HandleDeletion(e, DataManager.Parts, () => txtName.Text = "");
        #endregion
        
        private void btnAdd_Click(object sender, EventArgs e) => controller.CreateNewItem(DataManager.Parts);

        private async void btnSelectTexture_Click(object sender, EventArgs e)
        {
            await controller.ChangeTexture();
        }

        #region pivot placement
        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            // Use the controller's current object
            var part = controller.CurrentlyEditing;
            if (part == null || picPreview.Image == null) return;

            // Convert the 0.0-1.0 relative coordinates back to control pixels
            Point pt = GeometryHelper.ImageToControl(picPreview, part.PivotX, part.PivotY);

            using (Pen p = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawLine(p, pt.X - 10, pt.Y, pt.X + 10, pt.Y);
                e.Graphics.DrawLine(p, pt.X, pt.Y - 10, pt.X, pt.Y + 10);
                e.Graphics.DrawRectangle(p, pt.X - 2, pt.Y - 2, 4, 4);
            }
        
        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            var part = controller.CurrentlyEditing;
            if (part == null || picPreview.Image == null) return;

            // Convert the mouse click (control space) to 0.0-1.0 (image space)
            PointF relativePos = GeometryHelper.ControlToImage(picPreview, e.Location);

            // Save the normalized coordinates
            part.PivotX = relativePos.X;
            part.PivotY = relativePos.Y;

            // Refresh the view
            picPreview.Invalidate();
        }

        #endregion

        #region UI Object Representation
        private void SaveUiToObject()
        {
            if (controller.CurrentlyEditing != null)
            {
                controller.CurrentlyEditing.Name = txtName.Text;
                controller.CurrentlyEditing.BaseDamage = (float)numDamage.Value;
                controller.CurrentlyEditing.SpeedModifier = (float)numSpeed.Value;
                controller.CurrentlyEditing.ArmorModifier = (float)numArmor.Value;
                controller.CurrentlyEditing.RangeModifier = (float)numRange.Value;
                controller.CurrentlyEditing.CriticalModifier = (float)numCritical.Value;
                controller.CurrentlyEditing.HealthModifier = (float)numHealth.Value;
                controller.CurrentlyEditing.Type = isHead.Checked ? PartType.Head : PartType.Limb;

                SaveBehaviors();

                DataManager.Parts.ResetBindings(); // Updates the name in the list instantly
            }
        }

        private void LoadBehaviors(PartDefinition part)
        {
            // Uncheck all Behaviors
            for (int index = 0; index < chkBehaviors.Items.Count; index++)
            {
                chkBehaviors.SetItemChecked(index, false);
            }

            // Check used Behaviors
            foreach (string behavior in part.PartBehaviorNames)
            {
                for (int index = 0; index < chkBehaviors.Items.Count; index++)
                {
                    string item = (string)chkBehaviors.Items[index];
                    if (behavior == item)
                    {

                        chkBehaviors.SetItemChecked(index, true);
                    }
                }
            }
        }

        private void SaveBehaviors()
        {
            // Save Behaviors
            currentlyEditing.PartBehaviorNames = [];
            for (int index = 0; index < chkBehaviors.Items.Count; index++)
            {
                if (chkBehaviors.GetItemChecked(index))
                {
                    currentlyEditing.PartBehaviorNames = currentlyEditing.PartBehaviorNames.Append((string)chkBehaviors.Items[index]).ToArray();
                }
            }
        }

        private void LoadObjectToUi(PartDefinition part)
        {
            txtName.Text = part.Name;
            numDamage.Value = (decimal)part.BaseDamage;
            numSpeed.Value = (decimal)part.SpeedModifier;
            numArmor.Value = (decimal)part.ArmorModifier;
            numRange.Value = (decimal)part.RangeModifier;
            numCritical.Value = (decimal)part.CriticalModifier;
            numHealth.Value = (decimal)part.HealthModifier;
            isHead.Checked = part.Type == PartType.Head;

            LoadBehaviors();

            FormHelper.SyncTextureToPreview(part, picPreview, btnSelectTexture);

            picPreview.Invalidate();
        }
        #endregion

        private void baseSelection_Click(object sender, EventArgs e)
        {
            BaseForm baseWindow = new BaseForm();
            baseWindow.Show();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataManager.LoadBehaviors();
            foreach (string behavior in DataManager.Behaviors)
            {
                chkBehaviors.Items.Add(behavior);
            }
        }
        #region Directory Selection
        private void ctSelectButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    DataManager.ProjectContentPath = fbd.SelectedPath;
                    txtContentPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void mgcbSelectButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Executable Files|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    //DataManager.MgcbPath = ofd.FileName;
                    txtProjDirPath.Text = ofd.FileName;
                }
            }
        }

        #endregion


    }

}

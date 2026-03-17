using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Grafter
{
    public partial class Form1 : Form
    {
        BindingList<PartData> partsList = new BindingList<PartData>();

        private PartData currentlyEditing;

        private string projectContentPath = @"..\..\..\..\GraftGuard\Content\Parts\";

        public Form1()
        {
            InitializeComponent();
            lstParts.DataSource = partsList;
            picPreview.SizeMode = PictureBoxSizeMode.Zoom;
            Debug.WriteLine("Working Directory: " + Directory.GetCurrentDirectory());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveUiToObject();

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
                sfd.Title = "Save Your Parts List";
                sfd.FileName = "parts_data.csv"; // Default name

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var lines = partsList.Select(p => p.ToCsv());
                    File.WriteAllLines(sfd.FileName, lines);


                    MessageBox.Show("File saved successfully!");
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSV Files (*.csv)|*.csv";
                ofd.Title = "Select a Parts CSV File";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    partsList.Clear();
                    var lines = File.ReadAllLines(ofd.FileName);

                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            try
                            {
                                partsList.Add(PartData.FromCsv(line));

                            }
                            catch
                            {
                                MessageBox.Show($"Error reading line: {line}");
                            }
                        }
                    }

                    // --- AUTO-FILL LOGIC ---
                    // Check if we actually loaded any parts
                    if (partsList.Count > 0)
                    {
                        lstParts.SelectedIndex = 0; // This triggers SelectedIndexChanged
                    }
                }


            }
        }

        private void lstParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveUiToObject();

            // Ensure something is actually selected
            if (lstParts.SelectedItem is PartData selected)
            {

                currentlyEditing = selected;

                txtName.Text = selected.Name;
                numDamage.Value = (decimal)selected.BaseDamage;
                numSpeed.Value = (decimal)selected.SpeedModifier;
                numArmor.Value = (decimal)selected.ArmorModifier;
                numRange.Value = (decimal)selected.RangeModifier;
                numCritical.Value = (decimal)selected.CriticalModifier;
                numHealth.Value = (decimal)selected.HealthModifier;
                // Reconstruct the path based on the local project directory
                string expectedPath = Path.Combine(projectContentPath, selected.TextureName + ".png");

                picPreview.Image = null;

                if (File.Exists(expectedPath))
                {
                    selected.FullImagePath = expectedPath;
                    picPreview.Image = Image.FromFile(expectedPath);
                    btnSelectTexture.Text = selected.TextureName;
                }
                else
                {
                    picPreview.Image = null;
                    btnSelectTexture.Text = "Select Texture";
                }

            }
            else
            {
                picPreview.Image = null;
                btnSelectTexture.Text = "Select Texture";
                currentlyEditing = null;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Check if an item is actually selected in the ListBox
            if (lstParts.SelectedItem is PartData selected)
            {
                // Pull values from the UI controls and cast them back to float
                selected.Name = txtName.Text;
                selected.BaseDamage = (float)numDamage.Value;
                selected.SpeedModifier = (float)numSpeed.Value;
                selected.ArmorModifier = (float)numArmor.Value;
                selected.RangeModifier = (float)numRange.Value;
                selected.CriticalModifier = (float)numCritical.Value;
                selected.HealthModifier = (float)numHealth.Value;

                // Refresh the ListBox display so the name updates if changed
                partsList.ResetBindings();

                MessageBox.Show($"{selected.Name} updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a part from the list first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var newPart = new PartData { Name = "New Part" };
            partsList.Add(newPart);
            lstParts.SelectedItem = newPart; // Auto-select the new one
        }

        private void btnSelectTexture_Click(object sender, EventArgs e)
        {
            if (currentlyEditing == null) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.png;*.jpg;*.tga";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string sourcePath = ofd.FileName;
                    string fileName = Path.GetFileName(sourcePath);
                    string destinationPath = Path.Combine(projectContentPath, fileName);

                    try
                    {
                        // Only copy if the file isn't already there
                        if (!File.Exists(destinationPath))
                        {
                            if (!Directory.Exists(projectContentPath)) Directory.CreateDirectory(projectContentPath);
                            File.Copy(sourcePath, destinationPath);

                            // Trigger compilation only for NEW files
                            RegisterWithMonoGame(destinationPath);
                        }

                        // Always update the object data, even if the file already existed
                        currentlyEditing.TextureName = Path.GetFileNameWithoutExtension(fileName);
                        currentlyEditing.FullImagePath = destinationPath;

                        // Update UI Preview
                        if (picPreview.Image != null) picPreview.Image.Dispose();
                        picPreview.Image = Image.FromFile(destinationPath);
                        btnSelectTexture.Text = currentlyEditing.TextureName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error linking texture: {ex.Message}");
                    }
                }
            }
        }

        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            // This ensures pixel art stays crisp when zoomed in
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            if (currentlyEditing == null || picPreview.Image == null) return;

            // Calculate the actual screen position of the pivot
            // (Inverse of the math used in MouseClick)
            float ratioX = (float)picPreview.ClientSize.Width / picPreview.Image.Width;
            float ratioY = (float)picPreview.ClientSize.Height / picPreview.Image.Height;
            float ratio = Math.Min(ratioX, ratioY);
            float imgWidth = picPreview.Image.Width * ratio;
            float imgHeight = picPreview.Image.Height * ratio;
            float offsetX = (picPreview.ClientSize.Width - imgWidth) / 2;
            float offsetY = (picPreview.ClientSize.Height - imgHeight) / 2;

            float screenPivotX = offsetX + (currentlyEditing.PivotX * imgWidth);
            float screenPivotY = offsetY + (currentlyEditing.PivotY * imgHeight);

            // Draw a bright crosshair
            using (Pen p = new Pen(Color.Red, 2))
            {
                e.Graphics.DrawLine(p, screenPivotX - 10, screenPivotY, screenPivotX + 10, screenPivotY);
                e.Graphics.DrawLine(p, screenPivotX, screenPivotY - 10, screenPivotX, screenPivotY + 10);
            }
        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentlyEditing == null || picPreview.Image == null) return;

            // 1. Calculate the scale ratio of the image inside the Zoomed PictureBox
            float ratioX = (float)picPreview.ClientSize.Width / picPreview.Image.Width;
            float ratioY = (float)picPreview.ClientSize.Height / picPreview.Image.Height;
            float ratio = Math.Min(ratioX, ratioY);

            // 2. Calculate the offset of the image inside the PictureBox
            float imgWidth = picPreview.Image.Width * ratio;
            float imgHeight = picPreview.Image.Height * ratio;
            float offsetX = (picPreview.ClientSize.Width - imgWidth) / 2;
            float offsetY = (picPreview.ClientSize.Height - imgHeight) / 2;

            // 3. Convert Mouse Click to Image Coordinates
            float clickedImgX = (e.X - offsetX) / ratio;
            float clickedImgY = (e.Y - offsetY) / ratio;

            // 4. Convert to Normalized Coordinates (0.0 to 1.0)
            if (clickedImgX >= 0 && clickedImgX <= picPreview.Image.Width &&
                clickedImgY >= 0 && clickedImgY <= picPreview.Image.Height)
            {
                currentlyEditing.PivotX = clickedImgX / picPreview.Image.Width;
                currentlyEditing.PivotY = clickedImgY / picPreview.Image.Height;

                // Force the PictureBox to redraw so we can see a crosshair
                picPreview.Invalidate();
                MessageBox.Show($"Pivot set to: {currentlyEditing.PivotX:F2}, {currentlyEditing.PivotY:F2}");
            }
        }
        private void SaveUiToObject()
        {
            if (currentlyEditing != null)
            {
                currentlyEditing.Name = txtName.Text;
                currentlyEditing.BaseDamage = (float)numDamage.Value;
                currentlyEditing.SpeedModifier = (float)numSpeed.Value;
                currentlyEditing.ArmorModifier = (float)numArmor.Value;
                currentlyEditing.RangeModifier = (float)numRange.Value;
                currentlyEditing.CriticalModifier = (float)numCritical.Value;
                currentlyEditing.HealthModifier = (float)numHealth.Value;

                partsList.ResetBindings(); // Updates the name in the list instantly
            }
        }

        private void RegisterWithMonoGame(string texturePath)
        {
            // This tells MonoGame's builder to recognize the new PNG we just dropped in
            // You can point this to your .mgcb file or run the raw command as you did before
            CompileTexture(texturePath, projectContentPath);
        }

        private void CompileTexture(string sourcePngPath, string outputFolder)
        {
            // Get the filename without extension (e.g., "Sword")
            string fileName = Path.GetFileNameWithoutExtension(sourcePngPath);

            // Setup the MGCB command
            string arguments = $"/importer:TextureImporter /processor:TextureProcessor " +
                               $"/build:\"{sourcePngPath}\" /outputDir:\"{outputFolder}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "mgcb.exe", // You may need the full path to the mgcb executable
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                if (process.ExitCode == 0)
                    MessageBox.Show($"{fileName}.xnb compiled successfully!");
                else
                    MessageBox.Show("Compilation failed. Check if mgcb.exe is in your PATH.");
            }
        }

        private void lstParts_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the user pressed Backspace or Delete
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                // Ensure an item is actually selected
                if (lstParts.SelectedItem is PartData selectedPart)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to remove '{selectedPart.Name}'?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Remove from the BindingList (updates UI immediately)
                        partsList.Remove(selectedPart);

                        // Clear the preview if we deleted the part we were editing
                        if (currentlyEditing == selectedPart)
                        {
                            currentlyEditing = null;
                            picPreview.Image?.Dispose();
                            picPreview.Image = null;
                            txtName.Text = "";
                        }

                    }
                }
            }
        }
    }
}

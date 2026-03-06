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

        private string filePath = "parts_data.csv";

        public Form1()
        {
            InitializeComponent();
            lstParts.DataSource = partsList;
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

                    // Optional: Store this path so you know where to auto-load from next time
                    this.filePath = sfd.FileName;
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
                    this.filePath = ofd.FileName; // Keep track of the file we opened

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

                if(selected.FullImagePath != "")
                {
                    picPreview.Image = Image.FromFile(selected.FullImagePath);
                }
                else
                {
                    picPreview.Image = null;
                }

                if (selected.FullImagePath != "")
                {
                    btnSelectTexture.Text = selected.TextureName;
                }
                else
                {
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
                ofd.Filter = "Image Files|*.png;*.jpg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // Store the full path for the editor to use right now
                    currentlyEditing.FullImagePath = ofd.FileName;
                    currentlyEditing.TextureName = Path.GetFileNameWithoutExtension(ofd.FileName);

                    // Show preview in PictureBox
                    picPreview.Image = Image.FromFile(ofd.FileName);
                    btnSelectTexture.Text = currentlyEditing.TextureName;
                }
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

    }
}

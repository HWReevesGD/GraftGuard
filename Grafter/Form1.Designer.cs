namespace Grafter
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            picPreview = new PictureBox();
            btnSave = new Button();
            btnLoad = new Button();
            label3 = new Label();
            lstParts = new ListBox();
            numDamage = new NumericUpDown();
            label4 = new Label();
            label5 = new Label();
            numSpeed = new NumericUpDown();
            numArmor = new NumericUpDown();
            label6 = new Label();
            numCritical = new NumericUpDown();
            label7 = new Label();
            label8 = new Label();
            numHealth = new NumericUpDown();
            label9 = new Label();
            numRange = new NumericUpDown();
            btnAdd = new Button();
            label10 = new Label();
            btnSelectTexture = new Button();
            baseSelection = new Button();
            isHead = new CheckBox();
            chkBehaviors = new CheckedListBox();
            behaviorLabel = new Label();
            txtContentPath = new TextBox();
            ctSelectButton = new Button();
            projDirSelectButton = new Button();
            txtProjDirPath = new TextBox();
            labelRarity = new Label();
            numRarity = new NumericUpDown();
            descriptionLabel = new Label();
            textBoxDescription = new TextBox();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDamage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numArmor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCritical).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHealth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRarity).BeginInit();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(228, 354);
            txtName.Margin = new Padding(4);
            txtName.Name = "txtName";
            txtName.Size = new Size(301, 39);
            txtName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F);
            label1.Location = new Point(35, 11);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(348, 128);
            label1.TabIndex = 1;
            label1.Text = "Grafter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(59, 354);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(141, 37);
            label2.TabIndex = 2;
            label2.Text = "Part Name";
            // 
            // picPreview
            // 
            picPreview.Location = new Point(895, 109);
            picPreview.Margin = new Padding(4);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(654, 727);
            picPreview.TabIndex = 3;
            picPreview.TabStop = false;
            picPreview.Paint += picPreview_Paint;
            picPreview.MouseClick += picPreview_MouseClick;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(59, 926);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(232, 43);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(301, 926);
            btnLoad.Margin = new Padding(4);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(232, 43);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(895, 852);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(248, 32);
            label3.TabIndex = 6;
            label3.Text = "Active Base: Unknown";
            // 
            // lstParts
            // 
            lstParts.FormattingEnabled = true;
            lstParts.Location = new Point(59, 154);
            lstParts.Margin = new Padding(4);
            lstParts.Name = "lstParts";
            lstParts.Size = new Size(470, 164);
            lstParts.TabIndex = 7;
            lstParts.SelectedIndexChanged += lstParts_SelectedIndexChanged;
            lstParts.KeyDown += lstParts_KeyDown;
            // 
            // numDamage
            // 
            numDamage.DecimalPlaces = 2;
            numDamage.Location = new Point(266, 410);
            numDamage.Margin = new Padding(4);
            numDamage.Name = "numDamage";
            numDamage.Size = new Size(267, 39);
            numDamage.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(59, 410);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(178, 37);
            label4.TabIndex = 9;
            label4.Text = "Base Damage";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(55, 533);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(199, 37);
            label5.TabIndex = 10;
            label5.Text = "Speed Modifier";
            // 
            // numSpeed
            // 
            numSpeed.DecimalPlaces = 2;
            numSpeed.Location = new Point(262, 535);
            numSpeed.Margin = new Padding(4);
            numSpeed.Name = "numSpeed";
            numSpeed.Size = new Size(267, 39);
            numSpeed.TabIndex = 11;
            // 
            // numArmor
            // 
            numArmor.DecimalPlaces = 2;
            numArmor.Location = new Point(266, 599);
            numArmor.Margin = new Padding(4);
            numArmor.Name = "numArmor";
            numArmor.Size = new Size(267, 39);
            numArmor.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.Location = new Point(59, 597);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(199, 37);
            label6.TabIndex = 13;
            label6.Text = "Armor Modifier";
            // 
            // numCritical
            // 
            numCritical.DecimalPlaces = 2;
            numCritical.Location = new Point(266, 727);
            numCritical.Margin = new Padding(4);
            numCritical.Name = "numCritical";
            numCritical.Size = new Size(267, 39);
            numCritical.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.Location = new Point(59, 727);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(207, 37);
            label7.TabIndex = 15;
            label7.Text = "Critical Modifier";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 10F);
            label8.Location = new Point(59, 796);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(203, 37);
            label8.TabIndex = 16;
            label8.Text = "Health Modifier";
            // 
            // numHealth
            // 
            numHealth.DecimalPlaces = 2;
            numHealth.Location = new Point(266, 798);
            numHealth.Margin = new Padding(4);
            numHealth.Name = "numHealth";
            numHealth.Size = new Size(267, 39);
            numHealth.TabIndex = 17;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 10F);
            label9.Location = new Point(59, 661);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(200, 37);
            label9.TabIndex = 21;
            label9.Text = "Range Modifier";
            // 
            // numRange
            // 
            numRange.DecimalPlaces = 2;
            numRange.Location = new Point(266, 661);
            numRange.Margin = new Padding(4);
            numRange.Name = "numRange";
            numRange.Size = new Size(267, 39);
            numRange.TabIndex = 20;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(59, 979);
            btnAdd.Margin = new Padding(4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(474, 43);
            btnAdd.TabIndex = 22;
            btnAdd.Text = "Add Part";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(59, 862);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(101, 37);
            label10.TabIndex = 23;
            label10.Text = "Texture";
            // 
            // btnSelectTexture
            // 
            btnSelectTexture.Location = new Point(228, 860);
            btnSelectTexture.Margin = new Padding(4);
            btnSelectTexture.Name = "btnSelectTexture";
            btnSelectTexture.Size = new Size(303, 43);
            btnSelectTexture.TabIndex = 24;
            btnSelectTexture.Text = "Select Texture";
            btnSelectTexture.UseVisualStyleBackColor = true;
            btnSelectTexture.Click += btnSelectTexture_Click;
            // 
            // baseSelection
            // 
            baseSelection.Location = new Point(1285, 852);
            baseSelection.Margin = new Padding(6);
            baseSelection.Name = "baseSelection";
            baseSelection.Size = new Size(264, 41);
            baseSelection.TabIndex = 25;
            baseSelection.Text = "Select Base/Torso";
            baseSelection.UseVisualStyleBackColor = true;
            baseSelection.Click += baseSelection_Click;
            // 
            // isHead
            // 
            isHead.AutoSize = true;
            isHead.Location = new Point(396, 81);
            isHead.Margin = new Padding(4);
            isHead.Name = "isHead";
            isHead.Size = new Size(136, 36);
            isHead.TabIndex = 26;
            isHead.Text = "Is Head?";
            isHead.UseVisualStyleBackColor = true;
            // 
            // chkBehaviors
            // 
            chkBehaviors.Location = new Point(560, 339);
            chkBehaviors.Name = "chkBehaviors";
            chkBehaviors.Size = new Size(272, 688);
            chkBehaviors.TabIndex = 29;
            // 
            // behaviorLabel
            // 
            behaviorLabel.AutoSize = true;
            behaviorLabel.Font = new Font("Segoe UI", 12F);
            behaviorLabel.Location = new Point(578, 273);
            behaviorLabel.Name = "behaviorLabel";
            behaviorLabel.Size = new Size(220, 45);
            behaviorLabel.TabIndex = 28;
            behaviorLabel.Text = "Part Behaviors";
            behaviorLabel.Click += label11_Click;
            // 
            // txtContentPath
            // 
            txtContentPath.Location = new Point(895, 901);
            txtContentPath.Margin = new Padding(6);
            txtContentPath.Name = "txtContentPath";
            txtContentPath.Size = new Size(329, 39);
            txtContentPath.TabIndex = 27;
            // 
            // ctSelectButton
            // 
            ctSelectButton.Location = new Point(1236, 901);
            ctSelectButton.Margin = new Padding(6);
            ctSelectButton.Name = "ctSelectButton";
            ctSelectButton.Size = new Size(313, 41);
            ctSelectButton.TabIndex = 28;
            ctSelectButton.Text = "Select Content Folder";
            ctSelectButton.UseVisualStyleBackColor = true;
            ctSelectButton.Click += ctSelectButton_Click;
            // 
            // projDirSelectButton
            // 
            projDirSelectButton.Location = new Point(1236, 954);
            projDirSelectButton.Margin = new Padding(6);
            projDirSelectButton.Name = "projDirSelectButton";
            projDirSelectButton.Size = new Size(313, 39);
            projDirSelectButton.TabIndex = 29;
            projDirSelectButton.Text = "Select Project Directory";
            projDirSelectButton.UseVisualStyleBackColor = true;
            projDirSelectButton.Click += mgcbSelectButton_Click;
            // 
            // txtProjDirPath
            // 
            txtProjDirPath.Location = new Point(895, 954);
            txtProjDirPath.Margin = new Padding(6);
            txtProjDirPath.Name = "txtProjDirPath";
            txtProjDirPath.Size = new Size(329, 39);
            txtProjDirPath.TabIndex = 30;
            // 
            // labelRarity
            // 
            labelRarity.AutoSize = true;
            labelRarity.Font = new Font("Segoe UI", 10F);
            labelRarity.Location = new Point(59, 468);
            labelRarity.Margin = new Padding(4, 0, 4, 0);
            labelRarity.Name = "labelRarity";
            labelRarity.Size = new Size(85, 37);
            labelRarity.TabIndex = 32;
            labelRarity.Text = "Rarity";
            labelRarity.Click += label11_Click_1;
            // 
            // numRarity
            // 
            numRarity.DecimalPlaces = 2;
            numRarity.Location = new Point(266, 468);
            numRarity.Margin = new Padding(4);
            numRarity.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            numRarity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRarity.Name = "numRarity";
            numRarity.Size = new Size(267, 39);
            numRarity.TabIndex = 31;
            numRarity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Font = new Font("Segoe UI", 12F);
            descriptionLabel.Location = new Point(578, 20);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(246, 45);
            descriptionLabel.TabIndex = 33;
            descriptionLabel.Text = "Part Description";
            // 
            // textBoxDescription
            // 
            textBoxDescription.Location = new Point(560, 85);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.Size = new Size(272, 179);
            textBoxDescription.TabIndex = 34;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1606, 1039);
            Controls.Add(textBoxDescription);
            Controls.Add(descriptionLabel);
            Controls.Add(labelRarity);
            Controls.Add(numRarity);
            Controls.Add(behaviorLabel);
            Controls.Add(chkBehaviors);
            Controls.Add(txtProjDirPath);
            Controls.Add(projDirSelectButton);
            Controls.Add(ctSelectButton);
            Controls.Add(txtContentPath);
            Controls.Add(isHead);
            Controls.Add(baseSelection);
            Controls.Add(btnSelectTexture);
            Controls.Add(label10);
            Controls.Add(btnAdd);
            Controls.Add(label9);
            Controls.Add(numRange);
            Controls.Add(numHealth);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(numCritical);
            Controls.Add(label6);
            Controls.Add(numArmor);
            Controls.Add(numSpeed);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(numDamage);
            Controls.Add(lstParts);
            Controls.Add(label3);
            Controls.Add(btnLoad);
            Controls.Add(btnSave);
            Controls.Add(picPreview);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtName);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDamage).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSpeed).EndInit();
            ((System.ComponentModel.ISupportInitialize)numArmor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCritical).EndInit();
            ((System.ComponentModel.ISupportInitialize)numHealth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRarity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtName;
        private Label label1;
        private Label label2;
        private PictureBox picPreview;
        private Button btnSave;
        private Button btnLoad;
        private Label label3;
        private ListBox lstParts;
        private NumericUpDown numDamage;
        private Label label4;
        private Label label5;
        private NumericUpDown numSpeed;
        private NumericUpDown numArmor;
        private Label label6;
        private NumericUpDown numCritical;
        private Label label7;
        private Label label8;
        private NumericUpDown numHealth;
        private Label label9;
        private NumericUpDown numRange;
        private Button btnAdd;
        private Label label10;
        private Button btnSelectTexture;
        private Button baseSelection;
        private CheckBox isHead;
        private CheckedListBox chkBehaviors;
        private Label behaviorLabel;
        private TextBox txtContentPath;
        private Button ctSelectButton;
        private Button projDirSelectButton;
        private TextBox txtProjDirPath;
        private Label labelRarity;
        private NumericUpDown numRarity;
        private Label descriptionLabel;
        private TextBox textBoxDescription;
    }
}

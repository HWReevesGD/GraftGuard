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
            components = new System.ComponentModel.Container();
            txtName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            imageList1 = new ImageList(components);
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
            btnUpdate = new Button();
            label9 = new Label();
            numRange = new NumericUpDown();
            btnAdd = new Button();
            label10 = new Label();
            btnSelectTexture = new Button();
            baseSelection = new Button();
            isHead = new CheckBox();
            chkBehaviors = new CheckedListBox();
            label11 = new Label();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDamage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSpeed).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numArmor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCritical).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numHealth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRange).BeginInit();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Location = new Point(229, 355);
            txtName.Margin = new Padding(4);
            txtName.Name = "txtName";
            txtName.Size = new Size(302, 39);
            txtName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F);
            label1.Location = new Point(35, 10);
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
            label2.Location = new Point(60, 355);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(141, 37);
            label2.TabIndex = 2;
            label2.Text = "Part Name";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // picPreview
            // 
            picPreview.Location = new Point(979, 82);
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
            btnSave.Location = new Point(60, 925);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(144, 42);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(229, 925);
            btnLoad.Margin = new Padding(4);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(144, 42);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(979, 857);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(248, 32);
            label3.TabIndex = 6;
            label3.Text = "Active Base: Unknown";
            // 
            // lstParts
            // 
            lstParts.FormattingEnabled = true;
            lstParts.Location = new Point(60, 154);
            lstParts.Margin = new Padding(4);
            lstParts.Name = "lstParts";
            lstParts.Size = new Size(471, 164);
            lstParts.TabIndex = 7;
            lstParts.SelectedIndexChanged += lstParts_SelectedIndexChanged;
            lstParts.KeyDown += lstParts_KeyDown;
            // 
            // numDamage
            // 
            numDamage.Location = new Point(265, 438);
            numDamage.Margin = new Padding(4);
            numDamage.Name = "numDamage";
            numDamage.Size = new Size(268, 39);
            numDamage.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(60, 438);
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
            label5.Location = new Point(60, 515);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(199, 37);
            label5.TabIndex = 10;
            label5.Text = "Speed Modifier";
            // 
            // numSpeed
            // 
            numSpeed.Location = new Point(265, 516);
            numSpeed.Margin = new Padding(4);
            numSpeed.Name = "numSpeed";
            numSpeed.Size = new Size(268, 39);
            numSpeed.TabIndex = 11;
            // 
            // numArmor
            // 
            numArmor.Location = new Point(265, 599);
            numArmor.Margin = new Padding(4);
            numArmor.Name = "numArmor";
            numArmor.Size = new Size(268, 39);
            numArmor.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.Location = new Point(60, 598);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(199, 37);
            label6.TabIndex = 13;
            label6.Text = "Armor Modifier";
            // 
            // numCritical
            // 
            numCritical.Location = new Point(265, 727);
            numCritical.Margin = new Padding(4);
            numCritical.Name = "numCritical";
            numCritical.Size = new Size(268, 39);
            numCritical.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.Location = new Point(60, 727);
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
            label8.Location = new Point(60, 796);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(203, 37);
            label8.TabIndex = 16;
            label8.Text = "Health Modifier";
            // 
            // numHealth
            // 
            numHealth.Location = new Point(265, 797);
            numHealth.Margin = new Padding(4);
            numHealth.Name = "numHealth";
            numHealth.Size = new Size(268, 39);
            numHealth.TabIndex = 17;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(386, 925);
            btnUpdate.Margin = new Padding(4);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(144, 42);
            btnUpdate.TabIndex = 19;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 10F);
            label9.Location = new Point(60, 662);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(200, 37);
            label9.TabIndex = 21;
            label9.Text = "Range Modifier";
            // 
            // numRange
            // 
            numRange.Location = new Point(265, 662);
            numRange.Margin = new Padding(4);
            numRange.Name = "numRange";
            numRange.Size = new Size(268, 39);
            numRange.TabIndex = 20;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(60, 979);
            btnAdd.Margin = new Padding(4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(473, 42);
            btnAdd.TabIndex = 22;
            btnAdd.Text = "Add Part";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(60, 861);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(101, 37);
            label10.TabIndex = 23;
            label10.Text = "Texture";
            // 
            // btnSelectTexture
            // 
            btnSelectTexture.Location = new Point(229, 860);
            btnSelectTexture.Margin = new Padding(4);
            btnSelectTexture.Name = "btnSelectTexture";
            btnSelectTexture.Size = new Size(303, 42);
            btnSelectTexture.TabIndex = 24;
            btnSelectTexture.Text = "Select Texture";
            btnSelectTexture.UseVisualStyleBackColor = true;
            btnSelectTexture.Click += btnSelectTexture_Click;
            // 
            // baseSelection
            // 
            baseSelection.Location = new Point(1387, 850);
            baseSelection.Margin = new Padding(5, 6, 5, 6);
            baseSelection.Name = "baseSelection";
            baseSelection.Size = new Size(246, 49);
            baseSelection.TabIndex = 25;
            baseSelection.Text = "Select Base/Torso";
            baseSelection.UseVisualStyleBackColor = true;
            baseSelection.Click += baseSelection_Click;
            // 
            // isHead
            // 
            isHead.AutoSize = true;
            isHead.Location = new Point(395, 82);
            isHead.Margin = new Padding(4);
            isHead.Name = "isHead";
            isHead.Size = new Size(136, 36);
            isHead.TabIndex = 26;
            isHead.Text = "Is Head?";
            isHead.UseVisualStyleBackColor = true;
            // 
            // chkBehaviors
            // 
            chkBehaviors.CheckOnClick = true;
            chkBehaviors.FormattingEnabled = true;
            chkBehaviors.Location = new Point(560, 318);
            chkBehaviors.Margin = new Padding(12);
            chkBehaviors.Name = "chkBehaviors";
            chkBehaviors.Size = new Size(383, 580);
            chkBehaviors.TabIndex = 27;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Segoe UI", 12F);
            label11.Location = new Point(633, 270);
            label11.Name = "label11";
            label11.Size = new Size(220, 45);
            label11.TabIndex = 28;
            label11.Text = "Part Behaviors";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1710, 1039);
            Controls.Add(label11);
            Controls.Add(chkBehaviors);
            Controls.Add(isHead);
            Controls.Add(baseSelection);
            Controls.Add(btnSelectTexture);
            Controls.Add(label10);
            Controls.Add(btnAdd);
            Controls.Add(label9);
            Controls.Add(numRange);
            Controls.Add(btnUpdate);
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
            Margin = new Padding(4);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtName;
        private Label label1;
        private Label label2;
        private ImageList imageList1;
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
        private Button btnUpdate;
        private Label label9;
        private NumericUpDown numRange;
        private Button btnAdd;
        private Label label10;
        private Button btnSelectTexture;
        private Button baseSelection;
        private CheckBox isHead;
        private CheckedListBox chkBehaviors;
        private Label label11;
    }
}

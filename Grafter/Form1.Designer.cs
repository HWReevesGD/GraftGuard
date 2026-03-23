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
            label11 = new Label();
            txtContentPath = new TextBox();
            ctSelectButton = new Button();
            projDirSelectButton = new Button();
            txtProjDirPath = new TextBox();
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
            txtName.Location = new Point(123, 166);
            txtName.Margin = new Padding(2);
            txtName.Name = "txtName";
            txtName.Size = new Size(164, 23);
            txtName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F);
            label1.Location = new Point(19, 5);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(175, 65);
            label1.TabIndex = 1;
            label1.Text = "Grafter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(32, 166);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(74, 19);
            label2.TabIndex = 2;
            label2.Text = "Part Name";
            // 
            // picPreview
            // 
            picPreview.Location = new Point(349, 40);
            picPreview.Margin = new Padding(2);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(352, 341);
            picPreview.TabIndex = 3;
            picPreview.TabStop = false;
            picPreview.Paint += picPreview_Paint;
            picPreview.MouseClick += picPreview_MouseClick;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(32, 434);
            btnSave.Margin = new Padding(2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(125, 20);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(162, 434);
            btnLoad.Margin = new Padding(2);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(125, 20);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(349, 404);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(124, 15);
            label3.TabIndex = 6;
            label3.Text = "Active Base: Unknown";
            // 
            // lstParts
            // 
            lstParts.FormattingEnabled = true;
            lstParts.ItemHeight = 15;
            lstParts.Location = new Point(32, 72);
            lstParts.Margin = new Padding(2);
            lstParts.Name = "lstParts";
            lstParts.Size = new Size(255, 79);
            lstParts.TabIndex = 7;
            lstParts.SelectedIndexChanged += lstParts_SelectedIndexChanged;
            lstParts.KeyDown += lstParts_KeyDown;
            // 
            // numDamage
            // 
            numDamage.Location = new Point(143, 205);
            numDamage.Margin = new Padding(2);
            numDamage.Name = "numDamage";
            numDamage.Size = new Size(144, 23);
            numDamage.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(32, 205);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(92, 19);
            label4.TabIndex = 9;
            label4.Text = "Base Damage";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(32, 241);
            label5.Margin = new Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new Size(101, 19);
            label5.TabIndex = 10;
            label5.Text = "Speed Modifier";
            // 
            // numSpeed
            // 
            numSpeed.Location = new Point(143, 242);
            numSpeed.Margin = new Padding(2);
            numSpeed.Name = "numSpeed";
            numSpeed.Size = new Size(144, 23);
            numSpeed.TabIndex = 11;
            // 
            // numArmor
            // 
            numArmor.Location = new Point(143, 281);
            numArmor.Margin = new Padding(2);
            numArmor.Name = "numArmor";
            numArmor.Size = new Size(144, 23);
            numArmor.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.Location = new Point(32, 280);
            label6.Margin = new Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new Size(103, 19);
            label6.TabIndex = 13;
            label6.Text = "Armor Modifier";
            // 
            // numCritical
            // 
            numCritical.Location = new Point(143, 341);
            numCritical.Margin = new Padding(2);
            numCritical.Name = "numCritical";
            numCritical.Size = new Size(144, 23);
            numCritical.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.Location = new Point(32, 341);
            label7.Margin = new Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new Size(105, 19);
            label7.TabIndex = 15;
            label7.Text = "Critical Modifier";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 10F);
            label8.Location = new Point(32, 373);
            label8.Margin = new Padding(2, 0, 2, 0);
            label8.Name = "label8";
            label8.Size = new Size(104, 19);
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
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 10F);
            label9.Location = new Point(32, 310);
            label9.Margin = new Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new Size(102, 19);
            label9.TabIndex = 21;
            label9.Text = "Range Modifier";
            // 
            // numRange
            // 
            numRange.Location = new Point(143, 310);
            numRange.Margin = new Padding(2);
            numRange.Name = "numRange";
            numRange.Size = new Size(144, 23);
            numRange.TabIndex = 20;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(32, 459);
            btnAdd.Margin = new Padding(2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(255, 20);
            btnAdd.TabIndex = 22;
            btnAdd.Text = "Add Part";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(32, 404);
            label10.Margin = new Padding(2, 0, 2, 0);
            label10.Name = "label10";
            label10.Size = new Size(53, 19);
            label10.TabIndex = 23;
            label10.Text = "Texture";
            // 
            // btnSelectTexture
            // 
            btnSelectTexture.Location = new Point(123, 403);
            btnSelectTexture.Margin = new Padding(2);
            btnSelectTexture.Name = "btnSelectTexture";
            btnSelectTexture.Size = new Size(163, 20);
            btnSelectTexture.TabIndex = 24;
            btnSelectTexture.Text = "Select Texture";
            btnSelectTexture.UseVisualStyleBackColor = true;
            btnSelectTexture.Click += btnSelectTexture_Click;
            // 
            // baseSelection
            // 
            baseSelection.Location = new Point(569, 400);
            baseSelection.Name = "baseSelection";
            baseSelection.Size = new Size(132, 23);
            baseSelection.TabIndex = 25;
            baseSelection.Text = "Select Base/Torso";
            baseSelection.UseVisualStyleBackColor = true;
            baseSelection.Click += baseSelection_Click;
            // 
            // isHead
            // 
            isHead.AutoSize = true;
            isHead.Location = new Point(213, 38);
            isHead.Margin = new Padding(2);
            isHead.Name = "isHead";
            isHead.Size = new Size(70, 19);
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
            // txtContentPath
            // 
            txtContentPath.Location = new Point(349, 434);
            txtContentPath.Name = "txtContentPath";
            txtContentPath.Size = new Size(192, 23);
            txtContentPath.TabIndex = 27;
            // 
            // ctSelectButton
            // 
            ctSelectButton.Location = new Point(548, 434);
            ctSelectButton.Name = "ctSelectButton";
            ctSelectButton.Size = new Size(153, 23);
            ctSelectButton.TabIndex = 28;
            ctSelectButton.Text = "Select Content Folder";
            ctSelectButton.UseVisualStyleBackColor = true;
            ctSelectButton.Click += ctSelectButton_Click;
            // 
            // projDirSelectButton
            // 
            projDirSelectButton.Location = new Point(547, 459);
            projDirSelectButton.Name = "projDirSelectButton";
            projDirSelectButton.Size = new Size(153, 23);
            projDirSelectButton.TabIndex = 29;
            projDirSelectButton.Text = "Select Project Directory";
            projDirSelectButton.UseVisualStyleBackColor = true;
            projDirSelectButton.Click += mgcbSelectButton_Click;
            // 
            // txtProjDirPath
            // 
            txtProjDirPath.Location = new Point(349, 459);
            txtProjDirPath.Name = "txtProjDirPath";
            txtProjDirPath.Size = new Size(192, 23);
            txtProjDirPath.TabIndex = 30;
            // 
            // Form1
            //
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1710, 1039);
            Controls.Add(label11);
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
        private Label label11;
        private TextBox txtContentPath;
        private Button ctSelectButton;
        private Button projDirSelectButton;
        private TextBox txtProjDirPath;
    }
}

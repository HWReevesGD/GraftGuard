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
            txtName.Location = new Point(176, 277);
            txtName.Name = "txtName";
            txtName.Size = new Size(233, 31);
            txtName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F);
            label1.Location = new Point(27, 8);
            label1.Name = "label1";
            label1.Size = new Size(261, 96);
            label1.TabIndex = 1;
            label1.Text = "Grafter";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(46, 277);
            label2.Name = "label2";
            label2.Size = new Size(103, 28);
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
            picPreview.Location = new Point(499, 67);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(503, 568);
            picPreview.TabIndex = 3;
            picPreview.TabStop = false;
            picPreview.Paint += picPreview_Paint;
            picPreview.MouseClick += picPreview_MouseClick;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(46, 723);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(111, 33);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(176, 723);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(111, 33);
            btnLoad.TabIndex = 5;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(499, 673);
            label3.Name = "label3";
            label3.Size = new Size(185, 25);
            label3.TabIndex = 6;
            label3.Text = "Active Base: Unknown";
            // 
            // lstParts
            // 
            lstParts.FormattingEnabled = true;
            lstParts.ItemHeight = 25;
            lstParts.Location = new Point(46, 120);
            lstParts.Name = "lstParts";
            lstParts.Size = new Size(363, 129);
            lstParts.TabIndex = 7;
            lstParts.SelectedIndexChanged += lstParts_SelectedIndexChanged;
            lstParts.KeyDown += lstParts_KeyDown;
            // 
            // numDamage
            // 
            numDamage.Location = new Point(204, 342);
            numDamage.Name = "numDamage";
            numDamage.Size = new Size(206, 31);
            numDamage.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(46, 342);
            label4.Name = "label4";
            label4.Size = new Size(129, 28);
            label4.TabIndex = 9;
            label4.Text = "Base Damage";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F);
            label5.Location = new Point(46, 402);
            label5.Name = "label5";
            label5.Size = new Size(147, 28);
            label5.TabIndex = 10;
            label5.Text = "Speed Modifier";
            // 
            // numSpeed
            // 
            numSpeed.Location = new Point(204, 403);
            numSpeed.Name = "numSpeed";
            numSpeed.Size = new Size(206, 31);
            numSpeed.TabIndex = 11;
            // 
            // numArmor
            // 
            numArmor.Location = new Point(204, 468);
            numArmor.Name = "numArmor";
            numArmor.Size = new Size(206, 31);
            numArmor.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 10F);
            label6.Location = new Point(46, 467);
            label6.Name = "label6";
            label6.Size = new Size(148, 28);
            label6.TabIndex = 13;
            label6.Text = "Armor Modifier";
            // 
            // numCritical
            // 
            numCritical.Location = new Point(204, 568);
            numCritical.Name = "numCritical";
            numCritical.Size = new Size(206, 31);
            numCritical.TabIndex = 14;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 10F);
            label7.Location = new Point(46, 568);
            label7.Name = "label7";
            label7.Size = new Size(152, 28);
            label7.TabIndex = 15;
            label7.Text = "Critical Modifier";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 10F);
            label8.Location = new Point(46, 622);
            label8.Name = "label8";
            label8.Size = new Size(149, 28);
            label8.TabIndex = 16;
            label8.Text = "Health Modifier";
            // 
            // numHealth
            // 
            numHealth.Location = new Point(204, 623);
            numHealth.Name = "numHealth";
            numHealth.Size = new Size(206, 31);
            numHealth.TabIndex = 17;
            // 
            // btnUpdate
            // 
            btnUpdate.Location = new Point(297, 723);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(111, 33);
            btnUpdate.TabIndex = 19;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 10F);
            label9.Location = new Point(46, 517);
            label9.Name = "label9";
            label9.Size = new Size(147, 28);
            label9.TabIndex = 21;
            label9.Text = "Range Modifier";
            // 
            // numRange
            // 
            numRange.Location = new Point(204, 517);
            numRange.Name = "numRange";
            numRange.Size = new Size(206, 31);
            numRange.TabIndex = 20;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(46, 765);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(364, 33);
            btnAdd.TabIndex = 22;
            btnAdd.Text = "Add Part";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(46, 673);
            label10.Name = "label10";
            label10.Size = new Size(74, 28);
            label10.TabIndex = 23;
            label10.Text = "Texture";
            // 
            // btnSelectTexture
            // 
            btnSelectTexture.Location = new Point(176, 672);
            btnSelectTexture.Name = "btnSelectTexture";
            btnSelectTexture.Size = new Size(233, 33);
            btnSelectTexture.TabIndex = 24;
            btnSelectTexture.Text = "Select Texture";
            btnSelectTexture.UseVisualStyleBackColor = true;
            btnSelectTexture.Click += btnSelectTexture_Click;
            // 
            // baseSelection
            // 
            baseSelection.Location = new Point(813, 667);
            baseSelection.Margin = new Padding(4, 5, 4, 5);
            baseSelection.Name = "baseSelection";
            baseSelection.Size = new Size(189, 38);
            baseSelection.TabIndex = 25;
            baseSelection.Text = "Select Base/Torso";
            baseSelection.UseVisualStyleBackColor = true;
            baseSelection.Click += baseSelection_Click;
            // 
            // isHead
            // 
            isHead.AutoSize = true;
            isHead.Location = new Point(304, 64);
            isHead.Name = "isHead";
            isHead.Size = new Size(106, 29);
            isHead.TabIndex = 26;
            isHead.Text = "Is Head?";
            isHead.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1134, 812);
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
            Name = "Form1";
            Text = "Form1";
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
    }
}

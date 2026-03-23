namespace Grafter
{
    partial class BaseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            picPreview = new PictureBox();
            label1 = new Label();
            lstSockets = new ListBox();
            btnSelectTexture = new Button();
            label10 = new Label();
            btnAdd = new Button();
            btnLoad = new Button();
            btnSave = new Button();
            label2 = new Label();
            txtName = new TextBox();
            isTorso = new CheckBox();
            label3 = new Label();
            txtID = new TextBox();
            lstBases = new ListBox();
            label4 = new Label();
            addSocket = new Button();
            ((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
            SuspendLayout();
            // 
            // picPreview
            // 
            picPreview.Location = new Point(83, 89);
            picPreview.Margin = new Padding(2);
            picPreview.Name = "picPreview";
            picPreview.Size = new Size(352, 341);
            picPreview.TabIndex = 4;
            picPreview.TabStop = false;
            picPreview.Paint += picPreview_Paint;
            picPreview.MouseClick += picPreview_MouseClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 36F);
            label1.Location = new Point(11, 9);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(518, 65);
            label1.TabIndex = 5;
            label1.Text = "Grafter - Base Designer";
            // 
            // lstSockets
            // 
            lstSockets.FormattingEnabled = true;
            lstSockets.ItemHeight = 15;
            lstSockets.Location = new Point(24, 468);
            lstSockets.Margin = new Padding(2);
            lstSockets.Name = "lstSockets";
            lstSockets.Size = new Size(172, 109);
            lstSockets.TabIndex = 8;
            lstSockets.SelectedIndexChanged += lstSockets_SelectedIndexChanged;
            lstSockets.KeyDown += lstSockets_KeyDown;
            // 
            // btnSelectTexture
            // 
            btnSelectTexture.Location = new Point(329, 453);
            btnSelectTexture.Margin = new Padding(2);
            btnSelectTexture.Name = "btnSelectTexture";
            btnSelectTexture.Size = new Size(163, 20);
            btnSelectTexture.TabIndex = 31;
            btnSelectTexture.Text = "Select Texture";
            btnSelectTexture.UseVisualStyleBackColor = true;
            btnSelectTexture.Click += btnSelectTexture_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 10F);
            label10.Location = new Point(271, 453);
            label10.Margin = new Padding(2, 0, 2, 0);
            label10.Name = "label10";
            label10.Size = new Size(53, 19);
            label10.TabIndex = 30;
            label10.Text = "Texture";
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(24, 659);
            btnAdd.Margin = new Padding(2);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(172, 20);
            btnAdd.TabIndex = 29;
            btnAdd.Text = "Add Base";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(109, 634);
            btnLoad.Margin = new Padding(2);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(87, 20);
            btnLoad.TabIndex = 28;
            btnLoad.Text = "Load";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(24, 634);
            btnSave.Margin = new Padding(2);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(81, 20);
            btnSave.TabIndex = 27;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F);
            label2.Location = new Point(209, 481);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(115, 19);
            label2.TabIndex = 26;
            label2.Text = "Base/Torso Name";
            // 
            // txtName
            // 
            txtName.Location = new Point(328, 477);
            txtName.Margin = new Padding(2);
            txtName.Name = "txtName";
            txtName.Size = new Size(164, 23);
            txtName.TabIndex = 25;
            // 
            // isTorso
            // 
            isTorso.AutoSize = true;
            isTorso.Font = new Font("Segoe UI", 10F);
            isTorso.Location = new Point(431, 542);
            isTorso.Name = "isTorso";
            isTorso.Size = new Size(61, 23);
            isTorso.TabIndex = 32;
            isTorso.Text = "Torso";
            isTorso.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F);
            label3.Location = new Point(231, 515);
            label3.Margin = new Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new Size(93, 19);
            label3.TabIndex = 34;
            label3.Text = "Base/Torso ID";
            // 
            // txtID
            // 
            txtID.Location = new Point(328, 509);
            txtID.Margin = new Padding(2);
            txtID.Name = "txtID";
            txtID.Size = new Size(164, 23);
            txtID.TabIndex = 33;
            // 
            // lstBases
            // 
            lstBases.FormattingEnabled = true;
            lstBases.ItemHeight = 15;
            lstBases.Location = new Point(231, 570);
            lstBases.Margin = new Padding(2);
            lstBases.Name = "lstBases";
            lstBases.Size = new Size(261, 109);
            lstBases.TabIndex = 35;
            lstBases.SelectedIndexChanged += lstBases_SelectedIndexChanged;
            lstBases.KeyDown += lstBases_KeyDown;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F);
            label4.Location = new Point(24, 447);
            label4.Margin = new Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new Size(55, 19);
            label4.TabIndex = 36;
            label4.Text = "Sockets";
            // 
            // addSocket
            // 
            addSocket.Location = new Point(24, 595);
            addSocket.Margin = new Padding(2);
            addSocket.Name = "addSocket";
            addSocket.Size = new Size(172, 20);
            addSocket.TabIndex = 37;
            addSocket.Text = "Add Socket";
            addSocket.UseVisualStyleBackColor = true;
            addSocket.Click += addSocket_Click;
            // 
            // BaseForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(523, 694);
            Controls.Add(addSocket);
            Controls.Add(label4);
            Controls.Add(lstBases);
            Controls.Add(label3);
            Controls.Add(txtID);
            Controls.Add(isTorso);
            Controls.Add(btnSelectTexture);
            Controls.Add(label10);
            Controls.Add(btnAdd);
            Controls.Add(btnLoad);
            Controls.Add(btnSave);
            Controls.Add(label2);
            Controls.Add(txtName);
            Controls.Add(lstSockets);
            Controls.Add(label1);
            Controls.Add(picPreview);
            Name = "BaseForm";
            Text = "BaseForm";
            ((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox picPreview;
        private Label label1;
        private ListBox lstSockets;
        private Button btnSelectTexture;
        private Label label10;
        private Button btnAdd;
        private Button btnLoad;
        private Button btnSave;
        private Label label2;
        private TextBox txtName;
        private CheckBox isTorso;
        private Label label3;
        private TextBox txtID;
        private ListBox lstBases;
        private Label label4;
        private Button addSocket;
    }
}
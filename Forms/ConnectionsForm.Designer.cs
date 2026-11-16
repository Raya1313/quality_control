namespace WindowsFormsApp1.Forms
{
    partial class ConnectionsForm
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
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnManual = new System.Windows.Forms.Button();
            this.lblCall = new System.Windows.Forms.Label();
            this.lblHint = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnGroup = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numQty = new System.Windows.Forms.NumericUpDown();
            this.numWeight = new System.Windows.Forms.NumericUpDown();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.pnlEntry = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.numThickness = new System.Windows.Forms.NumericUpDown();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.dgvConns = new System.Windows.Forms.DataGridView();
            this.colWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThickness = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ConnectionCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlLink = new System.Windows.Forms.Panel();
            this.btnSaveLink = new System.Windows.Forms.Button();
            this.clbConnections = new System.Windows.Forms.CheckedListBox();
            this.lblLinkConns = new System.Windows.Forms.Label();
            this.cmbPart = new System.Windows.Forms.ComboBox();
            this.lblLinkPart = new System.Windows.Forms.Label();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            this.pnlEntry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConns)).BeginInit();
            this.pnlLink.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.Location = new System.Drawing.Point(308, 25);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(130, 23);
            this.btnDelete.TabIndex = 21;
            this.btnDelete.Text = "حذف اتصال";
            this.btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnManual
            // 
            this.btnManual.Location = new System.Drawing.Point(376, 65);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(226, 32);
            this.btnManual.TabIndex = 15;
            this.btnManual.Text = "روش اول (موردی)";
            this.btnManual.UseVisualStyleBackColor = true;
            // 
            // lblCall
            // 
            this.lblCall.AutoSize = true;
            this.lblCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCall.Location = new System.Drawing.Point(630, 65);
            this.lblCall.Name = "lblCall";
            this.lblCall.Size = new System.Drawing.Size(132, 25);
            this.lblCall.TabIndex = 14;
            this.lblCall.Text = "فراخوانی اتصالات";
            this.lblCall.Click += new System.EventHandler(this.lblCall_Click);
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Location = new System.Drawing.Point(371, 6);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(187, 16);
            this.lblHint.TabIndex = 20;
            this.lblHint.Text = "در صورت تایید لیست زیر، کلیک نمایید";
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lblHint);
            this.pnlBottom.Controls.Add(this.btnDelete);
            this.pnlBottom.Controls.Add(this.btnConfirm);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 399);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(834, 60);
            this.pnlBottom.TabIndex = 22;
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.SpringGreen;
            this.btnConfirm.Location = new System.Drawing.Point(470, 25);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(130, 23);
            this.btnConfirm.TabIndex = 19;
            this.btnConfirm.Text = "تایید کد اتصالات";
            this.btnConfirm.UseVisualStyleBackColor = false;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(72, 30);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 19;
            this.btnAdd.Text = "افزودن";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnGroup
            // 
            this.btnGroup.Location = new System.Drawing.Point(123, 64);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(226, 32);
            this.btnGroup.TabIndex = 16;
            this.btnGroup.Text = "روش دوم (گروهی) - Part List";
            this.btnGroup.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(562, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 16);
            this.label5.TabIndex = 33;
            this.label5.Text = "کد اتصال";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 16);
            this.label3.TabIndex = 31;
            this.label3.Text = "وزن";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(344, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 16);
            this.label1.TabIndex = 29;
            this.label1.Text = "تعداد";
            // 
            // numQty
            // 
            this.numQty.Location = new System.Drawing.Point(333, 30);
            this.numQty.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numQty.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numQty.Name = "numQty";
            this.numQty.Size = new System.Drawing.Size(59, 22);
            this.numQty.TabIndex = 26;
            this.numQty.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numWeight
            // 
            this.numWeight.DecimalPlaces = 3;
            this.numWeight.Location = new System.Drawing.Point(181, 31);
            this.numWeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numWeight.Name = "numWeight";
            this.numWeight.Size = new System.Drawing.Size(59, 22);
            this.numWeight.TabIndex = 28;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(543, 30);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(100, 22);
            this.txtCode.TabIndex = 23;
            // 
            // pnlEntry
            // 
            this.pnlEntry.Controls.Add(this.btnAdd);
            this.pnlEntry.Controls.Add(this.btnGroup);
            this.pnlEntry.Controls.Add(this.label1);
            this.pnlEntry.Controls.Add(this.label5);
            this.pnlEntry.Controls.Add(this.btnManual);
            this.pnlEntry.Controls.Add(this.lblCall);
            this.pnlEntry.Controls.Add(this.label3);
            this.pnlEntry.Controls.Add(this.label2);
            this.pnlEntry.Controls.Add(this.numWeight);
            this.pnlEntry.Controls.Add(this.numThickness);
            this.pnlEntry.Controls.Add(this.numQty);
            this.pnlEntry.Controls.Add(this.txtCode);
            this.pnlEntry.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEntry.Location = new System.Drawing.Point(0, 108);
            this.pnlEntry.Name = "pnlEntry";
            this.pnlEntry.Size = new System.Drawing.Size(834, 107);
            this.pnlEntry.TabIndex = 20;
            this.pnlEntry.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(254, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "ضخامت ورق";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // numThickness
            // 
            this.numThickness.DecimalPlaces = 2;
            this.numThickness.Location = new System.Drawing.Point(257, 30);
            this.numThickness.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numThickness.Name = "numThickness";
            this.numThickness.Size = new System.Drawing.Size(59, 22);
            this.numThickness.TabIndex = 27;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitle.Location = new System.Drawing.Point(400, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(123, 29);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "تعریف قطعات";
            // 
            // picLogo
            // 
            this.picLogo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picLogo.Location = new System.Drawing.Point(684, 4);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(150, 70);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // pnlBanner
            // 
            this.pnlBanner.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlBanner.Controls.Add(this.lblTitle);
            this.pnlBanner.Controls.Add(this.picLogo);
            this.pnlBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlBanner.Name = "pnlBanner";
            this.pnlBanner.Size = new System.Drawing.Size(834, 108);
            this.pnlBanner.TabIndex = 19;
            // 
            // dgvConns
            // 
            this.dgvConns.AllowUserToAddRows = false;
            this.dgvConns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvConns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWeight,
            this.colQty,
            this.colThickness,
            this.ConnectionCode,
            this.colRow});
            this.dgvConns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvConns.Location = new System.Drawing.Point(0, 0);
            this.dgvConns.MultiSelect = false;
            this.dgvConns.Name = "dgvConns";
            this.dgvConns.ReadOnly = true;
            this.dgvConns.RowHeadersWidth = 51;
            this.dgvConns.RowTemplate.Height = 24;
            this.dgvConns.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConns.Size = new System.Drawing.Size(834, 459);
            this.dgvConns.TabIndex = 21;
            // 
            // colWeight
            // 
            this.colWeight.DataPropertyName = "Weight";
            this.colWeight.HeaderText = "وزن";
            this.colWeight.MinimumWidth = 6;
            this.colWeight.Name = "colWeight";
            this.colWeight.ReadOnly = true;
            this.colWeight.Width = 125;
            // 
            // colQty
            // 
            this.colQty.DataPropertyName = "Qty";
            this.colQty.HeaderText = "تعداد";
            this.colQty.MinimumWidth = 6;
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            this.colQty.Width = 125;
            // 
            // colThickness
            // 
            this.colThickness.DataPropertyName = "Length";
            this.colThickness.HeaderText = "ضخامت";
            this.colThickness.MinimumWidth = 6;
            this.colThickness.Name = "colThickness";
            this.colThickness.ReadOnly = true;
            this.colThickness.Width = 125;
            // 
            // ConnectionCode
            // 
            this.ConnectionCode.DataPropertyName = "PartCode";
            this.ConnectionCode.HeaderText = "کد اتصال";
            this.ConnectionCode.MinimumWidth = 6;
            this.ConnectionCode.Name = "ConnectionCode";
            this.ConnectionCode.ReadOnly = true;
            this.ConnectionCode.Width = 125;
            // 
            // colRow
            // 
            this.colRow.HeaderText = "ردیف";
            this.colRow.MinimumWidth = 6;
            this.colRow.Name = "colRow";
            this.colRow.ReadOnly = true;
            this.colRow.Width = 125;
            // 
            // pnlLink
            // 
            this.pnlLink.Controls.Add(this.btnSaveLink);
            this.pnlLink.Controls.Add(this.clbConnections);
            this.pnlLink.Controls.Add(this.lblLinkConns);
            this.pnlLink.Controls.Add(this.cmbPart);
            this.pnlLink.Controls.Add(this.lblLinkPart);
            this.pnlLink.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLink.Location = new System.Drawing.Point(0, 271);
            this.pnlLink.Name = "pnlLink";
            this.pnlLink.Size = new System.Drawing.Size(834, 128);
            this.pnlLink.TabIndex = 23;
            // 
            // btnSaveLink
            // 
            this.btnSaveLink.Location = new System.Drawing.Point(405, 99);
            this.btnSaveLink.Name = "btnSaveLink";
            this.btnSaveLink.Size = new System.Drawing.Size(75, 23);
            this.btnSaveLink.TabIndex = 4;
            this.btnSaveLink.Text = "ذخیره اتصالات قطعه";
            this.btnSaveLink.UseVisualStyleBackColor = true;
            // 
            // clbConnections
            // 
            this.clbConnections.CheckOnClick = true;
            this.clbConnections.FormattingEnabled = true;
            this.clbConnections.Location = new System.Drawing.Point(123, 27);
            this.clbConnections.MultiColumn = true;
            this.clbConnections.Name = "clbConnections";
            this.clbConnections.Size = new System.Drawing.Size(154, 72);
            this.clbConnections.TabIndex = 3;
            // 
            // lblLinkConns
            // 
            this.lblLinkConns.AutoSize = true;
            this.lblLinkConns.Location = new System.Drawing.Point(285, 52);
            this.lblLinkConns.Name = "lblLinkConns";
            this.lblLinkConns.Size = new System.Drawing.Size(89, 16);
            this.lblLinkConns.TabIndex = 2;
            this.lblLinkConns.Text = "اتصالات این قطعه";
            this.lblLinkConns.Click += new System.EventHandler(this.label6_Click);
            // 
            // cmbPart
            // 
            this.cmbPart.FormattingEnabled = true;
            this.cmbPart.Location = new System.Drawing.Point(522, 44);
            this.cmbPart.Name = "cmbPart";
            this.cmbPart.Size = new System.Drawing.Size(121, 24);
            this.cmbPart.TabIndex = 1;
            // 
            // lblLinkPart
            // 
            this.lblLinkPart.AutoSize = true;
            this.lblLinkPart.Location = new System.Drawing.Point(681, 47);
            this.lblLinkPart.Name = "lblLinkPart";
            this.lblLinkPart.Size = new System.Drawing.Size(31, 16);
            this.lblLinkPart.TabIndex = 0;
            this.lblLinkPart.Text = "قطعه";
            // 
            // ConnectionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 459);
            this.Controls.Add(this.pnlLink);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlEntry);
            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.dgvConns);
            this.Name = "ConnectionsForm";
            this.Text = "ConnectionForm";
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            this.pnlEntry.ResumeLayout(false);
            this.pnlEntry.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThickness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlBanner.ResumeLayout(false);
            this.pnlBanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvConns)).EndInit();
            this.pnlLink.ResumeLayout(false);
            this.pnlLink.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Label lblCall;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numQty;
        private System.Windows.Forms.NumericUpDown numWeight;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Panel pnlEntry;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.DataGridView dgvConns;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numThickness;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThickness;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConnectionCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRow;
        private System.Windows.Forms.Panel pnlLink;
        private System.Windows.Forms.Button btnSaveLink;
        private System.Windows.Forms.CheckedListBox clbConnections;
        private System.Windows.Forms.Label lblLinkConns;
        private System.Windows.Forms.ComboBox cmbPart;
        private System.Windows.Forms.Label lblLinkPart;
    }
}
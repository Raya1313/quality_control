namespace WindowsFormsApp1.Forms
{
    partial class PartsForm
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
            this.pnlBanner = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.pnlEntry = new System.Windows.Forms.Panel();
            this.btnAdd1 = new System.Windows.Forms.Button();
            this.btnGroup = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnManual = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCall = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numWeight = new System.Windows.Forms.NumericUpDown();
            this.numLength = new System.Windows.Forms.NumericUpDown();
            this.numQty = new System.Windows.Forms.NumericUpDown();
            this.txtDrawingCode = new System.Windows.Forms.TextBox();
            this.txtPartName = new System.Windows.Forms.TextBox();
            this.txtPartCode = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.colRow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDrawing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParts = new System.Windows.Forms.DataGridView();
            this.pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.pnlEntry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
            this.pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBanner
            // 
            this.pnlBanner.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlBanner.Controls.Add(this.lblTitle);
            this.pnlBanner.Controls.Add(this.picLogo);
            this.pnlBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlBanner.Name = "pnlBanner";
            this.pnlBanner.Size = new System.Drawing.Size(862, 108);
            this.pnlBanner.TabIndex = 2;
            this.pnlBanner.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBanner_Paint);
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
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // picLogo
            // 
            this.picLogo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picLogo.Location = new System.Drawing.Point(712, 4);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(150, 70);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            this.picLogo.Click += new System.EventHandler(this.picLogo_Click);
            // 
            // pnlEntry
            // 
            this.pnlEntry.Controls.Add(this.btnAdd1);
            this.pnlEntry.Controls.Add(this.btnGroup);
            this.pnlEntry.Controls.Add(this.label2);
            this.pnlEntry.Controls.Add(this.label6);
            this.pnlEntry.Controls.Add(this.label5);
            this.pnlEntry.Controls.Add(this.btnManual);
            this.pnlEntry.Controls.Add(this.label4);
            this.pnlEntry.Controls.Add(this.lblCall);
            this.pnlEntry.Controls.Add(this.label3);
            this.pnlEntry.Controls.Add(this.label1);
            this.pnlEntry.Controls.Add(this.numWeight);
            this.pnlEntry.Controls.Add(this.numLength);
            this.pnlEntry.Controls.Add(this.numQty);
            this.pnlEntry.Controls.Add(this.txtDrawingCode);
            this.pnlEntry.Controls.Add(this.txtPartName);
            this.pnlEntry.Controls.Add(this.txtPartCode);
            this.pnlEntry.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEntry.Location = new System.Drawing.Point(0, 108);
            this.pnlEntry.Name = "pnlEntry";
            this.pnlEntry.Size = new System.Drawing.Size(862, 107);
            this.pnlEntry.TabIndex = 6;
            this.pnlEntry.Visible = true;
            this.pnlEntry.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlEntry_Paint);
            // 
            // btnAdd1
            // 
            this.btnAdd1.Location = new System.Drawing.Point(72, 30);
            this.btnAdd1.Name = "btnAdd1";
            this.btnAdd1.Size = new System.Drawing.Size(75, 23);
            this.btnAdd1.TabIndex = 19;
            this.btnAdd1.Text = "افزودن";
            this.btnAdd1.UseVisualStyleBackColor = true;
            this.btnAdd1.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnGroup
            // 
            this.btnGroup.Location = new System.Drawing.Point(123, 64);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(226, 32);
            this.btnGroup.TabIndex = 16;
            this.btnGroup.Text = "روش دوم (گروهی) - Assembly List";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 16);
            this.label2.TabIndex = 30;
            this.label2.Text = "طول";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(402, 11);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 16);
            this.label6.TabIndex = 34;
            this.label6.Text = "کد قطعه مطابق نقشه ها";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(562, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 16);
            this.label5.TabIndex = 33;
            this.label5.Text = "کد قطعه";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // btnManual
            // 
            this.btnManual.Location = new System.Drawing.Point(376, 65);
            this.btnManual.Name = "btnManual";
            this.btnManual.Size = new System.Drawing.Size(226, 32);
            this.btnManual.TabIndex = 15;
            this.btnManual.Text = "روش اول (موردی)";
            this.btnManual.UseVisualStyleBackColor = true;
            this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(673, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 16);
            this.label4.TabIndex = 32;
            this.label4.Text = "نام قطعه";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // lblCall
            // 
            this.lblCall.AutoSize = true;
            this.lblCall.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCall.Location = new System.Drawing.Point(630, 65);
            this.lblCall.Name = "lblCall";
            this.lblCall.Size = new System.Drawing.Size(120, 25);
            this.lblCall.TabIndex = 14;
            this.lblCall.Text = "فراخوانی قطعات";
            this.lblCall.Click += new System.EventHandler(this.lblCall_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(194, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 16);
            this.label3.TabIndex = 31;
            this.label3.Text = "وزن";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(346, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 16);
            this.label1.TabIndex = 29;
            this.label1.Text = "تعداد";
            this.label1.Click += new System.EventHandler(this.label1_Click);
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
            this.numWeight.ValueChanged += new System.EventHandler(this.numWeight_ValueChanged);
            // 
            // numLength
            // 
            this.numLength.DecimalPlaces = 2;
            this.numLength.Location = new System.Drawing.Point(257, 30);
            this.numLength.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numLength.Name = "numLength";
            this.numLength.Size = new System.Drawing.Size(59, 22);
            this.numLength.TabIndex = 27;
            this.numLength.ValueChanged += new System.EventHandler(this.numLength_ValueChanged);
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
            this.numQty.ValueChanged += new System.EventHandler(this.numQty_ValueChanged);
            // 
            // txtDrawingCode
            // 
            this.txtDrawingCode.Location = new System.Drawing.Point(411, 30);
            this.txtDrawingCode.Name = "txtDrawingCode";
            this.txtDrawingCode.Size = new System.Drawing.Size(100, 22);
            this.txtDrawingCode.TabIndex = 25;
            this.txtDrawingCode.TextChanged += new System.EventHandler(this.txtDrawingCode_TextChanged);
            // 
            // txtPartName
            // 
            this.txtPartName.Location = new System.Drawing.Point(650, 30);
            this.txtPartName.Name = "txtPartName";
            this.txtPartName.Size = new System.Drawing.Size(100, 22);
            this.txtPartName.TabIndex = 24;
            this.txtPartName.TextChanged += new System.EventHandler(this.txtPartName_TextChanged);
            // 
            // txtPartCode
            // 
            this.txtPartCode.Location = new System.Drawing.Point(531, 30);
            this.txtPartCode.Name = "txtPartCode";
            this.txtPartCode.Size = new System.Drawing.Size(100, 22);
            this.txtPartCode.TabIndex = 23;
            this.txtPartCode.TextChanged += new System.EventHandler(this.txtPartCode_TextChanged);
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.SpringGreen;
            this.btnConfirm.Location = new System.Drawing.Point(470, 25);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(130, 23);
            this.btnConfirm.TabIndex = 19;
            this.btnConfirm.Text = "تایید نام و کد قطعات";
            this.btnConfirm.UseVisualStyleBackColor = false;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.Transparent;
            this.btnDelete.Location = new System.Drawing.Point(308, 25);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(130, 23);
            this.btnDelete.TabIndex = 21;
            this.btnDelete.Text = "حذف قطعه";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Location = new System.Drawing.Point(371, 6);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(187, 16);
            this.lblHint.TabIndex = 20;
            this.lblHint.Text = "در صورت تایید لیست زیر، کلیک نمایید";
            this.lblHint.Click += new System.EventHandler(this.label7_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lblHint);
            this.pnlBottom.Controls.Add(this.btnDelete);
            this.pnlBottom.Controls.Add(this.btnConfirm);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 408);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(862, 60);
            this.pnlBottom.TabIndex = 18;
            this.pnlBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBottom_Paint);
            // 
            // colRow
            // 
            this.colRow.HeaderText = "ردیف";
            this.colRow.MinimumWidth = 6;
            this.colRow.Name = "colRow";
            this.colRow.ReadOnly = true;
            this.colRow.Width = 125;
            // 
            // colDrawing
            // 
            this.colDrawing.DataPropertyName = "DrawingCode";
            this.colDrawing.HeaderText = "کد قطعه مطابق نقشه ها";
            this.colDrawing.MinimumWidth = 6;
            this.colDrawing.Name = "colDrawing";
            this.colDrawing.ReadOnly = true;
            this.colDrawing.Width = 125;
            // 
            // colName
            // 
            this.colName.DataPropertyName = "PartName";
            this.colName.HeaderText = "نام قطعه";
            this.colName.MinimumWidth = 6;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 125;
            // 
            // colCode
            // 
            this.colCode.DataPropertyName = "PartCode";
            this.colCode.HeaderText = "کد قطعه";
            this.colCode.MinimumWidth = 6;
            this.colCode.Name = "colCode";
            this.colCode.ReadOnly = true;
            this.colCode.Width = 125;
            // 
            // colQty
            // 
            this.colQty.DataPropertyName = "Qty";
            this.colQty.HeaderText = "کل تعداد";
            this.colQty.MinimumWidth = 6;
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            this.colQty.Width = 125;
            // 
            // colLength
            // 
            this.colLength.DataPropertyName = "Length";
            this.colLength.HeaderText = "طول";
            this.colLength.MinimumWidth = 6;
            this.colLength.Name = "colLength";
            this.colLength.ReadOnly = true;
            this.colLength.Width = 125;
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
            // dgvParts
            // 
            this.dgvParts.AllowUserToAddRows = false;
            this.dgvParts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colWeight,
            this.colLength,
            this.colQty,
            this.colCode,
            this.colName,
            this.colDrawing,
            this.colRow});
            this.dgvParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParts.Location = new System.Drawing.Point(0, 0);
            this.dgvParts.MultiSelect = false;
            this.dgvParts.Name = "dgvParts";
            this.dgvParts.ReadOnly = true;
            this.dgvParts.RowHeadersWidth = 51;
            this.dgvParts.RowTemplate.Height = 24;
            this.dgvParts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvParts.Size = new System.Drawing.Size(862, 408);
            this.dgvParts.TabIndex = 17;
            this.dgvParts.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // PartsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(862, 468);
            this.Controls.Add(this.pnlEntry);
            this.Controls.Add(this.pnlBanner);
            this.Controls.Add(this.dgvParts);
            this.Controls.Add(this.pnlBottom);
            this.Name = "PartsForm";
            this.Text = "Form1";
            this.pnlBanner.ResumeLayout(false);
            this.pnlBanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlEntry.ResumeLayout(false);
            this.pnlEntry.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParts)).EndInit();
            this.ResumeLayout(false);
            // Fill-docked grid must be docked last (z-index 0), otherwise it lies under the top panels
            this.dgvParts.BringToFront();

        }

        #endregion
        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Panel pnlEntry;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numWeight;
        private System.Windows.Forms.NumericUpDown numLength;
        private System.Windows.Forms.NumericUpDown numQty;
        private System.Windows.Forms.TextBox txtDrawingCode;
        private System.Windows.Forms.TextBox txtPartName;
        private System.Windows.Forms.TextBox txtPartCode;
        private System.Windows.Forms.Button btnAdd1;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.Button btnManual;
        private System.Windows.Forms.Label lblCall;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDrawing;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeight;
        private System.Windows.Forms.DataGridView dgvParts;
    }
}
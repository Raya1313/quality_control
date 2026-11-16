namespace WindowsFormsApp1.Forms
{
    partial class BilingualBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutpnlBanner = new System.Windows.Forms.TableLayoutPanel();
            this.IbIFa = new System.Windows.Forms.Label();
            this.IbIEn = new System.Windows.Forms.Label();
            this.txtFa = new System.Windows.Forms.TextBox();
            this.txtEn = new System.Windows.Forms.TextBox();
            this.tableLayoutpnlBanner.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutpnlBanner
            // 
            this.tableLayoutpnlBanner.ColumnCount = 2;
            this.tableLayoutpnlBanner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutpnlBanner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutpnlBanner.Controls.Add(this.IbIFa, 0, 0);
            this.tableLayoutpnlBanner.Controls.Add(this.IbIEn, 0, 1);
            this.tableLayoutpnlBanner.Controls.Add(this.txtFa, 1, 0);
            this.tableLayoutpnlBanner.Controls.Add(this.txtEn, 1, 1);
            this.tableLayoutpnlBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutpnlBanner.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutpnlBanner.Name = "tableLayoutpnlBanner";
            this.tableLayoutpnlBanner.RowCount = 2;
            this.tableLayoutpnlBanner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutpnlBanner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutpnlBanner.Size = new System.Drawing.Size(270, 58);
            this.tableLayoutpnlBanner.TabIndex = 0;
            this.tableLayoutpnlBanner.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutpnlBanner_Paint);
            // 
            // IbIFa
            // 
            this.IbIFa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IbIFa.AutoSize = true;
            this.IbIFa.Location = new System.Drawing.Point(34, 0);
            this.IbIFa.Name = "IbIFa";
            this.IbIFa.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.IbIFa.Size = new System.Drawing.Size(23, 16);
            this.IbIFa.TabIndex = 0;
            this.IbIFa.Text = "Fa";
            this.IbIFa.Click += new System.EventHandler(this.IbIFa_Click);
            // 
            // IbIEn
            // 
            this.IbIEn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IbIEn.AutoSize = true;
            this.IbIEn.Location = new System.Drawing.Point(34, 29);
            this.IbIEn.Name = "IbIEn";
            this.IbIEn.Size = new System.Drawing.Size(23, 16);
            this.IbIEn.TabIndex = 1;
            this.IbIEn.Text = "En";
            this.IbIEn.Click += new System.EventHandler(this.IbIEn_Click);
            // 
            // txtFa
            // 
            this.txtFa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFa.Location = new System.Drawing.Point(63, 3);
            this.txtFa.Name = "txtFa";
            this.txtFa.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtFa.Size = new System.Drawing.Size(204, 22);
            this.txtFa.TabIndex = 2;
            // 
            // txtEn
            // 
            this.txtEn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEn.Location = new System.Drawing.Point(63, 32);
            this.txtEn.Name = "txtEn";
            this.txtEn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtEn.Size = new System.Drawing.Size(204, 22);
            this.txtEn.TabIndex = 3;
            // 
            // BilingualBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutpnlBanner);
            this.Name = "BilingualBox";
            this.Size = new System.Drawing.Size(270, 58);
            this.Load += new System.EventHandler(this.BilingualBox_Load);
            this.tableLayoutpnlBanner.ResumeLayout(false);
            this.tableLayoutpnlBanner.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutpnlBanner;
        private System.Windows.Forms.Label IbIFa;
        private System.Windows.Forms.Label IbIEn;
        private System.Windows.Forms.TextBox txtFa;
        private System.Windows.Forms.TextBox txtEn;
    }
}

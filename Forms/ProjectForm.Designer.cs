using System;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    partial class ProjectForm : Form
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
            this.btnLogo = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.bbName = new WindowsFormsApp1.Forms.BilingualBox();
            this.bbContractor = new WindowsFormsApp1.Forms.BilingualBox();
            this.bbClient = new WindowsFormsApp1.Forms.BilingualBox();
            this.bbQc = new WindowsFormsApp1.Forms.BilingualBox();
            this.bbQa = new WindowsFormsApp1.Forms.BilingualBox();
            this.bbCoordinator = new WindowsFormsApp1.Forms.BilingualBox();
            this.mtbContractDate = new System.Windows.Forms.MaskedTextBox();
            this.txtReportedPhase = new System.Windows.Forms.TextBox();
            this.lblPhaseCount = new System.Windows.Forms.Label();
            this.numPhases = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.tlp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPhases)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlBanner
            // 
            this.pnlBanner.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlBanner.Controls.Add(this.btnLogo);
            this.pnlBanner.Controls.Add(this.lblTitle);
            this.pnlBanner.Controls.Add(this.picLogo);
            this.pnlBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBanner.Location = new System.Drawing.Point(0, 0);
            this.pnlBanner.Name = "pnlBanner";
            this.pnlBanner.Size = new System.Drawing.Size(982, 100);
            this.pnlBanner.TabIndex = 0;
            this.pnlBanner.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBanner_Paint);
            // 
            // btnLogo
            // 
            this.btnLogo.Location = new System.Drawing.Point(867, 77);
            this.btnLogo.Name = "btnLogo";
            this.btnLogo.Size = new System.Drawing.Size(75, 23);
            this.btnLogo.TabIndex = 1;
            this.btnLogo.Text = "انتخاب لوگو";
            this.btnLogo.UseVisualStyleBackColor = true;
            this.btnLogo.Click += new System.EventHandler(this.btnLogo_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitle.Location = new System.Drawing.Point(401, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(217, 29);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "بخش اول (اطلاعات اولیه)";
            this.lblTitle.Click += new System.EventHandler(this.lblTitle_Click);
            // 
            // picLogo
            // 
            this.picLogo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.picLogo.Location = new System.Drawing.Point(832, 0);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(150, 70);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // tlp
            // 
            this.tlp.ColumnCount = 4;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlp.Controls.Add(this.label1, 0, 0);
            this.tlp.Controls.Add(this.label2, 0, 1);
            this.tlp.Controls.Add(this.label3, 0, 2);
            this.tlp.Controls.Add(this.label4, 0, 3);
            this.tlp.Controls.Add(this.label5, 2, 0);
            this.tlp.Controls.Add(this.label6, 2, 1);
            this.tlp.Controls.Add(this.label7, 2, 2);
            this.tlp.Controls.Add(this.label8, 2, 3);
            this.tlp.Controls.Add(this.bbName, 1, 0);
            this.tlp.Controls.Add(this.bbContractor, 1, 2);
            this.tlp.Controls.Add(this.bbClient, 1, 1);
            this.tlp.Controls.Add(this.bbQc, 3, 0);
            this.tlp.Controls.Add(this.bbQa, 3, 1);
            this.tlp.Controls.Add(this.bbCoordinator, 3, 2);
            this.tlp.Controls.Add(this.mtbContractDate, 1, 3);
            this.tlp.Controls.Add(this.txtReportedPhase, 3, 3);
            this.tlp.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlp.Location = new System.Drawing.Point(0, 100);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 4;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tlp.Size = new System.Drawing.Size(982, 286);
            this.tlp.TabIndex = 1;
            this.tlp.Paint += new System.Windows.Forms.PaintEventHandler(this.tlp_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(927, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "نام پروژه";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(913, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "\tنام کارفرما";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(867, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "نام پیمانکار (سازنده)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(887, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "تاریخ عقد قرارداد";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(384, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "مسئول کنترل کیفیت";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(382, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 16);
            this.label6.TabIndex = 5;
            this.label6.Text = "\tمسئول تضمین کیفیت";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(411, 140);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "هماهنگ کننده";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(401, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 16);
            this.label8.TabIndex = 7;
            this.label8.Text = "فاز یا بخش کار";
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // bbName
            // 
            this.bbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbName.En = "";
            this.bbName.Fa = "";
            this.bbName.Location = new System.Drawing.Point(494, 3);
            this.bbName.Name = "bbName";
            this.bbName.Size = new System.Drawing.Size(335, 64);
            this.bbName.TabIndex = 8;
            // 
            // bbContractor
            // 
            this.bbContractor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbContractor.En = "";
            this.bbContractor.Fa = "";
            this.bbContractor.Location = new System.Drawing.Point(494, 143);
            this.bbContractor.Name = "bbContractor";
            this.bbContractor.Size = new System.Drawing.Size(335, 64);
            this.bbContractor.TabIndex = 9;
            // 
            // bbClient
            // 
            this.bbClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbClient.En = "";
            this.bbClient.Fa = "";
            this.bbClient.Location = new System.Drawing.Point(494, 73);
            this.bbClient.Name = "bbClient";
            this.bbClient.Size = new System.Drawing.Size(335, 64);
            this.bbClient.TabIndex = 10;
            // 
            // bbQc
            // 
            this.bbQc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbQc.En = "";
            this.bbQc.Fa = "";
            this.bbQc.Location = new System.Drawing.Point(3, 3);
            this.bbQc.Name = "bbQc";
            this.bbQc.Size = new System.Drawing.Size(335, 64);
            this.bbQc.TabIndex = 11;
            // 
            // bbQa
            // 
            this.bbQa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbQa.En = "";
            this.bbQa.Fa = "";
            this.bbQa.Location = new System.Drawing.Point(3, 73);
            this.bbQa.Name = "bbQa";
            this.bbQa.Size = new System.Drawing.Size(335, 64);
            this.bbQa.TabIndex = 12;
            // 
            // bbCoordinator
            // 
            this.bbCoordinator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bbCoordinator.En = "";
            this.bbCoordinator.Fa = "";
            this.bbCoordinator.Location = new System.Drawing.Point(3, 143);
            this.bbCoordinator.Name = "bbCoordinator";
            this.bbCoordinator.Size = new System.Drawing.Size(335, 64);
            this.bbCoordinator.TabIndex = 13;
            this.bbCoordinator.Load += new System.EventHandler(this.bbCoordinator_Load);
            // 
            // mtbContractDate
            // 
            this.mtbContractDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mtbContractDate.Location = new System.Drawing.Point(494, 213);
            this.mtbContractDate.Mask = "0000\\/00\\/00";
            this.mtbContractDate.Name = "mtbContractDate";
            this.mtbContractDate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mtbContractDate.Size = new System.Drawing.Size(335, 22);
            this.mtbContractDate.TabIndex = 14;
            this.mtbContractDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtReportedPhase
            // 
            this.txtReportedPhase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReportedPhase.Location = new System.Drawing.Point(3, 213);
            this.txtReportedPhase.Name = "txtReportedPhase";
            this.txtReportedPhase.Size = new System.Drawing.Size(335, 22);
            this.txtReportedPhase.TabIndex = 15;
            // 
            // lblPhaseCount
            // 
            this.lblPhaseCount.AutoSize = true;
            this.lblPhaseCount.Location = new System.Drawing.Point(12, 413);
            this.lblPhaseCount.Name = "lblPhaseCount";
            this.lblPhaseCount.Size = new System.Drawing.Size(144, 16);
            this.lblPhaseCount.TabIndex = 2;
            this.lblPhaseCount.Text = "تعداد فاز / بخش های کاری";
            this.lblPhaseCount.Click += new System.EventHandler(this.lblPhaseCount_Click);
            // 
            // numPhases
            // 
            this.numPhases.Location = new System.Drawing.Point(183, 411);
            this.numPhases.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numPhases.Name = "numPhases";
            this.numPhases.Size = new System.Drawing.Size(120, 22);
            this.numPhases.TabIndex = 3;
            this.numPhases.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(496, 483);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 34);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "ذخیره";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // ProjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(982, 593);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numPhases);
            this.Controls.Add(this.lblPhaseCount);
            this.Controls.Add(this.tlp);
            this.Controls.Add(this.pnlBanner);
            this.Name = "ProjectForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "بخش اول (اطلاعات اولیه)";
            this.Load += new System.EventHandler(this.ProjectForm_Load);
            this.pnlBanner.ResumeLayout(false);
            this.pnlBanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.tlp.ResumeLayout(false);
            this.tlp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPhases)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        //private void btnLogo_Click(object sender, EventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

        private System.Windows.Forms.Panel pnlBanner;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Button btnLogo;
        private System.Windows.Forms.Label lblTitle;
        private TableLayoutPanel tlp;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private BilingualBox bbName;
        private BilingualBox bbContractor;
        private BilingualBox bbClient;
        private BilingualBox bbQc;
        private BilingualBox bbQa;
        private BilingualBox bbCoordinator;
        private MaskedTextBox mtbContractDate;
        private TextBox txtReportedPhase;
        private Label lblPhaseCount;
        private NumericUpDown numPhases;
        private Button btnSave;
    }
}
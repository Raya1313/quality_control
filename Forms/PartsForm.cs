using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using Dapper;
using Data;
using Microsoft.Data.SqlClient;

namespace WindowsFormsApp1.Forms
{
    public partial class PartsForm : Form
    {
        readonly BindingList<Part> _parts = new BindingList<Part>();
        readonly List<int> _deleted = new List<int>();

        public PartsForm()
        {
            InitializeComponent();

            dgvParts.AutoGenerateColumns = false;
            dgvParts.DataError += QcApp.Ui.GridError;
            dgvParts.DataSource = _parts;
            dgvParts.DataBindingComplete += (s, e) =>
            {
                for (int i = 0; i < dgvParts.Rows.Count; i++)
                    dgvParts.Rows[i].Cells[colRow.Index].Value = i + 1;
            };

            picLogo.Image = Session.LogoImage();
            LoadFromDb();
        }

        void LoadFromDb()
        {
            using (var c = Db.Open())
            {
                var rows = c.Query<Part>("SELECT * FROM Part WHERE ProjectId=@pid ORDER BY Id",
                                         new { pid = Session.ProjectId });
                foreach (var p in rows)
                {
                    p.IsSaved = true;
                    _parts.Add(p);
                }
            }
        }

        // PDF note 2: duplicate entry must not be possible
        bool TryAdd(Part p, out string err)
        {
            err = "";
            if (string.IsNullOrWhiteSpace(p.PartName))
            {
                err = "نام قطعه خالی است.";
                return false;
            }
            if (_parts.Any(x => string.Equals(x.PartName, p.PartName, StringComparison.OrdinalIgnoreCase)))
            {
                err = "قطعه " + p.PartName + " قبلاً ثبت شده است.";
                return false;
            }
            _parts.Add(p);
            return true;
        }

        // method 1: one by one
        void btnManual_Click(object sender, EventArgs e)
        {
            pnlEntry.Visible = true;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            var p = new Part
            {
                ProjectId = Session.ProjectId,
                PartName = txtPartName.Text.Trim(),
                PartCode = txtPartCode.Text.Trim(),
                DrawingCode = txtDrawingCode.Text.Trim(),
                Qty = (int)numQty.Value,
                Length = numLength.Value,
                Weight = numWeight.Value
            };

            string err;
            if (!TryAdd(p, out err))
            {
                Msg.Show(err);
                return;
            }

            txtPartName.Clear();
            txtPartCode.Clear();
            txtDrawingCode.Clear();
            txtPartName.Focus();
        }

        // method 2: Excel with columns name, quantity, length, weight (row 1 is a header)
        void btnGroup_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "Excel|*.xlsx" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                var errors = new List<string>();
                using (var wb = new XLWorkbook(dlg.FileName))
                {
                    var used = wb.Worksheet(1).RangeUsed();
                    if (used == null)
                    {
                        Msg.Show("فایل خالی است.");
                        return;
                    }

                    foreach (var r in used.RowsUsed().Skip(1))
                    {
                        int qty;
                        if (!r.Cell(2).TryGetValue<int>(out qty))
                        {
                            errors.Add("سطر " + r.RowNumber() + ": تعداد نامعتبر است.");
                            continue;
                        }

                        decimal len, wt;
                        var p = new Part
                        {
                            ProjectId = Session.ProjectId,
                            PartName = r.Cell(1).GetString().Trim(),
                            Qty = qty,
                            Length = r.Cell(3).TryGetValue<decimal>(out len) ? len : (decimal?)null,
                            Weight = r.Cell(4).TryGetValue<decimal>(out wt) ? wt : (decimal?)null
                        };

                        string err;
                        if (!TryAdd(p, out err))
                            errors.Add("سطر " + r.RowNumber() + ": " + err);
                    }
                }

                if (errors.Count > 0) Msg.Show(string.Join("\n", errors));
            }
        }

        // PDF note 1: parts can be removed from the list
        void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvParts.CurrentRow == null) return;
            var p = dgvParts.CurrentRow.DataBoundItem as Part;
            if (p == null) return;

            if (p.IsSaved) _deleted.Add(p.Id);
            _parts.Remove(p);
        }

        // green button: write the list to the database
        void btnConfirm_Click(object sender, EventArgs e)
        {
            using (var c = Db.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    foreach (var id in _deleted)
                        c.Execute("DELETE FROM Part WHERE Id=@id", new { id }, tx);

                    foreach (var p in _parts.Where(x => !x.IsSaved))
                        c.Execute(@"INSERT INTO Part (ProjectId,PartName,PartCode,DrawingCode,Qty,Length,Weight)
                                    VALUES (@ProjectId,@PartName,@PartCode,@DrawingCode,@Qty,@Length,@Weight)",
                                  p, tx);
                    tx.Commit();
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    tx.Rollback();
                    Msg.Show("قطعه تکراری در لیست وجود دارد.");
                    return;
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    tx.Rollback();
                    Msg.Show("این قطعه در یک گزارش بازرسی استفاده شده و قابل حذف نیست.");
                    _deleted.Clear();
                    _parts.Clear();
                    LoadFromDb();
                    return;
                }
            }

            _deleted.Clear();
            _parts.Clear();
            LoadFromDb();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void pnlEntry_Paint(object sender, PaintEventArgs e) { }
        private void lblTitle_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }

        private void picLogo_Click(object sender, EventArgs e)
        {

        }

        private void pnlBanner_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAdd1_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numWeight_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numLength_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numQty_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtDrawingCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPartName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPartCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblCall_Click(object sender, EventArgs e)
        {

        }

        private void btnManual_Click_1(object sender, EventArgs e)
        {

        }

        private void btnGroup_Click_1(object sender, EventArgs e)
        {

        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {

        }

        private void pnlBottom_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
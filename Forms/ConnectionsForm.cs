using ClosedXML.Excel;
using Dapper;
using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    public partial class ConnectionsForm : Form
    {
        readonly BindingList<Connection> _conns = new();
        readonly List<int> _deleted = new();

        public ConnectionsForm()
        {
            InitializeComponent();
            dgvConns.AutoGenerateColumns = false;
            dgvConns.DataSource = _conns;
            dgvConns.DataBindingComplete += (_, _) =>
            {
                for (int i = 0; i < dgvConns.Rows.Count; i++)
                    dgvConns.Rows[i].Cells[colRow.Index].Value = i + 1;
            };
            picLogo.Image = Session.LogoImage();
            LoadFromDb();
            LoadLinkPanel();
        }

        void LoadFromDb()
        {
            using var c = Db.Open();
            var rows = c.Query<Connection>("SELECT * FROM Connection WHERE ProjectId=@pid ORDER BY Id",
                                           new { pid = Session.ProjectId });
            foreach (var x in rows) { x.IsSaved = true; _conns.Add(x); }
        }

        bool TryAdd(Connection x, out string err)
        {
            err = "";
            if (string.IsNullOrWhiteSpace(x.ConnectionCode))
            { err = "کد اتصال خالی است."; return false; }
            if (_conns.Any(y => string.Equals(y.ConnectionCode, x.ConnectionCode, StringComparison.OrdinalIgnoreCase)))
            { err = $"اتصال {x.ConnectionCode} قبلاً ثبت شده است."; return false; }
            _conns.Add(x);
            return true;
        }

        void btnManual_Click(object sender, EventArgs e) => pnlEntry.Visible = true;

        void btnAdd_Click(object sender, EventArgs e)
        {
            var x = new Connection
            {
                ProjectId = Session.ProjectId,
                ConnectionCode = txtCode.Text.Trim(),
                PlateThickness = numThickness.Value,
                Qty = (int)numQty.Value,
                Weight = numWeight.Value
            };
            if (!TryAdd(x, out var err)) { Msg.Show(err); return; }
            txtCode.Clear();
            txtCode.Focus();
        }

        // Excel with columns name, quantity, weight (row 1 is a header)
        void btnGroup_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog { Filter = "Excel|*.xlsx" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            var errors = new List<string>();
            using var wb = new XLWorkbook(dlg.FileName);
            foreach (var r in wb.Worksheet(1).RangeUsed()!.RowsUsed().Skip(1))
            {
                var x = new Connection
                {
                    ProjectId = Session.ProjectId,
                    ConnectionCode = r.Cell(1).GetString().Trim(),
                    Qty = r.Cell(2).TryGetValue<int>(out var q) ? q : null,
                    Weight = r.Cell(3).TryGetValue<decimal>(out var w) ? w : null
                };
                if (!TryAdd(x, out var err)) errors.Add($"سطر {r.RowNumber()}: {err}");
            }
            if (errors.Count > 0) Msg.Show(string.Join("\n", errors));
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvConns.CurrentRow?.DataBoundItem is not Connection x) return;
            if (x.IsSaved) _deleted.Add(x.Id);
            _conns.Remove(x);
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            using var c = Db.Open();
            using var tx = c.BeginTransaction();
            try
            {
                foreach (var id in _deleted)
                {
                    c.Execute("DELETE FROM PartConnection WHERE ConnectionId=@id", new { id }, tx);
                    c.Execute("DELETE FROM Connection WHERE Id=@id", new { id }, tx);
                }
                foreach (var x in _conns.Where(y => !y.IsSaved))
                    c.Execute(@"INSERT INTO Connection (ProjectId,ConnectionCode,PlateThickness,Qty,Weight)
                            VALUES (@ProjectId,@ConnectionCode,@PlateThickness,@Qty,@Weight)", x, tx);
                tx.Commit();
            }
            catch (SqlException ex) when (ex.Number is 2627 or 2601)
            {
                tx.Rollback();
                Msg.Show("اتصال تکراری در لیست وجود دارد.");
                return;
            }

            _deleted.Clear();
            _conns.Clear();
            LoadFromDb();
            LoadLinkPanel();
        }

        List<Connection> _all = new();

        void LoadLinkPanel()
        {
            using var c = Db.Open();
            var pid = new { pid = Session.ProjectId };

            _all = c.Query<Connection>(
                "SELECT Id, ConnectionCode FROM Connection WHERE ProjectId=@pid ORDER BY ConnectionCode", pid).ToList();
            clbConnections.DataSource = _all;
            clbConnections.DisplayMember = "ConnectionCode";

            cmbPart.SelectedIndexChanged -= cmbPart_SelectedIndexChanged;
            cmbPart.DataSource = c.Query<Part>(
                "SELECT Id, PartName FROM Part WHERE ProjectId=@pid ORDER BY PartName", pid).ToList();
            cmbPart.DisplayMember = "PartName";
            cmbPart.ValueMember = "Id";
            cmbPart.SelectedIndexChanged += cmbPart_SelectedIndexChanged;
            ShowLinks();
        }

        void cmbPart_SelectedIndexChanged(object? sender, EventArgs e) => ShowLinks();

        void ShowLinks()
        {
            if (cmbPart.SelectedValue is not int partId) return;
            using var c = Db.Open();
            var linked = c.Query<int>("SELECT ConnectionId FROM PartConnection WHERE PartId=@partId",
                                      new { partId }).ToHashSet();
            for (int i = 0; i < _all.Count; i++)
                clbConnections.SetItemChecked(i, linked.Contains(_all[i].Id));
        }

        void btnSaveLink_Click(object sender, EventArgs e)
        {
            if (cmbPart.SelectedValue is not int partId) return;
            using var c = Db.Open();
            using var tx = c.BeginTransaction();
            c.Execute("DELETE FROM PartConnection WHERE PartId=@partId", new { partId }, tx);
            foreach (int i in clbConnections.CheckedIndices)
                c.Execute("INSERT INTO PartConnection (PartId, ConnectionId) VALUES (@partId, @cid)",
                          new { partId, cid = _all[i].Id }, tx);
            tx.Commit();
        }

        private void dgvConns_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void pnlBanner_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnlEntry_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblCall_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}

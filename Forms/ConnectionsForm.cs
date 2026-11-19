using ClosedXML.Excel;
using Dapper;
using Data;
using Microsoft.Data.SqlClient;
using QcApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // Connections (Part List): add one by one or from Excel, delete, and link each part
    // to its connections. Everything is written to the database right away.
    public partial class ConnectionsForm : Form
    {
        public class Row
        {
            public int Id { get; set; }
            public string ConnectionCode { get; set; } = "";
            public decimal? PlateThickness { get; set; }
            public int? Qty { get; set; }
            public decimal? Weight { get; set; }
            public string? LinkedParts { get; set; }
        }

        readonly BindingList<Row> _rows = new BindingList<Row>();
        readonly DataGridView _grid = Ui.Grid();


        readonly TextBox _code = new TextBox();
        readonly NumericUpDown _thick = new NumericUpDown { DecimalPlaces = 1, Increment = 0.5m, Minimum = 0, Maximum = 500 };
        readonly NumericUpDown _qty = new NumericUpDown { Minimum = 1, Maximum = 100000, Value = 1 };
        readonly NumericUpDown _weight = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, Maximum = 1000000 };

        readonly ComboBox _cmbPart = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        readonly CheckedListBox _clb = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true };
        bool _loading;

        public ConnectionsForm()
        {
            Ui.Setup(this, "تعریف اتصالات (Part List)", 1250, 720);

            var add = Ui.Btn("افزودن", 110);
            var excel = Ui.Btn("فراخوانی گروهی از اکسل", 200);
            add.Margin = new Padding(6, 24, 6, 6);
            excel.Margin = new Padding(6, 24, 6, 6);
            add.Click += (s, e) => AddOne();
            excel.Click += (s, e) => ImportExcel();

            var delete = Ui.Btn("حذف اتصال انتخاب شده", 200);
            var close = Ui.Btn("بستن", 140);
            delete.Click += (s, e) => DeleteSelected();
            close.Click += (s, e) => Close();

            var root = Ui.Root(this, Ui.Abs(90), Ui.Abs(96), Ui.Fill(), Ui.Abs(56));

            PictureBox logo;
            root.Controls.Add(Ui.Banner("بخش اول (تعریف اتصالات)", out logo, null), 0, 0);

            var entry = new FlowLayoutPanel { Dock = DockStyle.Fill, RightToLeft = RightToLeft.Yes, Padding = new Padding(8) };
            entry.Controls.Add(Ui.Field("کد اتصال", _code, 150));
            entry.Controls.Add(Ui.Field("ضخامت ورق (mm)", _thick, 110));
            entry.Controls.Add(Ui.Field("تعداد", _qty, 90));
            entry.Controls.Add(Ui.Field("وزن (kg)", _weight, 110));
            entry.Controls.Add(add);
            entry.Controls.Add(excel);
            root.Controls.Add(entry, 0, 1);

            var body = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            root.Controls.Add(body, 0, 2);

            Ui.AddCol(_grid, "ردیف", "");
            Ui.AddCol(_grid, "کد اتصال", "ConnectionCode");
            Ui.AddCol(_grid, "ضخامت ورق", "PlateThickness");
            Ui.AddCol(_grid, "تعداد", "Qty");
            Ui.AddCol(_grid, "وزن", "Weight");
            Ui.AddCol(_grid, "قطعات مرتبط", "LinkedParts");
            _grid.DataSource = _rows;

            _grid.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 0)
                    return;

                e.Value = (e.RowIndex + 1).ToString();
                e.FormattingApplied = true;
            };
            body.Controls.Add(_grid, 0, 0);
            body.Controls.Add(BuildLinkPanel(), 1, 0);

            root.Controls.Add(Ui.BottomBar(delete, "هر تغییر بلافاصله در پایگاه داده ذخیره می‌شود", close), 0, 3);

            LoadParts();
            LoadAll();
        }

        Control BuildLinkPanel()
        {
            var p = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(8) };
            p.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
            p.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            p.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            p.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            p.Controls.Add(new Label
            {
                Text = "لینک اتصالات به قطعه: یک قطعه را انتخاب کنید، اتصالات آن را تیک بزنید و ذخیره کنید.",
                Dock = DockStyle.Fill,
                Font = new Font("Tahoma", 9f, FontStyle.Bold)
            }, 0, 0);
            p.Controls.Add(_cmbPart, 0, 1);
            p.Controls.Add(_clb, 0, 2);

            var save = Ui.Btn("ذخیره لینک‌ها", 130);
            var all = Ui.Btn("همه", 70);
            var none = Ui.Btn("هیچ", 70);
            save.Click += (s, e) => SaveLinks();
            all.Click += (s, e) => { for (int i = 0; i < _clb.Items.Count; i++) _clb.SetItemChecked(i, true); };
            none.Click += (s, e) => { for (int i = 0; i < _clb.Items.Count; i++) _clb.SetItemChecked(i, false); };
            var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, RightToLeft = RightToLeft.Yes };
            bar.Controls.Add(save);
            bar.Controls.Add(all);
            bar.Controls.Add(none);
            p.Controls.Add(bar, 0, 3);

            _cmbPart.SelectedIndexChanged += (s, e) => { if (!_loading) ShowLinks(); };
            return p;
        }

        // ---- loading --------------------------------------------------------

        void LoadParts()
        {
            using (var c = Db.Open())
            {
                var parts = c.Query<Part>("SELECT * FROM Part WHERE ProjectId=@pid ORDER BY PartName",
                                          new { pid = Session.ProjectId }).ToList();
                _loading = true;
                _cmbPart.DataSource = parts;
                _cmbPart.DisplayMember = "PartName";
                _loading = false;
            }
        }

        void LoadAll()
        {
            using (var c = Db.Open())
            {
                var rows = c.Query<Row>(@"
                    SELECT k.Id, k.ConnectionCode, k.PlateThickness, k.Qty, k.Weight,
                           STUFF((SELECT N', ' + p.PartName
                                  FROM PartConnection pc JOIN Part p ON p.Id = pc.PartId
                                  WHERE pc.ConnectionId = k.Id
                                  ORDER BY p.PartName
                                  FOR XML PATH(N'')), 1, 2, N'') AS LinkedParts
                    FROM Connection k
                    WHERE k.ProjectId=@pid
                    ORDER BY k.Id", new { pid = Session.ProjectId }).ToList();

                _rows.RaiseListChangedEvents = false;
                _rows.Clear();
                foreach (var r in rows) _rows.Add(r);
                _rows.RaiseListChangedEvents = true;
                _rows.ResetBindings();
                for (int i = 0; i < _grid.Rows.Count; i++)
                {
                    _grid.Rows[i].Cells[0].Value = i + 1;
                }
            }

            // the checklist shows code and thickness (text items, so nothing depends on DisplayMember)
            _clb.Items.Clear();
            foreach (var r in _rows)
            {
                var t = r.PlateThickness.HasValue ? "   (ضخامت " + r.PlateThickness.Value.ToString("0.##") + ")" : "";
                _clb.Items.Add(r.ConnectionCode + t);
            }
            ShowLinks();
        }

        void ShowLinks()
        {
            var part = _cmbPart.SelectedItem as Part;
            for (int i = 0; i < _clb.Items.Count; i++) _clb.SetItemChecked(i, false);
            if (part == null) return;

            HashSet<int> linked;
            using (var c = Db.Open())
                linked = new HashSet<int>(c.Query<int>("SELECT ConnectionId FROM PartConnection WHERE PartId=@id",
                                                       new { id = part.Id }));
            for (int i = 0; i < _rows.Count; i++)
                _clb.SetItemChecked(i, linked.Contains(_rows[i].Id));
        }

        // ---- add / import / delete -----------------------------------------

        // false with a message when the code is empty or already exists
        bool Insert(
            SqlConnection c,
            string code,
            decimal? thickness,
            int? qty,
            decimal? weight,
            out string err)
        {
            err = "";

            // Required field validation
            if (string.IsNullOrWhiteSpace(code))
            {
                err = "فیلد «کد اتصال» الزامی است.";
                return false;
            }

            if (!thickness.HasValue)
            {
                err = "فیلد «ضخامت ورق» الزامی است.";
                return false;
            }

            if (!qty.HasValue)
            {
                err = "فیلد «تعداد» الزامی است.";
                return false;
            }

            if (!weight.HasValue)
            {
                err = "فیلد «وزن» الزامی است.";
                return false;
            }

            // Check duplicate connection code
            int n = c.ExecuteScalar<int>(
                @"SELECT COUNT(1)
          FROM Connection
          WHERE ProjectId = @pid AND ConnectionCode = @code",
                new { pid = Session.ProjectId, code });

            if (n > 0)
            {
                err = "اتصال «" + code + "» قبلاً ثبت شده است.";
                return false;
            }

            try
            {
                c.Execute(
                    @"INSERT INTO Connection
              (ProjectId, ConnectionCode, PlateThickness, Qty, Weight)
              VALUES
              (@pid, @code, @thickness, @qty, @weight)",
                    new
                    {
                        pid = Session.ProjectId,
                        code,
                        thickness,
                        qty,
                        weight
                    });
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                err = "اتصال «" + code + "» قبلاً ثبت شده است.";
                return false;
            }

            return true;
        }

        void AddOne()
        {
            string err;
            using (var c = Db.Open())
            {
                decimal? thickness = _thick.Value > 0 ? _thick.Value : (decimal?)null;
                decimal? weight = _weight.Value > 0 ? _weight.Value : (decimal?)null;
                if (!Insert(c, _code.Text.Trim(), thickness, (int)_qty.Value, weight, out err))
                {
                    Msg.Show(err);
                    return;
                }
            }
            _code.Clear();
            _code.Focus();
            LoadAll();
        }


        // Excel columns (PDF page 2): code, quantity, weight, plate thickness. Row 1 is a header.
        void ImportExcel()
        {
            using (var dlg = new OpenFileDialog { Filter = "Excel|*.xlsx" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                var errors = new List<string>();
                int added = 0;
                try
                {
                    using (var wb = new XLWorkbook(dlg.FileName))
                    using (var c = Db.Open())
                    {
                        var used = wb.Worksheet(1).RangeUsed();
                        if (used == null)
                        {
                            Msg.Show("فایل اکسل خالی است.");
                            return;
                        }

                        foreach (var r in used.RowsUsed().Skip(1))
                        {
                            int rowNumber = r.RowNumber();

                            int q;
                            decimal w, t;

                            int? qty = r.Cell(2).TryGetValue<int>(out q)
                                ? q : (int?)null;

                            decimal? weight = r.Cell(3).TryGetValue<decimal>(out w)
                                ? w : (decimal?)null;

                            decimal? thick = r.Cell(4).TryGetValue<decimal>(out t)
                                ? t : (decimal?)null;

                            string err;

                            if (Insert(c, r.Cell(1).GetString().Trim(),
                                       thick, qty, weight, out err))
                            {
                                added++;
                            }
                            else
                            {
                                errors.Add("سطر " + rowNumber + ": " + err);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Msg.Show("خواندن فایل اکسل ناموفق بود:\n" + ex.Message);
                }

                LoadAll();
                var msg = added + " اتصال اضافه شد.";
                if (errors.Count > 0) msg += "\n" + string.Join("\n", errors);
                Msg.Show(msg);
            }
        }

        void DeleteSelected()
        {
            var row = _grid.CurrentRow == null ? null : _grid.CurrentRow.DataBoundItem as Row;
            if (row == null) return;

            using (var c = Db.Open())
            using (var tx = c.BeginTransaction())
            {
                try
                {
                    c.Execute("DELETE FROM PartConnection WHERE ConnectionId=@id", new { id = row.Id }, tx);
                    c.Execute("DELETE FROM Connection WHERE Id=@id", new { id = row.Id }, tx);
                    tx.Commit();
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    tx.Rollback();
                    Msg.Show("این اتصال در یک گزارش بازرسی استفاده شده و قابل حذف نیست.");
                    return;
                }
            }
            LoadAll();
        }

        // ---- part <-> connection links -------------------------------------

        void SaveLinks()
        {
            var part = _cmbPart.SelectedItem as Part;
            if (part == null) { Msg.Show("ابتدا قطعات را تعریف کنید و یک قطعه انتخاب کنید."); return; }

            try
            {
                using (var c = Db.Open())
                using (var tx = c.BeginTransaction())
                {
                    c.Execute("DELETE FROM PartConnection WHERE PartId=@id", new { id = part.Id }, tx);
                    foreach (int i in _clb.CheckedIndices)
                        c.Execute("INSERT INTO PartConnection (PartId, ConnectionId) VALUES (@pid, @cid)",
                                  new { pid = part.Id, cid = _rows[i].Id }, tx);
                    tx.Commit();
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Msg.Show("کلید جدول PartConnection باید (PartId, ConnectionId) باشد تا یک اتصال به چند قطعه لینک شود.\n" +
                         "فایل Database\\schema.sql را ببینید.");
                return;
            }

            LoadAll();
            Msg.Show("لینک‌های قطعه " + part.PartName + " ذخیره شد.");
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

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void lblCall_Click(object sender, EventArgs e)
        {

        }

        private void Label5_Click(object sender, EventArgs e)
        {

        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

    }
}
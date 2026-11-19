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
    // Parts (Assembly List): add one by one or from Excel, delete, then confirm with the green button.
    // The list is only written to the database when the green button is pressed.
    // Built the same way as ConnectionsForm: everything comes from the shared Ui helpers.
    public partial class PartsForm : Form
    {
        readonly BindingList<Part> _parts = new BindingList<Part>();
        readonly List<int> _deleted = new List<int>();
        readonly DataGridView _grid = Ui.Grid();

        readonly TextBox _name = new TextBox();
        readonly TextBox _code = new TextBox();
        readonly TextBox _drawing = new TextBox();
        readonly NumericUpDown _qty = new NumericUpDown { Minimum = 1, Maximum = 100000, Value = 1 };
        readonly NumericUpDown _length = new NumericUpDown { DecimalPlaces = 2, Minimum = 0, Maximum = 100000 };
        readonly NumericUpDown _weight = new NumericUpDown { DecimalPlaces = 3, Minimum = 0, Maximum = 100000 };

        public PartsForm()
        {
            Ui.Setup(this, "تعریف قطعات (Assembly List)", 1250, 720);

            var add = Ui.Btn("افزودن (موردی)", 130);
            var excel = Ui.Btn("فراخوانی گروهی از اکسل", 200);
            add.Margin = new Padding(6, 24, 6, 6);
            excel.Margin = new Padding(6, 24, 6, 6);
            add.Click += (s, e) => AddOne();
            excel.Click += (s, e) => ImportExcel();

            var delete = Ui.Btn("حذف قطعه انتخاب شده", 200);
            var confirm = Ui.Btn("تایید نام و کد قطعات", 200);
            delete.Click += (s, e) => DeleteSelected();
            confirm.Click += (s, e) => Confirm();

            var root = Ui.Root(this, Ui.Abs(90), Ui.Abs(96), Ui.Fill(), Ui.Abs(56));

            PictureBox logo;
            root.Controls.Add(Ui.Banner("بخش اول (تعریف قطعات)", out logo, null), 0, 0);

            var entry = new FlowLayoutPanel { Dock = DockStyle.Fill, RightToLeft = RightToLeft.Yes, Padding = new Padding(8) };
            entry.Controls.Add(Ui.Field("نام قطعه", _name, 140));
            entry.Controls.Add(Ui.Field("کد قطعه", _code, 120));
            entry.Controls.Add(Ui.Field("کد قطعه مطابق نقشه ها", _drawing, 160));
            entry.Controls.Add(Ui.Field("تعداد", _qty, 80));
            entry.Controls.Add(Ui.Field("طول", _length, 100));
            entry.Controls.Add(Ui.Field("وزن", _weight, 100));
            entry.Controls.Add(add);
            entry.Controls.Add(excel);
            root.Controls.Add(entry, 0, 1);

            Ui.AddCol(_grid, "ردیف", "");
            Ui.AddCol(_grid, "کد قطعه مطابق نقشه ها", "DrawingCode");
            Ui.AddCol(_grid, "نام قطعه", "PartName");
            Ui.AddCol(_grid, "کد قطعه", "PartCode");
            Ui.AddCol(_grid, "کل تعداد", "Qty");
            Ui.AddCol(_grid, "طول", "Length");
            Ui.AddCol(_grid, "وزن", "Weight");
            Ui.NumberFirstColumn(_grid);
            _grid.DataSource = _parts;
            root.Controls.Add(_grid, 0, 2);

            root.Controls.Add(Ui.BottomBar(delete, "در صورت تایید لیست زیر، کلیک نمایید", confirm), 0, 3);

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
        void AddOne()
        {
            var p = new Part
            {
                ProjectId = Session.ProjectId,
                PartName = _name.Text.Trim(),
                PartCode = _code.Text.Trim(),
                DrawingCode = _drawing.Text.Trim(),
                Qty = (int)_qty.Value,
                Length = _length.Value,
                Weight = _weight.Value
            };

            string err;
            if (!TryAdd(p, out err))
            {
                Msg.Show(err);
                return;
            }

            _name.Clear();
            _code.Clear();
            _drawing.Clear();
            _name.Focus();
        }

        // method 2: Excel with columns name, quantity, length, weight (row 1 is a header)
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
                            if (TryAdd(p, out err)) added++;
                            else errors.Add("سطر " + r.RowNumber() + ": " + err);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Msg.Show("خواندن فایل اکسل ناموفق بود:\n" + ex.Message);
                    return;
                }

                var msg = added + " قطعه به لیست اضافه شد. برای ذخیره، دکمه سبز را بزنید.";
                if (errors.Count > 0) msg += "\n\n" + string.Join("\n", errors);
                Msg.Show(msg);
            }
        }

        // PDF note 1: parts can be removed from the list
        void DeleteSelected()
        {
            var p = _grid.CurrentRow == null ? null : _grid.CurrentRow.DataBoundItem as Part;
            if (p == null) return;

            if (p.IsSaved) _deleted.Add(p.Id);
            _parts.Remove(p);
        }

        // green button: write the list to the database
        void Confirm()
        {
            _grid.EndEdit();
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
            Msg.Show("لیست قطعات ذخیره شد.");
        }
    }
}

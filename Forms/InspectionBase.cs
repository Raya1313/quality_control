using Dapper;
using Data;
using QcApp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // Shared screen for the DT, VT, UT and PT reports: header fields, part picking
    // (manual / list / random), editable grid, save, preview and print.
    public abstract class InspectionBase : Form
    {
        static readonly System.Globalization.CultureInfo Inv = System.Globalization.CultureInfo.InvariantCulture;

        protected readonly string Kind;
        protected readonly DataGridView Grid = Ui.Grid();
        protected byte[]? Sketch;

        readonly bool _hasSketch;
        readonly FlowLayoutPanel _header;
        readonly Dictionary<string, Control> _fields = new Dictionary<string, Control>();
        readonly Dictionary<string, string> _dates = new Dictionary<string, string>();   // key -> caption
        readonly HashSet<string> _optionalDates = new HashSet<string>();
        readonly PictureBox _sketchBox = new PictureBox
        {
            Width = 170,
            Height = 70,
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(6)
        };

        protected abstract IList RowsList { get; }
        protected abstract void AddRowsFor(PartPick pick);
        protected abstract void Persist(Dictionary<string, string> p, byte[]? sketch);
        protected abstract void LoadRowsFor(int reportId);
        protected abstract List<ReportDoc> BuildDocs();

        protected InspectionBase(string kind, string title, bool hasSketch)
        {
            Kind = kind;
            _hasSketch = hasSketch;
            Ui.Setup(this, title, 1400, 780);

            var delete = Ui.Btn("حذف سطر", 130);
            var confirm = Ui.Btn("تایید و ذخیره گزارش", 220);
            delete.Click += (s, e) => DeleteCurrent();
            confirm.Click += (s, e) => Save();

            var root = Ui.Root(this, Ui.Abs(90), Ui.Auto(), Ui.Fill(), Ui.Abs(56));

            PictureBox logo;
            root.Controls.Add(Ui.Banner(title, out logo, null), 0, 0);

            _header = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = true,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(8)
            };
            root.Controls.Add(_header, 0, 1);

            Grid.ReadOnly = false;
            Grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            Grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            Grid.ColumnHeadersHeight = 56;
            Grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            root.Controls.Add(Grid, 0, 2);

            root.Controls.Add(Ui.BottomBar(delete, "در صورت تایید لیست زیر، کلیک نمایید", confirm), 0, 3);
        }

        // ---- header fields --------------------------------------------------

        protected TextBox AddText(string key, string caption, int width, string def = "")
        {
            var t = new TextBox { Text = def };
            Register(key, caption, t, width);
            return t;
        }

        protected void AddDate(string key, string caption, bool required = true)
        {
            var m = new MaskedTextBox { Mask = "0000/00/00", RightToLeft = RightToLeft.No, Culture = Inv };
            _dates[key] = caption;
            if (!required) _optionalDates.Add(key);
            Register(key, caption, m, 190);
        }

        protected NumericUpDown AddNum(string key, string caption, decimal value, decimal min, decimal max, int width = 120)
        {
            var n = new NumericUpDown { Minimum = min, Maximum = max, Value = value };
            Register(key, caption, n, width);
            return n;
        }

        protected ComboBox AddCombo(string key, string caption, int width, params string[] items)
        {
            var c = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
            c.Items.AddRange(items);
            if (items.Length > 0) c.Text = items[0];
            Register(key, caption, c, width);
            return c;
        }

        void Register(string key, string caption, Control c, int width)
        {
            _fields[key] = c;
            _header.Controls.Add(Ui.Field(caption, c, width));
        }

        protected void AddButton(string text, int width, Action action)
        {
            var b = Ui.Btn(text, width);
            b.Margin = new Padding(6, 24, 6, 6);
            b.Click += (s, e) => action();
            _header.Controls.Add(b);
        }

        protected string P(string key)
        {
            Control c;
            if (!_fields.TryGetValue(key, out c)) return "";
            var n = c as NumericUpDown;
            if (n != null) return n.Value.ToString(Inv);
            if (c is MaskedTextBox && IsBlankDate(c.Text)) return "";
            return c.Text.Trim();
        }

        static bool IsBlankDate(string s)
        {
            return s.Replace("/", "").Trim().Length == 0;
        }

        // ---- grid columns ---------------------------------------------------

        protected void Txt(string header, string prop, int width, bool readOnly)
        {
            Grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly
            });
        }

        protected void Chk(string header, string prop, int width)
        {
            Grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width
            });
        }

        protected void Combo(string header, string prop, int width, params KeyValuePair<string, string>[] items)
        {
            Grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                DataSource = items,
                DisplayMember = "Value",
                ValueMember = "Key",
                FlatStyle = FlatStyle.Flat
            });
        }

        // call at the end of the subclass constructor, after fields and columns are defined
        protected void Start()
        {
            AddButton("افزودن قطعات", 140, PickParts);
            if (_hasSketch)
            {
                AddButton("افزودن شماتیک قطعه", 160, PickSketch);
                _header.Controls.Add(_sketchBox);
            }
            AddButton("پیش نمایش و چاپ", 150, Print);

            Ui.NumberFirstColumn(Grid);
            Grid.DataSource = RowsList;
            LoadFromDb();
        }

        // ---- actions --------------------------------------------------------

        void PickParts()
        {
            List<Part> parts;
            using (var c = Db.Open())
                parts = c.Query<Part>("SELECT * FROM Part WHERE ProjectId=@pid ORDER BY Id",
                                      new { pid = Session.ProjectId }).ToList();

            if (parts.Count == 0) { Msg.Show("ابتدا قطعات را تعریف کنید."); return; }

            using (var dlg = new PartPickerDialog(parts))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                var existing = new HashSet<string>(
                    RowsList.Cast<InstanceRow>().Select(r => r.PartId + ":" + r.InstanceNo));
                int skipped = 0;
                foreach (var pick in dlg.Picks)
                {
                    if (!existing.Add(pick.PartId + ":" + pick.InstanceNo)) { skipped++; continue; }   // no repeats
                    AddRowsFor(pick);
                }
                if (skipped > 0)
                    Msg.Show(skipped + " قطعه قبلا در لیست بوده و دوباره اضافه نشد.");
            }
        }

        void PickSketch()
        {
            using (var d = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp" })
            {
                if (d.ShowDialog() != DialogResult.OK) return;
                Sketch = File.ReadAllBytes(d.FileName);
                ShowSketch();
            }
        }

        void ShowSketch()
        {
            _sketchBox.Image = Sketch == null ? null : new Bitmap(new MemoryStream(Sketch));
        }

        void DeleteCurrent()
        {
            var item = Grid.CurrentRow == null ? null : Grid.CurrentRow.DataBoundItem;
            if (item != null) RowsList.Remove(item);
        }

        // PDF: no inspection date may be before the contract date
        bool ValidateDates()
        {
            var contract = Session.Project.ContractDate.Date;
            foreach (var kv in _dates)
            {
                if (_optionalDates.Contains(kv.Key) && IsBlankDate(_fields[kv.Key].Text)) continue;
                DateTime d;
                if (!Calendar.TryParse(_fields[kv.Key].Text, out d))
                {
                    Msg.Show("تاریخ «" + kv.Value + "» نامعتبر است.");
                    return false;
                }
                if (d.Date < contract)
                {
                    Msg.Show("«" + kv.Value + "» نمی‌تواند قبل از تاریخ عقد قرارداد (" +
                             Calendar.ToText(contract) + ") باشد.");
                    return false;
                }
            }
            return true;
        }

        Dictionary<string, string> Params()
        {
            var d = new Dictionary<string, string>();
            foreach (var kv in _fields) d[kv.Key] = P(kv.Key);
            return d;
        }

        void ApplyParams(Dictionary<string, string> d)
        {
            foreach (var kv in _fields)
            {
                string v;
                if (!d.TryGetValue(kv.Key, out v)) continue;
                var n = kv.Value as NumericUpDown;
                if (n != null)
                {
                    decimal x;
                    if (decimal.TryParse(v, System.Globalization.NumberStyles.Float, Inv, out x))
                        n.Value = Math.Max(n.Minimum, Math.Min(n.Maximum, x));
                }
                else kv.Value.Text = v;
            }
        }

        void Save()
        {
            Grid.EndEdit();
            if (RowsList.Count == 0) { Msg.Show("لیست خالی است."); return; }
            if (!ValidateDates()) return;

            try
            {
                Persist(Params(), Sketch);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Msg.Show("سطر تکراری در لیست وجود دارد.");
                return;
            }
            Msg.Show("گزارش ذخیره شد.");
        }

        void LoadFromDb()
        {
            var h = InspectionRepo.LoadHeader(Session.ProjectId, Kind);
            if (h == null) return;
            ApplyParams(h.Params);
            Sketch = h.Sketch;
            ShowSketch();
            LoadRowsFor(h.Id);
        }

        void Print()
        {
            Grid.EndEdit();
            if (RowsList.Count == 0) { Msg.Show("لیست خالی است."); return; }
            ReportPrinter.Preview(BuildDocs());
        }

        // ---- helpers for the print layouts ---------------------------------

        protected static string Mark(bool b) { return b ? "√" : ""; }

        protected static string Either(string en, string fa)
        {
            return !string.IsNullOrWhiteSpace(en) ? en : fa;
        }

        protected static string ProjectEn() { return Either(Session.Project.NameEn, Session.Project.NameFa); }
        protected static string ClientEn() { return Either(Session.Project.ClientEn, Session.Project.ClientFa); }
        protected static string ContractorEn() { return Either(Session.Project.ContractorEn, Session.Project.ContractorFa); }
        protected static string QcEn() { return Either(Session.Project.QcManagerEn, Session.Project.QcManagerFa); }
        protected static string CoordEn() { return Either(Session.Project.CoordinatorEn, Session.Project.CoordinatorFa); }

        protected static Image? SketchImage(byte[]? data)
        {
            return data == null ? null : new Bitmap(new MemoryStream(data));
        }

        protected static string PhaseText()
        {
            return string.IsNullOrWhiteSpace(Session.Project.ReportedPhase) ? "-" : Session.Project.ReportedPhase;
        }

        protected static PartPick ToPick(InstanceRow r)
        {
            return new PartPick { PartId = r.PartId, PartName = r.PartName, InstanceNo = r.InstanceNo, InstanceTotal = r.InstanceTotal };
        }

    }
}
using Dapper;
using Data;
using QcApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // Section 2: pre/during welding inspection checklist
    public class WeldChecklistForm : Form
    {
        public class Link
        {
            public int PartId { get; set; }
            public int ConnectionId { get; set; }
            public string ConnectionCode { get; set; } = "";
            public decimal? PlateThickness { get; set; }
        }

        readonly BindingList<WeldRow> _rows = new BindingList<WeldRow>();
        readonly DataGridView _grid = Ui.Grid();
        int _reportId;

        readonly MaskedTextBox _start = new MaskedTextBox
        {
            Mask = "0000/00/00",
            RightToLeft = RightToLeft.No,
            Culture = System.Globalization.CultureInfo.InvariantCulture
        };
        readonly NumericUpDown _gapBefore = Num(1, 1, 30);
        readonly NumericUpDown _gapDuring = Num(1, 0, 30);
        readonly NumericUpDown _capacity = Num(10, 1, 1000);
        readonly NumericUpDown _percent = Num(50, 1, 100);
        readonly TextBox _supervisor = new TextBox();
        readonly TextBox _hall = new TextBox();
        readonly TextBox _skeleton = new TextBox();
        readonly TextBox _metal = new TextBox { Text = "ST-37" };
        readonly TextBox _consumable = new TextBox { Text = "E-70" };

        static NumericUpDown Num(decimal v, decimal min, decimal max)
        {
            return new NumericUpDown { Minimum = min, Maximum = max, Value = v };
        }

        static void Set(NumericUpDown n, decimal v)
        {
            n.Value = Math.Max(n.Minimum, Math.Min(n.Maximum, v));
        }

        public WeldChecklistForm()
        {
            Ui.Setup(this, "چک لیست بازرسی قبل و حین جوشکاری", 1400, 760);

            var delete = Ui.Btn("حذف سطر", 130);
            var confirm = Ui.Btn("تایید و ذخیره گزارش", 220);
            delete.Click += (s, e) => DeleteRow();
            confirm.Click += (s, e) => Save();

            var root = Ui.Root(this, Ui.Abs(90), Ui.Auto(), Ui.Fill(), Ui.Abs(56));

            PictureBox logo;
            root.Controls.Add(Ui.Banner("بخش دوم (چک لیست بازرسی قبل و حین جوشکاری)", out logo, null), 0, 0);
            root.Controls.Add(BuildHeader(), 0, 1);
            BuildGrid();
            root.Controls.Add(_grid, 0, 2);
            root.Controls.Add(Ui.BottomBar(delete, "در صورت تایید لیست زیر، کلیک نمایید", confirm), 0, 3);

            LoadFromDb();
        }

        Control BuildHeader()
        {
            var gen = Ui.Btn("ساخت لیست بازرسی", 160);
            var tick = Ui.Btn("تایید همه موارد", 140);
            var print = Ui.Btn("پیش نمایش و چاپ", 150);
            gen.Margin = new Padding(6, 24, 6, 6);
            tick.Margin = new Padding(6, 24, 6, 6);
            print.Margin = new Padding(6, 24, 6, 6);
            gen.Click += (s, e) => Generate();
            tick.Click += (s, e) => TickAll();
            print.Click += (s, e) => Print();

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = true,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(8)
            };
            flow.Controls.Add(Ui.Field("تاریخ شروع بازرسی (قبل از جوشکاری)", _start, 190));
            flow.Controls.Add(Ui.Field("فاصله گزارش‌ها (روز) قبل از جوشکاری", _gapBefore, 150));
            flow.Controls.Add(Ui.Field("فاصله گزارش‌ها (روز) حین جوشکاری", _gapDuring, 150));
            flow.Controls.Add(Ui.Field("ظرفیت تولید کارخانه (قطعه در روز)", _capacity, 150));
            flow.Controls.Add(Ui.Field("درصد اتصالات مورد بازرسی هر قطعه", _percent, 150));
            flow.Controls.Add(Ui.Field("مسئول سالن مونتاژ", _supervisor, 150));
            flow.Controls.Add(Ui.Field("سالن", _hall, 90));
            flow.Controls.Add(Ui.Field("زمان ساخت اسکلت", _skeleton, 120));
            flow.Controls.Add(Ui.Field("نوع فلز پایه", _metal, 90));
            flow.Controls.Add(Ui.Field("مواد مصرفی جوشکاری", _consumable, 100));
            flow.Controls.Add(gen);
            flow.Controls.Add(tick);
            flow.Controls.Add(print);
            return flow;
        }

        void BuildGrid()
        {
            _grid.ReadOnly = false;
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            _grid.ColumnHeadersHeight = 56;
            _grid.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;

            Txt("ردیف", "", 45, true);
            Txt("تاریخ", "ReportDateText", 85, true);
            Txt("کد قطعه", "PartLabel", 120, true);
            Txt("کد اتصال", "ConnectionCode", 80, true);
            Txt("زاویه پخ", "BevelAngle", 60, false);
            Chk("آماده سازی اتصال", "JointPrepOk", 70);
            Txt("ضخامت ورق", "PlateThickness", 60, true);
            Txt("نوع برش", "CutType", 55, true);
            Chk("WPS جوشکاری", "WpsOk", 65);
            Chk("کنترل جوشکار", "WelderCtrlOk", 65);
            Chk("کنترل نهایی قبل از جوشکاری", "FinalCtrlOk", 90);

            Txt("حین جوشکاری\nتاریخ", "DuringDateText", 85, true);
            Txt("کد جوشکار", "WelderCode", 70, false);
            Txt("دمای محیط", "AmbientTemp", 60, false);
            Txt("سرعت پیشروی", "TravelSpeed", 65, false);
            Txt("شدت جریان", "Amperage", 60, false);
            for (int i = 1; i <= 6; i++)
                Chk("آماده سازی قطعه\n" + i, "Ready" + i, 55);
            Txt("توضیحات", "Notes", 180, false);

            Ui.NumberFirstColumn(_grid);
            _grid.DataSource = _rows;
        }

        void Txt(string header, string prop, int width, bool readOnly)
        {
            _grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width,
                ReadOnly = readOnly
            });
        }

        void Chk(string header, string prop, int width)
        {
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = header,
                DataPropertyName = prop,
                Width = width
            });
        }

        // PDF: start date can't be before the contract date
        bool TryReadStart(out DateTime start)
        {
            if (!Calendar.TryParse(_start.Text, out start))
            {
                Msg.Show("تاریخ شروع بازرسی نامعتبر است.");
                return false;
            }
            var contract = Session.Project.ContractDate.Date;
            if (start.Date < contract)
            {
                Msg.Show("تاریخ شروع بازرسی نمی‌تواند قبل از تاریخ عقد قرارداد (" +
                         Calendar.ToText(contract) + ") باشد.");
                return false;
            }
            return true;
        }

        void Generate()
        {
            DateTime start;
            if (!TryReadStart(out start)) return;

            if (_rows.Count > 0 &&
                MessageBox.Show("لیست فعلی با لیست جدید جایگزین شود؟", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) != DialogResult.Yes)
                return;

            List<Part> parts;
            List<Link> links;
            using (var c = Db.Open())
            {
                parts = c.Query<Part>("SELECT * FROM Part WHERE ProjectId=@pid ORDER BY Id",
                                      new { pid = Session.ProjectId }).ToList();
                links = c.Query<Link>(@"
                    SELECT pc.PartId, pc.ConnectionId, k.ConnectionCode, k.PlateThickness
                    FROM PartConnection pc
                    JOIN Connection k ON k.Id = pc.ConnectionId
                    WHERE k.ProjectId=@pid", new { pid = Session.ProjectId }).ToList();
            }

            if (parts.Count == 0)
            {
                Msg.Show("ابتدا قطعات را تعریف کنید.");
                return;
            }

            int cap = (int)_capacity.Value;
            int gapB = (int)_gapBefore.Value;
            int gapD = (int)_gapDuring.Value;
            int perReport = cap * gapB;            // parts produced between two report dates
            decimal pct = _percent.Value;
            var rnd = new Random();

            var result = new List<WeldRow>();
            var unlinked = new List<string>();
            int index = 0;

            foreach (var p in parts)
            {
                var own = links.Where(x => x.PartId == p.Id).ToList();
                if (own.Count == 0) { unlinked.Add(p.PartName); continue; }

                int take = Math.Max(1, (int)Math.Ceiling(own.Count * pct / 100m));

                for (int k = 1; k <= p.Qty; k++, index++)
                {
                    var date = start.AddDays((index / perReport) * gapB);

                    var picked = own.OrderBy(x => rnd.Next()).Take(take).OrderBy(x => x.ConnectionCode);
                    foreach (var link in picked)
                    {
                        result.Add(new WeldRow
                        {
                            PartId = p.Id,
                            PartName = p.PartName,
                            InstanceNo = k,
                            InstanceTotal = p.Qty,
                            ConnectionId = link.ConnectionId,
                            ConnectionCode = link.ConnectionCode,
                            PlateThickness = link.PlateThickness,
                            ReportDate = date,
                            DuringDate = date.AddDays(gapD)
                        });
                    }
                }
            }

            _rows.RaiseListChangedEvents = false;
            _rows.Clear();
            foreach (var x in result) _rows.Add(x);
            _rows.RaiseListChangedEvents = true;
            _rows.ResetBindings();

            if (unlinked.Count > 0)
                Msg.Show("این قطعات به هیچ اتصالی لینک نشده‌اند و در لیست نیامدند:\n" +
                         string.Join("، ", unlinked));
        }

        void TickAll()
        {
            foreach (var x in _rows)
            {
                x.JointPrepOk = x.WpsOk = x.WelderCtrlOk = x.FinalCtrlOk = true;
                x.Ready1 = x.Ready2 = x.Ready3 = x.Ready4 = x.Ready5 = x.Ready6 = true;
            }
            _rows.ResetBindings();
        }

        void DeleteRow()
        {
            var x = _grid.CurrentRow == null ? null : _grid.CurrentRow.DataBoundItem as WeldRow;
            if (x == null) return;
            _rows.Remove(x);
        }

        void Save()
        {
            _grid.EndEdit();
            if (_rows.Count == 0) { Msg.Show("لیست خالی است."); return; }

            DateTime start;
            if (!TryReadStart(out start)) return;

            var r = new WeldReport
            {
                Id = _reportId,
                ProjectId = Session.ProjectId,
                StartDate = start,
                GapBefore = (int)_gapBefore.Value,
                GapDuring = (int)_gapDuring.Value,
                DailyCapacity = (int)_capacity.Value,
                ConnectionPercent = _percent.Value,
                HallSupervisor = _supervisor.Text.Trim(),
                Hall = _hall.Text.Trim(),
                SkeletonTime = _skeleton.Text.Trim(),
                BaseMetal = _metal.Text.Trim(),
                WeldConsumable = _consumable.Text.Trim()
            };

            try
            {
                WeldRepo.Save(r, _rows);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                Msg.Show("سطر تکراری در لیست وجود دارد.");
                return;
            }

            _reportId = r.Id;
            Msg.Show("گزارش ذخیره شد.");
        }

        void LoadFromDb()
        {
            var r = WeldRepo.LoadReport(Session.ProjectId);
            if (r == null) return;

            _reportId = r.Id;
            _start.Text = Calendar.ToText(r.StartDate);
            Set(_gapBefore, r.GapBefore);
            Set(_gapDuring, r.GapDuring);
            Set(_capacity, r.DailyCapacity);
            Set(_percent, r.ConnectionPercent);
            _supervisor.Text = r.HallSupervisor ?? "";
            _hall.Text = r.Hall ?? "";
            _skeleton.Text = r.SkeletonTime ?? "";
            _metal.Text = r.BaseMetal ?? "";
            _consumable.Text = r.WeldConsumable ?? "";

            foreach (var x in WeldRepo.LoadRows(r.Id)) _rows.Add(x);
        }

        static string Mark(bool b) { return b ? "√" : ""; }
        static string N(decimal? v) { return v.HasValue ? v.Value.ToString("0.##") : ""; }

        void Print()
        {
            _grid.EndEdit();
            if (_rows.Count == 0) { Msg.Show("لیست خالی است."); return; }

            var d = new ReportDoc
            {
                Title = "چک لیست بازرسی قبل و حین جوشکاری",
                Code = "کد مدرک: QC-FR-12/02",
                Revision = "تاریخ بازنگری: 1403/10/01",
                Rtl = true,
                Landscape = true,
                InfoColumns = 3,
                MinRows = 15,
                Headers = new[]
                {
                    "ردیف", "تاریخ", "کد قطعه", "کد اتصال", "لبه سازی (زاویه پخ)", "آماده سازی اتصال", "ضخامت ورق",
                    "نوع برش", "WPS جوشکاری", "کنترل جوشکار", "کنترل نهایی قبل از جوشکاری",
                    "تاریخ (حین)", "کد جوشکار", "دمای محیط", "سرعت پیشروی", "شدت جریان",
                    "1", "2", "3", "4", "5", "6", "توضیحات"
                },
                Widths = new float[] { 3, 7, 9, 6, 5, 5, 5, 5, 5, 5, 6, 7, 5, 4, 5, 5, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 9 },
                Signers = new[]
                {
                    "تکمیل کننده و مسئول QC\nنام و نام خانوادگی: " + Session.Project.QcManagerFa + "\nامضاء و تاریخ:",
                    "تایید کننده (هماهنگ کننده فعالیتهای جوشکاری)\nنام و نام خانوادگی: " + Session.Project.CoordinatorFa + "\nامضاء و تاریخ:"
                }
            };
            d.Info.Add(new[] { "نام پروژه:", Session.Project.NameFa });
            d.Info.Add(new[] { "نام کارفرما:", Session.Project.ClientFa });
            d.Info.Add(new[] { "مسئول سالن:", _supervisor.Text.Trim() });
            d.Info.Add(new[] { "سالن:", _hall.Text.Trim() });
            d.Info.Add(new[] { "نوع فلز پایه:", _metal.Text.Trim() });
            d.Info.Add(new[] { "مواد مصرفی جوشکاری:", _consumable.Text.Trim() });

            int n = 1;
            foreach (var r in _rows)
            {
                d.Rows.Add(new[]
                {
                    n.ToString(), r.ReportDateText, r.PartLabel, r.ConnectionCode, N(r.BevelAngle), Mark(r.JointPrepOk),
                    N(r.PlateThickness), r.CutType, Mark(r.WpsOk), Mark(r.WelderCtrlOk), Mark(r.FinalCtrlOk),
                    r.DuringDateText, r.WelderCode ?? "", N(r.AmbientTemp), N(r.TravelSpeed), N(r.Amperage),
                    Mark(r.Ready1), Mark(r.Ready2), Mark(r.Ready3), Mark(r.Ready4), Mark(r.Ready5), Mark(r.Ready6),
                    r.Notes ?? ""
                });
                n++;
            }
            ReportPrinter.Preview(new List<ReportDoc> { d });
        }
    }
}
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
    // Section 7: nonconformance form (one record per defective part instance)
    public class NcrForm : Form
    {
        static readonly System.Globalization.CultureInfo Inv = System.Globalization.CultureInfo.InvariantCulture;

        readonly List<Part> _parts;
        readonly BindingList<Ncr> _list = new BindingList<Ncr>();
        readonly BindingList<NcrAction> _actions = new BindingList<NcrAction>();
        readonly BindingList<NcrCheck> _checks = new BindingList<NcrCheck>();
        readonly DataGridView _listGrid = Ui.Grid();

        DataGridView? _gridActions, _gridChecks;
        Ncr _current = new Ncr();
        bool _loading;

        readonly MaskedTextBox _date = DateBox();
        readonly MaskedTextBox _deadline = DateBox();
        readonly TextBox _productType = new TextBox { Text = "داخلی" };
        readonly ComboBox _cmbPart = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        readonly NumericUpDown _instance = new NumericUpDown { Minimum = 1, Maximum = 1, Value = 1 };
        readonly TextBox _stage = new TextBox();
        readonly TextBox _supervisor = new TextBox();
        readonly TextBox _inspector = new TextBox();
        readonly TextBox _preparer = new TextBox();
        readonly ComboBox _severity = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown };
        readonly TextBox _desc = Multi(60);
        readonly TextBox _task = Multi(60);
        readonly TextBox _session = new TextBox();
        readonly TextBox _execResult = Multi(50);
        readonly TextBox _inspResult = Multi(50);
        readonly TextBox _review = new TextBox();
        readonly TextBox _final = Multi(50);
        readonly NumericUpDown _qAccepted = Qty();
        readonly NumericUpDown _qRework = Qty();
        readonly NumericUpDown _qReturned = Qty();
        readonly NumericUpDown _qOther = Qty();
        readonly NumericUpDown _qScrap = Qty();

        static MaskedTextBox DateBox()
        {
            return new MaskedTextBox { Mask = "0000/00/00", RightToLeft = RightToLeft.No, Culture = Inv };
        }
        static TextBox Multi(int h)
        {
            return new TextBox { Multiline = true, Height = h, ScrollBars = ScrollBars.Vertical };
        }
        static NumericUpDown Qty()
        {
            return new NumericUpDown { Minimum = 0, Maximum = 100000, Value = 0 };
        }

        public NcrForm()
        {
            Ui.Setup(this, "فرم عدم انطباق", 1300, 820);

            using (var c = Db.Open())
                _parts = c.Query<Part>("SELECT * FROM Part WHERE ProjectId=@pid ORDER BY Id",
                                       new { pid = Session.ProjectId }).ToList();

            var newBtn = Ui.Btn("مورد جدید", 130);
            var printBtn = Ui.Btn("پیش نمایش و چاپ", 150);
            var delete = Ui.Btn("حذف مورد", 130);
            var save = Ui.Btn("تایید و ذخیره", 200);
            newBtn.Click += (s, e) => NewRecord();
            printBtn.Click += (s, e) => Print();
            delete.Click += (s, e) => DeleteRecord();
            save.Click += (s, e) => SaveRecord();

            var root = Ui.Root(this, Ui.Abs(90), Ui.Abs(46), Ui.Abs(140), Ui.Fill(), Ui.Abs(56));

            PictureBox logo;
            root.Controls.Add(Ui.Banner("بخش هفتم (فرم عدم انطباق)", out logo, null), 0, 0);

            var tool = new FlowLayoutPanel { Dock = DockStyle.Fill, RightToLeft = RightToLeft.Yes };
            tool.Controls.Add(newBtn);
            tool.Controls.Add(printBtn);
            root.Controls.Add(tool, 0, 1);

            BuildListGrid();
            root.Controls.Add(_listGrid, 0, 2);
            root.Controls.Add(BuildEditor(), 0, 3);
            root.Controls.Add(Ui.BottomBar(delete, "پس از تکمیل فرم، کلیک نمایید", save), 0, 4);

            _cmbPart.DataSource = _parts;
            _cmbPart.DisplayMember = "PartName";
            _cmbPart.SelectedIndexChanged += (s, e) => SyncInstanceMax();
            _severity.Items.AddRange(new object[] { "عمده", "جزئی", "بحرانی" });

            _date.Text = Calendar.ToText(DateTime.Today);
            SyncInstanceMax();
            LoadList();
        }

        void BuildListGrid()
        {
            Ui.AddCol(_listGrid, "ردیف", "");
            Ui.AddCol(_listGrid, "تاریخ", "NcrDateText");
            Ui.AddCol(_listGrid, "قطعه", "PartLabel");
            Ui.AddCol(_listGrid, "درجه اهمیت", "Severity");
            Ui.AddCol(_listGrid, "شرح عدم انطباق", "Description");
            Ui.NumberFirstColumn(_listGrid);
            _listGrid.DataSource = _list;
            _listGrid.SelectionChanged += (s, e) => LoadSelected();
        }

        DataGridView SubGrid(string[] headers, string[] props, object source)
        {
            var g = Ui.Grid();
            g.Dock = DockStyle.None;
            g.Width = 920;
            g.Height = 130;
            g.ReadOnly = false;
            g.AllowUserToAddRows = true;
            g.AllowUserToDeleteRows = true;
            g.SelectionMode = DataGridViewSelectionMode.CellSelect;
            for (int i = 0; i < headers.Length; i++) Ui.AddCol(g, headers[i], props[i]);
            g.DataSource = source;
            return g;
        }

        Control BuildEditor()
        {
            var f = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(8)
            };

            f.Controls.Add(Ui.Field("تاریخ", _date, 110));
            f.Controls.Add(Ui.Field("نوع محصول نامنطبق", _productType, 130));
            f.Controls.Add(Ui.Field("نام قطعه", _cmbPart, 140));
            f.Controls.Add(Ui.Field("شماره قطعه", _instance, 80));
            f.Controls.Add(Ui.Field("نام مرحله", _stage, 140));
            f.Controls.Add(Ui.Field("مسئول سالن مونتاژ", _supervisor, 140));
            f.Controls.Add(Ui.Field("نام بازرس", _inspector, 140));
            f.Controls.Add(Ui.Field("تهیه کننده", _preparer, 140));
            f.Controls.Add(Ui.Field("درجه اهمیت", _severity, 110));
            f.Controls.Add(Ui.Field("شرح عدم انطباق", _desc, 900));
            f.Controls.Add(Ui.Field("شرح تعیین تکلیف", _task, 900));

            _gridActions = SubGrid(new[] { "مواد", "ابزار و تجهیزات", "مجری", "شرح اجرا" },
                                   new[] { "Material", "Tools", "Executor", "Execution" }, _actions);
            f.Controls.Add(Ui.Field("دستور العمل اصلاح اقلام نامنطبق (مواد / ابزار و تجهیزات / مجری / شرح اجرا)",
                _gridActions, 920));
            _gridChecks = SubGrid(new[] { "مشخصه کنترلی", "تلرانس قابل قبول", "روش اندازه گیری", "تعداد و تناوب نمونه گیری", "توضیحات" },
                                  new[] { "Spec", "Tolerance", "Method", "Sampling", "Notes" }, _checks);
            f.Controls.Add(Ui.Field("دستور العمل بازرسی و پذیرش (مشخصه کنترلی / تلرانس / روش اندازه گیری / تعداد نمونه / توضیحات)",
                _gridChecks, 920));

            f.Controls.Add(Ui.Field("مدت جلسه تعیین تکلیف", _session, 140));
            f.Controls.Add(Ui.Field("مهلت تکمیل تا مورخ", _deadline, 110));
            f.Controls.Add(Ui.Field("نتایج اجرا (اصلاح)", _execResult, 900));
            f.Controls.Add(Ui.Field("نتیجه بازرسی", _inspResult, 900));

            f.Controls.Add(Ui.Field("مقدار پذیرفته شده", _qAccepted, 110));
            f.Controls.Add(Ui.Field("مقدار دوباره کاری", _qRework, 110));
            f.Controls.Add(Ui.Field("مقدار مرجوعی", _qReturned, 110));
            f.Controls.Add(Ui.Field("مقدار استفاده شده در موارد دیگر", _qOther, 150));
            f.Controls.Add(Ui.Field("مقدار ضایعات", _qScrap, 110));
            f.Controls.Add(Ui.Field("مدت زمان بازرسی", _review, 140));
            f.Controls.Add(Ui.Field("نتیجه نهایی تعیین تکلیف", _final, 900));
            return f;
        }

        void SyncInstanceMax()
        {
            var p = _cmbPart.SelectedItem as Part;
            _instance.Maximum = p == null ? 1 : Math.Max(1, p.Qty);
            if (_instance.Value > _instance.Maximum) _instance.Value = _instance.Maximum;
        }

        void LoadList()
        {
            _list.Clear();
            foreach (var n in NcrRepo.List(Session.ProjectId)) _list.Add(n);
            if (_list.Count == 0) NewRecord();
        }

        void NewRecord()
        {
            _loading = true;
            _listGrid.ClearSelection();
            _current = new Ncr();
            Fill(_current);
            _loading = false;
        }

        void LoadSelected()
        {
            if (_loading || _listGrid.CurrentRow == null) return;
            var n = _listGrid.CurrentRow.DataBoundItem as Ncr;
            if (n == null) return;

            _loading = true;
            _current = n;
            Fill(n);
            _loading = false;
        }

        void Fill(Ncr n)
        {
            _date.Text = Calendar.ToText(n.Id == 0 ? DateTime.Today : n.NcrDate);
            _deadline.Text = n.Deadline.HasValue ? Calendar.ToText(n.Deadline.Value) : "";
            _productType.Text = n.Id == 0 ? "داخلی" : n.ProductType;
            var part = _parts.FirstOrDefault(p => p.Id == n.PartId);
            if (part != null) _cmbPart.SelectedItem = part;
            SyncInstanceMax();
            _instance.Value = Math.Max(_instance.Minimum, Math.Min(_instance.Maximum, Math.Max(1, n.InstanceNo)));
            _stage.Text = n.Stage;
            _supervisor.Text = n.HallSupervisor;
            _inspector.Text = n.Inspector;
            _preparer.Text = n.Preparer;
            _severity.Text = n.Severity;
            _desc.Text = n.Description;
            _task.Text = n.TaskDescription;
            _session.Text = n.SessionDuration;
            _execResult.Text = n.ExecResult;
            _inspResult.Text = n.InspResult;
            _review.Text = n.ReviewDuration;
            _final.Text = n.FinalTask;
            _qAccepted.Value = n.QtyAccepted;
            _qRework.Value = n.QtyRework;
            _qReturned.Value = n.QtyReturned;
            _qOther.Value = n.QtyOtherUse;
            _qScrap.Value = n.QtyScrap;

            _actions.Clear();
            _checks.Clear();
            if (n.Id != 0)
            {
                foreach (var a in NcrRepo.Actions(n.Id)) _actions.Add(a);
                foreach (var k in NcrRepo.Checks(n.Id)) _checks.Add(k);
            }
        }

        // copies the editor into _current; false if something is missing or wrong
        bool Read()
        {
            if (_gridActions != null) _gridActions.EndEdit();
            if (_gridChecks != null) _gridChecks.EndEdit();
            DateTime d;
            if (!Calendar.TryParse(_date.Text, out d)) { Msg.Show("تاریخ نامعتبر است."); return false; }
            if (d.Date < Session.Project.ContractDate.Date)
            {
                Msg.Show("تاریخ نمی‌تواند قبل از تاریخ عقد قرارداد (" + Calendar.ToText(Session.Project.ContractDate) + ") باشد.");
                return false;
            }

            DateTime? deadline = null;
            if (_deadline.Text.Replace("/", "").Trim().Length > 0)
            {
                DateTime dl;
                if (!Calendar.TryParse(_deadline.Text, out dl)) { Msg.Show("مهلت تکمیل نامعتبر است."); return false; }
                deadline = dl;
            }

            var part = _cmbPart.SelectedItem as Part;
            if (part == null) { Msg.Show("ابتدا قطعات را تعریف کنید و یک قطعه انتخاب کنید."); return false; }
            if (_desc.Text.Trim().Length == 0) { Msg.Show("شرح عدم انطباق را وارد کنید."); return false; }

            _current.ProjectId = Session.ProjectId;
            _current.PartId = part.Id;
            _current.InstanceNo = (int)_instance.Value;
            _current.NcrDate = d;
            _current.Deadline = deadline;
            _current.ProductType = _productType.Text.Trim();
            _current.Stage = _stage.Text.Trim();
            _current.HallSupervisor = _supervisor.Text.Trim();
            _current.Inspector = _inspector.Text.Trim();
            _current.Preparer = _preparer.Text.Trim();
            _current.Severity = _severity.Text.Trim();
            _current.Description = _desc.Text.Trim();
            _current.TaskDescription = _task.Text.Trim();
            _current.SessionDuration = _session.Text.Trim();
            _current.ExecResult = _execResult.Text.Trim();
            _current.InspResult = _inspResult.Text.Trim();
            _current.ReviewDuration = _review.Text.Trim();
            _current.FinalTask = _final.Text.Trim();
            _current.QtyAccepted = (int)_qAccepted.Value;
            _current.QtyRework = (int)_qRework.Value;
            _current.QtyReturned = (int)_qReturned.Value;
            _current.QtyOtherUse = (int)_qOther.Value;
            _current.QtyScrap = (int)_qScrap.Value;
            _current.PartName = part.PartName;
            _current.InstanceTotal = part.Qty;
            return true;
        }

        static bool Blank(string? a, string? b, string? c, string? d)
        {
            return string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b) &&
                   string.IsNullOrWhiteSpace(c) && string.IsNullOrWhiteSpace(d);
        }

        void SaveRecord()
        {
            if (!Read()) return;

            var actions = _actions.Where(a => !Blank(a.Material, a.Tools, a.Executor, a.Execution)).ToList();
            var checks = _checks.Where(k => !(Blank(k.Spec, k.Tolerance, k.Method, k.Sampling) && string.IsNullOrWhiteSpace(k.Notes))).ToList();

            NcrRepo.Save(_current, actions, checks);
            Msg.Show("فرم عدم انطباق ذخیره شد.");

            _loading = true;
            LoadList();
            var row = _list.FirstOrDefault(x => x.Id == _current.Id);
            if (row != null)
            {
                _current = row;
                int idx = _list.IndexOf(row);
                _listGrid.ClearSelection();
                _listGrid.Rows[idx].Selected = true;
                _listGrid.CurrentCell = _listGrid.Rows[idx].Cells[1];
            }
            _loading = false;
        }

        void DeleteRecord()
        {
            if (_current.Id == 0) { NewRecord(); return; }
            if (MessageBox.Show("این مورد حذف شود؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign)
                != DialogResult.Yes) return;

            NcrRepo.Delete(_current.Id);
            _loading = true;
            LoadList();
            _loading = false;
            NewRecord();
        }

        void Print()
        {
            if (!Read()) return;
            var n = _current;
            var proj = Session.Project;

            var d1 = new ReportDoc
            {
                Title = "فرم کنترل محصول نامنطبق",
                Code = "کد مدرک: PU-FR-16/02",
                Revision = "تاریخ بازنگری: 1403/10/01",
                Rtl = true,
                InfoColumns = 1,
                MinRows = 4,
                Headers = new[] { "مواد", "ابزار و تجهیزات", "مجری", "شرح اجرا" },
                Widths = new float[] { 3, 3, 3, 6 },
                Signers = new[]
                {
                    "تهیه کننده:\n" + n.Preparer,
                    "مسئول کنترل کیفیت:\n" + proj.QcManagerFa
                }
            };
            d1.Info.Add(new[] { "نوع محصول نامنطبق:", n.ProductType });
            d1.Info.Add(new[] { "تاریخ:", n.NcrDateText });
            d1.Info.Add(new[] { "نام پروژه:", proj.NameFa });
            d1.Info.Add(new[] { "کارفرما:", proj.ClientFa });
            d1.Info.Add(new[] { "مسئول سالن مونتاژ:", n.HallSupervisor });
            d1.Info.Add(new[] { "نام مرحله:", n.Stage });
            d1.Info.Add(new[] { "نام قطعه / کد قطعه:", n.PartLabel });
            d1.Info.Add(new[] { "نام بازرس:", n.Inspector });
            d1.Info.Add(new[] { "شرح عدم انطباق:", n.Description });
            d1.Info.Add(new[] { "درجه اهمیت:", n.Severity });
            d1.Info.Add(new[] { "شرح تعیین تکلیف:", n.TaskDescription });
            d1.Info.Add(new[] { "دستور العمل اصلاح اقلام نامنطبق:", "" });
            foreach (var a in _actions.Where(a => !Blank(a.Material, a.Tools, a.Executor, a.Execution)))
                d1.Rows.Add(new[] { a.Material ?? "", a.Tools ?? "", a.Executor ?? "", a.Execution ?? "" });

            var d2 = new ReportDoc
            {
                Title = "فرم کنترل محصول نامنطبق (ادامه)",
                Code = "کد مدرک: PU-FR-16/02",
                Revision = "تاریخ بازنگری: 1403/10/01",
                Rtl = true,
                InfoColumns = 1,
                MinRows = 4,
                Headers = new[] { "مشخصه کنترلی", "تلرانس قابل قبول", "روش اندازه گیری", "تعداد و تناوب نمونه گیری", "توضیحات" },
                Widths = new float[] { 3, 3, 3, 3, 3 },
                Signers = new[]
                {
                    "تایید (امضاء) مسئول کنترل کیفیت:\n" + proj.QcManagerFa,
                    "امضاء بازرس:\n" + n.Inspector
                }
            };
            d2.Info.Add(new[] { "دستور العمل بازرسی و پذیرش:", "" });
            foreach (var k in _checks.Where(k => !(Blank(k.Spec, k.Tolerance, k.Method, k.Sampling) && string.IsNullOrWhiteSpace(k.Notes))))
                d2.Rows.Add(new[] { k.Spec ?? "", k.Tolerance ?? "", k.Method ?? "", k.Sampling ?? "", k.Notes ?? "" });
            // info lines print under the title band, so the check table follows them
            d2.Info.Add(new[] { "مدت جلسه تعیین تکلیف:", n.SessionDuration });
            d2.Info.Add(new[] { "مهلت تکمیل تا مورخ:", n.Deadline.HasValue ? Calendar.ToText(n.Deadline.Value) : "" });
            d2.Info.Add(new[] { "نتایج اجرا (اصلاح):", n.ExecResult });
            d2.Info.Add(new[] { "نتیجه بازرسی:", n.InspResult });
            d2.Info.Add(new[] { "مقدار پذیرفته شده:", n.QtyAccepted.ToString() });
            d2.Info.Add(new[] { "مقدار دوباره کاری:", n.QtyRework.ToString() });
            d2.Info.Add(new[] { "مقدار مرجوعی:", n.QtyReturned.ToString() });
            d2.Info.Add(new[] { "مقدار استفاده شده در موارد دیگر:", n.QtyOtherUse.ToString() });
            d2.Info.Add(new[] { "مقدار ضایعات:", n.QtyScrap.ToString() });
            d2.Info.Add(new[] { "مدت زمان بازرسی:", n.ReviewDuration });
            d2.Info.Add(new[] { "نتیجه نهایی تعیین تکلیف:", n.FinalTask });

            ReportPrinter.Preview(new List<ReportDoc> { d1, d2 });
        }
    }
}

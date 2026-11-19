using QcApp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // The three ways the PDF asks for to pick parts: one by one, from the list, or random by percent.
    [System.ComponentModel.DesignerCategory("Code")]   // built in code: open it with View Code, not the designer
    public class PartPickerDialog : Form
    {
        readonly List<Part> _parts;
        readonly List<PartPick> _all = new List<PartPick>();

        readonly TabControl _tabs = new TabControl { Dock = DockStyle.Fill };
        readonly ComboBox _cmbPart = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        readonly NumericUpDown _numInstance = new NumericUpDown { Minimum = 1, Maximum = 1, Value = 1, Width = 80 };
        readonly BindingList<PartPick> _manual = new BindingList<PartPick>();
        readonly ListBox _lstManual = new ListBox { Dock = DockStyle.Fill, DisplayMember = "Label" };
        readonly CheckedListBox _lstAll = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true };
        readonly NumericUpDown _percent = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 20, Width = 80 };
        readonly Label _lblRandom = new Label { AutoSize = true, Margin = new Padding(6) };

        public List<PartPick> Picks { get; private set; } = new List<PartPick>();

        public PartPickerDialog(List<Part> parts)
        {
            _parts = parts;
            foreach (var p in parts)
                for (int k = 1; k <= p.Qty; k++)
                    _all.Add(new PartPick { PartId = p.Id, PartName = p.PartName, InstanceNo = k, InstanceTotal = p.Qty });

            Text = "افزودن قطعات";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10);
            Width = 560; Height = 520;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
            Controls.Add(root);
            root.Controls.Add(_tabs, 0, 0);

            BuildManualTab();
            BuildListTab();
            BuildRandomTab();

            var ok = Ui.Btn("تایید", 110);
            var cancel = Ui.Btn("انصراف", 110);
            ok.Click += (s, e) => Confirm();
            cancel.DialogResult = DialogResult.Cancel;
            var bar = new FlowLayoutPanel { Dock = DockStyle.Fill, RightToLeft = RightToLeft.Yes };
            bar.Controls.Add(ok);
            bar.Controls.Add(cancel);
            root.Controls.Add(bar, 0, 1);
            CancelButton = cancel;
        }

        void BuildManualTab()
        {
            var tab = new TabPage("روش اول: درج مورد به مورد");
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 70, RightToLeft = RightToLeft.Yes };
            _cmbPart.DataSource = _parts;
            _cmbPart.DisplayMember = "PartName";
            _cmbPart.SelectedIndexChanged += (s, e) => SyncInstanceMax();
            SyncInstanceMax();

            var add = Ui.Btn("افزودن", 90);
            add.Margin = new Padding(6, 24, 6, 6);
            add.Click += (s, e) =>
            {
                var p = (Part)_cmbPart.SelectedItem;
                int n = (int)_numInstance.Value;
                if (_manual.Any(x => x.PartId == p.Id && x.InstanceNo == n)) return;   // no repeats
                _manual.Add(new PartPick { PartId = p.Id, PartName = p.PartName, InstanceNo = n, InstanceTotal = p.Qty });
            };
            var remove = Ui.Btn("حذف", 90);
            remove.Margin = new Padding(6, 24, 6, 6);
            remove.Click += (s, e) =>
            {
                var x = _lstManual.SelectedItem as PartPick;
                if (x != null) _manual.Remove(x);
            };

            top.Controls.Add(Ui.Field("نام قطعه", _cmbPart, 200));
            top.Controls.Add(Ui.Field("شماره", _numInstance, 80));
            top.Controls.Add(add);
            top.Controls.Add(remove);

            _lstManual.DataSource = _manual;
            tab.Controls.Add(_lstManual);
            tab.Controls.Add(top);
            _tabs.TabPages.Add(tab);
        }

        void SyncInstanceMax()
        {
            var p = _cmbPart.SelectedItem as Part;
            _numInstance.Maximum = p == null ? 1 : Math.Max(1, p.Qty);
            if (_numInstance.Value > _numInstance.Maximum) _numInstance.Value = _numInstance.Maximum;
        }

        void BuildListTab()
        {
            var tab = new TabPage("روش دوم: انتخاب از لیست قطعات");
            foreach (var x in _all) _lstAll.Items.Add(x.Label);

            var all = Ui.Btn("انتخاب همه", 110);
            var none = Ui.Btn("پاک کردن انتخاب", 130);
            all.Click += (s, e) => { for (int i = 0; i < _lstAll.Items.Count; i++) _lstAll.SetItemChecked(i, true); };
            none.Click += (s, e) => { for (int i = 0; i < _lstAll.Items.Count; i++) _lstAll.SetItemChecked(i, false); };

            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, RightToLeft = RightToLeft.Yes };
            top.Controls.Add(all);
            top.Controls.Add(none);

            tab.Controls.Add(_lstAll);
            tab.Controls.Add(top);
            _tabs.TabPages.Add(tab);
        }

        void BuildRandomTab()
        {
            var tab = new TabPage("روش سوم: رندم (خودکار)");
            var top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 120, RightToLeft = RightToLeft.Yes, WrapContents = true };
            _percent.ValueChanged += (s, e) => UpdateRandomLabel();
            top.Controls.Add(Ui.Field("درصد قطعات مورد بازرسی", _percent, 80));
            top.Controls.Add(_lblRandom);
            tab.Controls.Add(top);
            _tabs.TabPages.Add(tab);
            UpdateRandomLabel();
        }

        int RandomCount()
        {
            return QcRules.RandomCount(_all.Count, _percent.Value);
        }

        void UpdateRandomLabel()
        {
            _lblRandom.Text = "از مجموع " + _all.Count + " قطعه، " + RandomCount() + " قطعه به صورت تصادفی انتخاب می‌شود.";
        }

        void Confirm()
        {
            if (_tabs.SelectedIndex == 0)
            {
                Picks = _manual.ToList();
            }
            else if (_tabs.SelectedIndex == 1)
            {
                Picks = new List<PartPick>();
                foreach (int i in _lstAll.CheckedIndices) Picks.Add(_all[i]);
            }
            else
            {
                var rnd = new Random();
                Picks = _all.OrderBy(x => rnd.Next()).Take(RandomCount()).ToList();
                // keep the original part order so the report reads in sequence
                Picks = Picks.OrderBy(x => _all.IndexOf(x)).ToList();
            }

            if (Picks.Count == 0) { Msg.Show("هیچ قطعه‌ای انتخاب نشده است."); return; }
            DialogResult = DialogResult.OK;
        }
    }
}

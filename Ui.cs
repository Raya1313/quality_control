#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QcApp
{
    // small helpers so every form is built the same way
    public static class Ui
    {
        public static readonly Color Purple = ColorTranslator.FromHtml("#E6D5F2");
        public static readonly Color Green = ColorTranslator.FromHtml("#00B050");

        public static void Setup(Form f, string title, int width, int height)
        {
            f.Text = title;
            f.Width = width;
            f.Height = height;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.RightToLeft = RightToLeft.Yes;
            f.RightToLeftLayout = false;
            f.Font = new Font("Tahoma", 9f);
        }

        public static RowStyle Abs(int h) { return new RowStyle(SizeType.Absolute, h); }
        public static RowStyle Fill() { return new RowStyle(SizeType.Percent, 100); }
        public static RowStyle Auto() { return new RowStyle(SizeType.AutoSize); }

        public static TableLayoutPanel Root(Form f, params RowStyle[] rows)
        {
            var t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = rows.Length
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var r in rows) t.RowStyles.Add(r);
            f.Controls.Add(t);
            return t;
        }

        // purple banner: logo on the right, title in the middle, optional control on the left
        public static Control Banner(string title, out PictureBox logo, Control? leftControl)
        {
            var t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Purple,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            logo = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Session.LogoImage()
            };
            var lbl = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 13f, FontStyle.Bold)
            };

            t.Controls.Add(logo, 0, 0);
            t.Controls.Add(lbl, 1, 0);
            if (leftControl != null)
            {
                leftControl.Dock = DockStyle.Fill;
                leftControl.Margin = new Padding(15, 25, 15, 25);
                t.Controls.Add(leftControl, 2, 0);
            }
            return t;
        }

        public static Label Caption(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Tahoma", 9f, FontStyle.Bold)
            };
        }

        public static Button Btn(string text, int width)
        {
            return new Button { Text = text, Width = width, Height = 32, Margin = new Padding(6) };
        }

        // small caption above an input
        public static Control Field(string caption, Control input, int width)
        {
            var p = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false,
                Margin = new Padding(6)
            };
            p.Controls.Add(new Label { Text = caption, AutoSize = true });
            input.Width = width;
            p.Controls.Add(input);
            return p;
        }

        public static DataGridView Grid()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = true,
                BackgroundColor = Color.White,
                RightToLeft = RightToLeft.Yes
            };
            g.DataError += GridError;      // every grid built here never shows the default error dialog
            return g;
        }
        public static void NumberRows(DataGridView g)
        {
            g.RowPostPaint += (s, e) =>
            {
                var grid = (DataGridView)s!;
                var text = (e.RowIndex + 1).ToString();

                using var brush = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor);

                var bounds = new Rectangle(
                    e.RowBounds.Left,
                    e.RowBounds.Top,
                    grid.RowHeadersWidth,
                    e.RowBounds.Height);

                TextRenderer.DrawText(
                    e.Graphics,
                    text,
                    grid.Font,
                    bounds,
                    grid.RowHeadersDefaultCellStyle.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
        }
        static bool _gridErrorShowing;

        // Replaces the default "DataGridView Default Error Dialog" (wrong type, value not valid, ...).
        // Hook it on any grid with:  grid.DataError += Ui.GridError;
        public static void GridError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;

            // while painting or measuring nobody typed anything, so just leave the cell blank
            const DataGridViewDataErrorContexts typed =
                DataGridViewDataErrorContexts.Parsing |
                DataGridViewDataErrorContexts.Commit |
                DataGridViewDataErrorContexts.LeaveControl;
            if ((e.Context & typed) == 0) return;
            if (_gridErrorShowing) return;

            string col = "";
            var g = sender as DataGridView;
            if (g != null && e.ColumnIndex >= 0 && e.ColumnIndex < g.Columns.Count)
                col = g.Columns[e.ColumnIndex].HeaderText.Replace("\r", " ").Replace("\n", " ").Trim();

            _gridErrorShowing = true;
            try
            {
                Msg.Show(col.Length > 0
                    ? "مقدار واردشده برای «" + col + "» معتبر نیست. مقدار درست را وارد کنید."
                    : "مقدار واردشده معتبر نیست.");
            }
            finally { _gridErrorShowing = false; }
        }

        public static void AddCol(DataGridView g, string header, string property)
        {
            g.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = property
            });
        }

        public const string RowHeader = "ردیف";

        // the column headed "ردیف" (or the first column) shows 1, 2, 3 ... automatically.
        // It is computed while painting, so it is never blank and stays right after add/delete/re-bind.
        public static void NumberFirstColumn(DataGridView g)
        {
            g.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                var col = g.Columns[e.ColumnIndex];
                bool isNumber = col.HeaderText.Trim() == RowHeader || (e.ColumnIndex == 0 && col.DataPropertyName == "");
                if (!isNumber) return;
                e.Value = (e.RowIndex + 1).ToString();
                e.FormattingApplied = true;
            };
            g.RowsAdded += (s, e) => g.Invalidate();
            g.RowsRemoved += (s, e) => g.Invalidate();
            g.DataBindingComplete += (s, e) => g.Invalidate();
        }

        // delete button, hint, green confirm button
        public static Control BottomBar(Button delete, string hint, Button confirm)
        {
            var t = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = new Padding(0)
            };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 240));
            t.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            delete.Dock = DockStyle.Fill;
            delete.Margin = new Padding(8);

            var lbl = new Label
            {
                Text = hint,
                Dock = DockStyle.Fill,
                BackColor = Color.Yellow,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(8)
            };

            confirm.Dock = DockStyle.Fill;
            confirm.Margin = new Padding(8);
            confirm.BackColor = Green;
            confirm.ForeColor = Color.White;
            confirm.FlatStyle = FlatStyle.Flat;

            t.Controls.Add(delete, 0, 0);
            t.Controls.Add(lbl, 1, 0);
            t.Controls.Add(confirm, 2, 0);
            return t;
        }
    }
}
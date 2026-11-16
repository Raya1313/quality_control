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
        public static Control Banner(string title, out PictureBox logo, Control leftControl)
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
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                RightToLeft = RightToLeft.Yes
            };
        }

        public static void AddCol(DataGridView g, string header, string property)
        {
            g.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = header,
                DataPropertyName = property
            });
        }

        // first column of the grid shows 1, 2, 3 ...
        public static void NumberFirstColumn(DataGridView g)
        {
            g.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == 0)
                {
                    e.Value = e.RowIndex + 1;
                    e.FormattingApplied = true;
                }
            };
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

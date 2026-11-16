using QcApp;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1.Forms
{
    // Entry point after the project is chosen: one button per section of the PDF.
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            Ui.Setup(this, "نرم افزار کنترل کیفیت", 760, 640);

            var root = Ui.Root(this, Ui.Abs(90), Ui.Fill());
            PictureBox logo;
            root.Controls.Add(Ui.Banner("پروژه: " + Session.Project.NameFa, out logo, null), 0, 0);

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = true,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(24)
            };
            root.Controls.Add(flow, 0, 1);

            Add(flow, "بخش اول: تعریف قطعات", () => Open(new PartsForm()));
            Add(flow, "بخش اول: تعریف اتصالات", () => Open(new ConnectionsForm()));
            Add(flow, "بخش دوم: چک لیست قبل و حین جوشکاری", () => Open(new WeldChecklistForm()));
            Add(flow, "بخش سوم: کنترل ابعادی (DT)", () => Open(new DimensionalForm()));
            Add(flow, "بخش چهارم: بازرسی چشمی (VT)", () => Open(new VisualForm()));
            Add(flow, "بخش پنجم: بازرسی فراصوت (UT)", () => Open(new UltrasonicForm()));
            Add(flow, "بخش ششم: بازرسی مایع نافذ (PT)", () => Open(new PenetrantForm()));
            Add(flow, "بخش هفتم: فرم عدم انطباق", () => Open(new NcrForm()));
        }

        static void Add(FlowLayoutPanel flow, string text, Action open)
        {
            var b = new Button
            {
                Text = text,
                Width = 320,
                Height = 64,
                Margin = new Padding(10),
                Font = new Font("Tahoma", 10f, FontStyle.Bold),
                BackColor = Ui.Purple,
                FlatStyle = FlatStyle.Flat
            };
            b.Click += (s, e) => open();
            flow.Controls.Add(b);
        }

        static void Open(Form f)
        {
            using (f) f.ShowDialog();
        }
    }
}

using System;
using System.Windows.Forms;
using Data;
using WindowsFormsApp1.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // continue with the last project instead of inserting a new one on every launch
            var last = ProjectRepo.LoadLast();
            bool reuse = last != null &&
                MessageBox.Show("پروژه «" + last.NameFa + "» ادامه یابد؟\n(خیر = ثبت پروژه جدید)", "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == DialogResult.Yes;

            if (reuse && last != null)
            {
                Session.ProjectId = last.Id;
                Session.Project = last;
            }
            else
            {
                using (var f = new ProjectForm())
                {
                    if (f.ShowDialog() != DialogResult.OK) return;
                }
            }

            Application.Run(new MainMenuForm());
        }
    }
}

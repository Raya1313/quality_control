using Data;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp1.Forms
{
    public partial class ProjectForm : Form
    {
        byte[]? _logo;

        public ProjectForm()
        {
            InitializeComponent();
        }

        void btnLogo_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog { Filter = "Images|*.png;*.jpg;*.jpeg" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                _logo = File.ReadAllBytes(dlg.FileName);
                picLogo.Image = Image.FromStream(new MemoryStream(_logo));
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(bbName.Fa))
            {
                Msg.Show("نام پروژه را وارد کنید.");
                return;
            }

            DateTime contractDate;
            if (!Calendar.TryParse(mtbContractDate.Text, out contractDate))
            {
                Msg.Show("تاریخ عقد قرارداد نامعتبر است.");
                mtbContractDate.Focus();
                return;
            }

            var p = new Project
            {
                NameFa = bbName.Fa,
                NameEn = bbName.En,
                ClientFa = bbClient.Fa,
                ClientEn = bbClient.En,
                ContractorFa = bbContractor.Fa,
                ContractorEn = bbContractor.En,
                QcManagerFa = bbQc.Fa,
                QcManagerEn = bbQc.En,
                QaManagerFa = bbQa.Fa,
                QaManagerEn = bbQa.En,
                CoordinatorFa = bbCoordinator.Fa,
                CoordinatorEn = bbCoordinator.En,
                ContractDate = contractDate,
                PhaseCount = (int)numPhases.Value,
                ReportedPhase = txtReportedPhase.Text,
                Logo = _logo
            };

            p.Id = ProjectRepo.Insert(p);
            Session.ProjectId = p.Id;
            Session.Project = p;
            DialogResult = DialogResult.OK;
        }

        private void ProjectForm_Load(object sender, EventArgs e)
        {

        }

        private void pnlBanner_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }


        private void tlp_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        private void lblPhaseCount_Click(object sender, EventArgs e) { }
        private void bbCoordinator_Load(object sender, EventArgs e) { }
    }
}

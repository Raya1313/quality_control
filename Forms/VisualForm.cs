using Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WindowsFormsApp1.Forms
{
    // Section 4: visual test (VT)
    [System.ComponentModel.DesignerCategory("Code")]   // built in code: open it with View Code, not the designer
    public class VisualForm : InspectionBase
    {
        readonly BindingList<VtRow> _rows = new BindingList<VtRow>();
        protected override IList RowsList { get { return _rows; } }

        public VisualForm() : base("VT", "بخش چهارم (بازرسی چشمی VT)", false)
        {
            AddDate("StartDate", "تاریخ شروع بازرسی چشمی (VT)");
            AddNum("Gap", "فواصل روزهای گزارش", 1, 1, 60);
            AddText("BaseMetal", "نوع فلز مورد استفاده", 120, "ST37");
            AddText("Filler", "فلز پرکننده", 150, "ER70S-6 & EM11");
            AddText("Standard", "استاندارد مورد استفاده", 120, "AWS D1.1");
            AddText("Subject", "Inspection Subject", 120, "Base Metal");
            AddText("Process", "Production Process", 120, "");
            AddText("Stage", "Production Stage", 120, "Semi Finished");

            Txt("ردیف", "", 45, true);
            Txt("کد قطعه", "PartLabel", 120, true);
            Txt("نوع", "ItemType", 80, false);
            Chk("Porosity", "Porosity", 65);
            Chk("Under cut", "Undercut", 65);
            Chk("Crack", "Crack", 55);
            Chk("End Crater", "EndCrater", 65);
            Chk("LOF", "Lof", 50);
            Chk("Other", "Other", 55);
            Combo("نتیجه", "Result", 170,
                new KeyValuePair<string, string>("Accepted", "پذیرفته (Accepted)"),
                new KeyValuePair<string, string>("Repair", "تعمیر (Repair)"),
                new KeyValuePair<string, string>("AcceptedAfterRepair", "پذیرفته پس از تعمیر"));
            Txt("موقعیت عیب", "DefectPosition", 200, false);

            Start();
        }

        // PDF page 1: C = column, B = beam, R / RA = rafter; a leading number is the phase (2C4 = phase 2)
        static string GuessType(string partName)
        {
            return QcRules.PartType(partName);
        }

        protected override void AddRowsFor(PartPick p)
        {
            _rows.Add(new VtRow
            {
                PartId = p.PartId, PartName = p.PartName, InstanceNo = p.InstanceNo, InstanceTotal = p.InstanceTotal,
                ItemType = GuessType(p.PartName)
            });
        }

        protected override void Persist(Dictionary<string, string> p, byte[]? sketch)
        {
            InspectionRepo.Save(Session.ProjectId, Kind, p, sketch, InspectionRepo.VtSql, _rows);
        }

        protected override void LoadRowsFor(int reportId)
        {
            foreach (var r in InspectionRepo.LoadRows<VtRow>("VtRow", reportId)) _rows.Add(r);
        }

        protected override List<ReportDoc> BuildDocs()
        {
            var d = new ReportDoc
            {
                Title = "Visual Test Report",
                Code = "Code: QC-FR-04/02",
                Revision = "Revision Date: 1403/10/01",
                InfoColumns = 2,
                NumberBlankRows = true,
                MinRows = 20,
                Headers = new[] { "NO", "Name", "Type", "Porosity", "Under cut", "Crack", "End Crater", "LOF", "Other",
                                  "Accepted", "Repair", "Accepted Repair", "Defective Position" },
                Widths = new float[] { 3, 10, 7, 4, 4, 4, 4, 4, 4, 5, 5, 5, 14 },
                Signers = new[]
                {
                    "Q.C Department\nName: " + QcEn() + "\nSignature:",
                    "Approved by\nName: " + CoordEn() + "\nSignature:",
                    "Client's representative\nName: " + ClientEn() + "\nSignature:"
                }
            };
            d.Info.Add(new[] { "Client:", ClientEn() });
            d.Info.Add(new[] { "Material:", P("BaseMetal") });
            d.Info.Add(new[] { "Project name:", ProjectEn() });
            d.Info.Add(new[] { "Filler metal:", P("Filler") });
            d.Info.Add(new[] { "Manufacturer:", ContractorEn() });
            d.Info.Add(new[] { "Standard/code:", P("Standard") });
            d.Info.Add(new[] { "Faze:", PhaseText() });
            d.Info.Add(new[] { "Exam Date:", P("StartDate") });
            d.Info.Add(new[] { "Inspection Subject:", P("Subject") });
            d.Info.Add(new[] { "Production Stage:", P("Stage") });
            d.Info.Add(new[] { "Production Process:", P("Process") });
            d.Info.Add(new[] { "", "" });

            int n = 1;
            foreach (var r in _rows)
            {
                bool acc = r.Result == "Accepted";
                bool rep = r.Result == "Repair" || r.Result == "AcceptedAfterRepair";
                bool accRep = r.Result == "AcceptedAfterRepair";
                d.Rows.Add(new[]
                {
                    n.ToString(), r.PartLabel, r.ItemType,
                    Mark(r.Porosity), Mark(r.Undercut), Mark(r.Crack), Mark(r.EndCrater), Mark(r.Lof), Mark(r.Other),
                    Mark(acc), Mark(rep), Mark(accRep), r.DefectPosition ?? ""
                });
                n++;
            }
            return new List<ReportDoc> { d };
        }
    }
}

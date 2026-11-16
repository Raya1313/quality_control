using Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WindowsFormsApp1.Forms
{
    // Section 6: liquid penetrant (PT)
    public class PenetrantForm : InspectionBase
    {
        readonly BindingList<PtRow> _rows = new BindingList<PtRow>();
        protected override IList RowsList { get { return _rows; } }

        public PenetrantForm() : base("PT", "بخش ششم (بازرسی مایع نافذ PT)", true)
        {
            AddDate("StartDate", "تاریخ شروع بازرسی مایع نافذ (PT)");
            AddNum("Gap", "فواصل روزهای گزارش", 1, 1, 60);
            AddText("Percent", "Percent Of Test", 90, "%10 ~ %20");
            AddText("Method", "Exam Method", 140, "Post Emulsification");
            AddText("Finish", "Surface Finish", 80, "Wet");
            AddText("Clean", "Method Clean", 80, "Brushed");
            AddText("Process", "Welding Process", 90, "Base Metal");
            AddText("Contrast", "Contrast", 100, "Color Visible");
            AddText("Temp", "Surface Temp", 80, "30-32 C");
            AddText("Material", "Material", 70, "ST37");
            AddText("Developer", "Type Of Developer", 100, "Solvent based");
            AddText("Dwell", "Dwell Time Penetrante", 80, "14 min");
            AddText("DevTime", "Developing Time", 80, "10~15min");
            AddText("Length", "Length Of R/P (mm)", 80, "800");
            AddText("Maker", "Materials Manufacturer", 100, "");
            AddText("BatchPen", "Batch No. Penetrant", 100, "");
            AddText("BatchDev", "Batch No. Developer", 100, "");
            AddText("Standard", "Applicable STD / SPEC", 100, "AWS D1.1");
            AddCombo("Judgement", "Judgement", 90, "Accept", "Reject");

            Txt("ردیف", "", 45, true);
            Txt("کد قطعه", "PartLabel", 120, true);
            Txt("Position", "Position", 130, false);
            Txt("Level / Direction", "LevelDir", 110, false);
            Combo("Type Of Discontinuity", "DiscType", 110,
                new KeyValuePair<string, string>("", ""),
                new KeyValuePair<string, string>("C", "C (Crack)"),
                new KeyValuePair<string, string>("C.C", "C.C (Crater Crack)"),
                new KeyValuePair<string, string>("L.O.F", "L.O.F"),
                new KeyValuePair<string, string>("PO", "PO (Porosity)"));
            Txt("Size (mm)", "DiscSizeMm", 70, false);
            Combo("نتیجه", "Result", 90,
                new KeyValuePair<string, string>("ACC", "ACC"),
                new KeyValuePair<string, string>("REP", "Rep."));
            Chk("After Rep.", "AfterRep", 70);

            Start();
        }

        protected override void AddRowsFor(PartPick p)
        {
            _rows.Add(new PtRow
            {
                PartId = p.PartId, PartName = p.PartName, InstanceNo = p.InstanceNo, InstanceTotal = p.InstanceTotal,
                Position = "Corner Joint CJP"
            });
        }

        protected override void Persist(Dictionary<string, string> p, byte[]? sketch)
        {
            InspectionRepo.Save(Session.ProjectId, Kind, p, sketch, InspectionRepo.PtSql, _rows);
        }

        protected override void LoadRowsFor(int reportId)
        {
            foreach (var r in InspectionRepo.LoadRows<PtRow>("PtRow", reportId)) _rows.Add(r);
        }

        protected override List<ReportDoc> BuildDocs()
        {
            var d = new ReportDoc
            {
                Title = "Liquid Penetrant Inspection Report",
                Code = "Code: QC-FR-04/02",
                Revision = "Revision Date: 1403/10/01",
                InfoColumns = 2,
                MinRows = 15,
                Sketch = SketchImage(Sketch),
                Headers = new[] { "Row", "Name", "Position", "Level / Direction", "Type Of Discontinuity",
                                  "Discontinuity Size (mm)", "ACC.", "Rep.", "After Rep." },
                Widths = new float[] { 3, 10, 12, 10, 10, 10, 5, 5, 6 },
                Signers = new[]
                {
                    "Manufacturer\nName: " + ContractorEn() + "\nSignature:",
                    "T.P.I\nName: " + QcEn() + "\nSignature:",
                    "Client\nName: " + ClientEn() + "\nSignature:"
                }
            };
            d.Info.Add(new[] { "Project:", ProjectEn() });
            d.Info.Add(new[] { "Phase:", PhaseText() });
            d.Info.Add(new[] { "Client:", ClientEn() });
            d.Info.Add(new[] { "Date:", P("StartDate") });
            d.Info.Add(new[] { "T.P.I:", QcEn() });
            d.Info.Add(new[] { "Welding Process:", P("Process") });
            d.Info.Add(new[] { "Manufacturer:", ContractorEn() });
            d.Info.Add(new[] { "Material:", P("Material") });
            d.Info.Add(new[] { "Percent Of Test:", P("Percent") });
            d.Info.Add(new[] { "Applicable STD / SPEC:", P("Standard") });
            d.Info.Add(new[] { "Exam Method:", P("Method") });
            d.Info.Add(new[] { "Contrast:", P("Contrast") });
            d.Info.Add(new[] { "Type Of Developer:", P("Developer") });
            d.Info.Add(new[] { "Materials Manufacturer:", P("Maker") });
            d.Info.Add(new[] { "Batch No. Penetrant:", P("BatchPen") });
            d.Info.Add(new[] { "Batch No. Developer:", P("BatchDev") });
            d.Info.Add(new[] { "Surface Finish:", P("Finish") });
            d.Info.Add(new[] { "Surface Temp:", P("Temp") });
            d.Info.Add(new[] { "Dwell Time Penetrante:", P("Dwell") });
            d.Info.Add(new[] { "Developing Time:", P("DevTime") });
            d.Info.Add(new[] { "Method Clean:", P("Clean") });
            d.Info.Add(new[] { "Remark Defect:", "Crack: C   Crater Crack: C.C   Lack Of Fusion: L.O.F   Porosity: PO" });
            d.Info.Add(new[] { "Length Of R/P (mm):", P("Length") });
            d.Info.Add(new[] { "Judgement:", P("Judgement") });

            int n = 1;
            foreach (var r in _rows)
            {
                bool acc = r.Result == "ACC";
                d.Rows.Add(new[]
                {
                    n.ToString(), r.PartLabel, r.Position ?? "", r.LevelDir ?? "", r.DiscType ?? "",
                    r.DiscSizeMm.HasValue ? r.DiscSizeMm.Value.ToString("0.##") : "",
                    Mark(acc), Mark(!acc), Mark(r.AfterRep)
                });
                n++;
            }
            return new List<ReportDoc> { d };
        }
    }
}

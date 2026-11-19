using Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WindowsFormsApp1.Forms
{
    // Section 5: ultrasonic inspection (UT), page 1 (spec + sketch) and page 2 (weld table)
    [System.ComponentModel.DesignerCategory("Code")]   // built in code: open it with View Code, not the designer
    public class UltrasonicForm : InspectionBase
    {
        readonly BindingList<UtRow> _rows = new BindingList<UtRow>();
        protected override IList RowsList { get { return _rows; } }

        public UltrasonicForm() : base("UT", "بخش پنجم (بازرسی فراصوت UT)", true)
        {
            AddDate("StartDate", "تاریخ شروع بازرسی فراصوت (UT)");
            AddNum("Gap", "فواصل روزهای گزارش", 1, 1, 60);
            AddNum("WeldsPerPart", "تعداد جوش (W) برای هر قطعه", 3, 1, 20, 140);

            AddText("Model", "Model", 100, "Sonatest");
            AddText("Freq", "Freq. (Mhz)", 70, "2 Mhz");
            AddText("TypeSer", "Type/Ser. No", 80, "D-10+");
            AddText("IndexMm", "Index (mm)", 70, "8");
            AddText("Probe", "Probe No.", 90, "220633");
            AddText("Angle", "Angle (°)", 70, "70°");
            AddText("Size", "Size (mm)", 80, "20×22");

            AddText("Material", "Material", 70, "ST37");
            AddText("Groove", "Groove Type", 80, "SV & SB");
            AddText("WeldProc", "Welding Process", 100, "GMAW & SAW");
            AddText("Standard", "Code/Standard", 90, "AWS D1.1");
            AddText("Edition", "Edition", 70, "2020");
            AddText("RefDb", "Reference dB", 80, "66");
            AddText("ScanDb", "Scanning dB", 80, "80");
            AddText("Couplant", "Couplant", 130, "Dishwashing Liquid");
            AddText("Inspector", "Inspector / Level", 140, "");

            Txt("ردیف", "", 45, true);
            Txt("کد قطعه", "PartLabel", 120, true);
            Txt("Weld", "WeldId", 55, false);
            Txt("Welder No.", "WelderNo", 75, false);
            Txt("Scanning Surface", "ScanSurface", 75, false);
            Txt("Thk. (mm)", "ThicknessMm", 60, false);
            Txt("Tested Length (mm)", "TestedLengthMm", 80, false);
            Txt("Probe No.", "ProbeAngle", 65, false);
            Txt("X", "IndX", 50, false);
            Txt("Y", "IndY", 50, false);
            Txt("D", "IndD", 50, false);
            Txt("L", "IndL", 50, false);
            Txt("Interpretation", "Interpretation", 110, false);
            Chk("Acc. Yes", "Accepted", 60);
            Txt("Remarks", "Remarks", 150, false);

            Start();
        }

        // one row per weld (W1, W2, ...) of each picked part instance
        protected override void AddRowsFor(PartPick p)
        {
            int welds = (int)decimal.Parse(P("WeldsPerPart"), System.Globalization.CultureInfo.InvariantCulture);
            for (int w = 1; w <= welds; w++)
            {
                _rows.Add(new UtRow
                {
                    PartId = p.PartId, PartName = p.PartName, InstanceNo = p.InstanceNo, InstanceTotal = p.InstanceTotal,
                    WeldId = "W" + w,
                    ScanSurface = "A",
                    ProbeAngle = P("Angle")
                });
            }
        }

        protected override void Persist(Dictionary<string, string> p, byte[]? sketch)
        {
            InspectionRepo.Save(Session.ProjectId, Kind, p, sketch, InspectionRepo.UtSql, _rows);
        }

        protected override void LoadRowsFor(int reportId)
        {
            foreach (var r in InspectionRepo.LoadRows<UtRow>("UtRow", reportId)) _rows.Add(r);
        }

        static string Num(decimal? v) { return v.HasValue ? v.Value.ToString("0.##") : ""; }

        const int RowsPerPage = 20;          // the table of the PDF sample (page 2 of 2) has 20 rows

        // page 1 names the inspected items once, e.g. "C1  1/11, 2/11 ; B2  1/4"
        static string ItemList(IEnumerable<UtRow> rows)
        {
            var parts = new List<string>();
            foreach (var g in rows.GroupBy(r => r.PartId))
            {
                var first = g.First();
                var numbers = g.Select(r => r.InstanceNo).Distinct().OrderBy(i => i)
                               .Select(i => i + "/" + first.InstanceTotal);
                parts.Add(first.PartName + "  " + string.Join(", ", numbers));
            }
            return string.Join(" ; ", parts);
        }

        // One specification page (the sketch and test data are the same for every item), then the weld table
        // with 20 rows to a page. The page count follows the number of rows, not 2 pages for every part.
        protected override List<ReportDoc> BuildDocs()
        {
            var docs = new List<ReportDoc>();
            var rows = _rows.ToList();
            int tablePages = QcRules.TablePages(rows.Count, RowsPerPage);
            int total = 1 + tablePages;
            bool manyItems = rows.Select(r => r.PartId + ":" + r.InstanceNo).Distinct().Count() > 1;

            var p1 = new ReportDoc
            {
                Title = "ULTRASONIC INSPECTION REPORT",
                Code = "Process ID: QC   FR   No: 6   Rev.: 2",
                Revision = "PAGE : 1 OF " + total,
                InfoColumns = 2,
                Sketch = SketchImage(Sketch),
                SketchCaption = "Sketch:"
            };
            p1.Info.Add(new[] { "Project:", ProjectEn() });
            p1.Info.Add(new[] { "Request No:", "" });
            p1.Info.Add(new[] { "Client:", ClientEn() });
            p1.Info.Add(new[] { "Faze:", PhaseText() });
            p1.Info.Add(new[] { "Manufacturer:", ContractorEn() });
            p1.Info.Add(new[] { "Exam Date/Time:", P("StartDate") });
            p1.Info.Add(new[] { "Item Name:", ItemList(rows) });
            p1.Info.Add(new[] { "Material:", P("Material") });
            p1.Info.Add(new[] { "Production Process:", "Welding" });
            p1.Info.Add(new[] { "Surface Condition:", "As Welded" });
            p1.Info.Add(new[] { "Testing Subject:", "Weld Metal" });
            p1.Info.Add(new[] { "Inspection Method:", "Contact / Pulse Echo" });
            p1.Info.Add(new[] { "Groove Type:", P("Groove") });
            p1.Info.Add(new[] { "Welding Process:", P("WeldProc") });
            p1.Info.Add(new[] { "Code/Standard:", P("Standard") });
            p1.Info.Add(new[] { "Edition:", P("Edition") });
            p1.Info.Add(new[] { "Reference dB:", P("RefDb") });
            p1.Info.Add(new[] { "Scanning dB:", P("ScanDb") });
            p1.Info.Add(new[] { "Couplant:", P("Couplant") });
            p1.Info.Add(new[] { "Sensitivity Calibration:", "SDH" });
            p1.Info.Add(new[] { "Examination Model:", P("Model") });
            p1.Info.Add(new[] { "Type/Ser. No.:", P("TypeSer") });
            p1.Info.Add(new[] { "Probe No.:", P("Probe") });
            p1.Info.Add(new[] { "Freq. (Mhz):", P("Freq") });
            p1.Info.Add(new[] { "Size (mm):", P("Size") });
            p1.Info.Add(new[] { "Angle (°):", P("Angle") });
            p1.Info.Add(new[] { "Index (mm):", P("IndexMm") });
            p1.Info.Add(new[] { "", "" });
            docs.Add(p1);

            for (int page = 0; page < tablePages; page++)
            {
                // with more than one item an "Item" column says which part each weld belongs to
                var headers = new List<string> { "Identification" };
                var widths = new List<float> { 5 };
                if (manyItems) { headers.Add("Item"); widths.Add(7); }
                headers.AddRange(new[] { "Weld", "Welder No.", "Scanning Surface / Dir.", "Thk. (mm)",
                                         "Tested Length (mm)", "Probe No.", "X", "Y", "D", "L", "Interpretation",
                                         "Acc. Yes", "Acc. No", "Remarks" });
                widths.AddRange(new float[] { 4, 6, 8, 5, 7, 5, 4, 4, 4, 4, 8, 4, 4, 10 });

                var p2 = new ReportDoc
                {
                    Title = "ULTRASONIC INSPECTION REPORT",
                    Code = "Process ID: QC   FR   No: 6   Rev.: 2",
                    Revision = "PAGE : " + (page + 2) + " OF " + total,
                    InfoColumns = 2,
                    Landscape = true,
                    MinRows = RowsPerPage,
                    Headers = headers.ToArray(),
                    Widths = widths.ToArray(),
                    Signers = new[]
                    {
                        "Inspector: " + P("Inspector") + "\nReport Date: " + P("StartDate"),
                        "Approved by: " + QcEn() + "\nReport Date: " + P("StartDate"),
                        "Client's representative:\nReport Date:"
                    }
                };
                p2.Info.Add(new[] { "Project:", ProjectEn() });
                p2.Info.Add(new[] { "Request No:", "" });
                p2.Info.Add(new[] { "Client:", ClientEn() });
                p2.Info.Add(new[] { "Faze:", PhaseText() });
                p2.Info.Add(new[] { "Manufacturer:", ContractorEn() });
                p2.Info.Add(new[] { "Exam Date/Time:", P("StartDate") });

                int n = page * RowsPerPage;
                foreach (var r in rows.Skip(page * RowsPerPage).Take(RowsPerPage))
                {
                    n++;
                    var cells = new List<string> { n.ToString() };
                    if (manyItems) cells.Add(r.PartLabel);
                    cells.AddRange(new[]
                    {
                        r.WeldId, r.WelderNo ?? "", r.ScanSurface ?? "", Num(r.ThicknessMm),
                        Num(r.TestedLengthMm), r.ProbeAngle ?? "", Num(r.IndX), Num(r.IndY), Num(r.IndD), Num(r.IndL),
                        r.Interpretation ?? "", Mark(r.Accepted), Mark(!r.Accepted), r.Remarks ?? ""
                    });
                    p2.Rows.Add(cells.ToArray());
                }
                docs.Add(p2);
            }
            return docs;
        }
    }
}

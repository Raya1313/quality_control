using Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace WindowsFormsApp1.Forms
{
    // Section 3: dimensional control (DT)
    [System.ComponentModel.DesignerCategory("Code")]   
    public class DimensionalForm : InspectionBase
    {
        readonly BindingList<DtRow> _rows = new BindingList<DtRow>();
        protected override IList RowsList { get { return _rows; } }

        public DimensionalForm() : base("DT", "بخش سوم (کنترل ابعادی DT)", false)
        {
            AddDate("DateBefore", "تاریخ شروع بازرسی - قبل (Before)");
            AddDate("DateAfter", "تاریخ شروع بازرسی - بعد (After)", false);
            AddNum("GapBefore", "فاصله روزهای گزارش - قبل", 1, 1, 60);
            AddNum("GapAfter", "فاصله روزهای گزارش - بعد", 1, 1, 60);
            AddText("Vendor", "Vendor", 120, "Structure");
            AddText("Tpi", "TPI", 100, "-");

            Txt("ردیف", "", 45, true);
            Txt("کد قطعه", "PartLabel", 120, true);
            Txt("تعداد", "Qty", 55, false);
            Chk("Fit up", "FitUpOk", 55);
            Chk("Welding", "WeldOk", 60);
            Chk("بعد از جوش\nDim. check", "DimOk", 75);
            foreach (var c in "ABCDEFGHIJKL")
                Chk("نقطه " + c, "Cp" + c, 50);

            Start();
        }

        protected override bool ValidateExtra()
        {
            System.DateTime before, after;
            if (TryDate("DateBefore", out before) && TryDate("DateAfter", out after) && QcRules.EndBeforeStart(before, after))
            {
                Msg.Show("تاریخ شروع بازرسی «بعد» نمی‌تواند قبل از تاریخ «قبل» باشد.");
                return false;
            }
            return true;
        }

        protected override void AddRowsFor(PartPick p)
        {
            _rows.Add(new DtRow
            {
                PartId = p.PartId, PartName = p.PartName, InstanceNo = p.InstanceNo, InstanceTotal = p.InstanceTotal
            });
        }

        protected override void Persist(Dictionary<string, string> p, byte[]? sketch)
        {
            InspectionRepo.Save(Session.ProjectId, Kind, p, sketch, InspectionRepo.DtSql, _rows);
        }

        protected override void LoadRowsFor(int reportId)
        {
            foreach (var r in InspectionRepo.LoadRows<DtRow>("DtRow", reportId)) _rows.Add(r);
        }

        protected override List<ReportDoc> BuildDocs()
        {
            var d = new ReportDoc
            {
                Title = "Final Dimensional Report",
                Code = "Code: QC-FR-03/02",
                Revision = "Revision Date: 1403/10/01",
                InfoColumns = 2,
                Landscape = true,
                NumberBlankRows = true,
                MinRows = 15,
                Headers = new[] { "NO.", "Item No.", "QTY", "Fit up Result", "Welding Result", "Dim. Check after weld",
                                  "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" },
                Widths = new float[] { 3, 9, 4, 6, 6, 8, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                Signers = new[]
                {
                    "Name: " + QcEn() + "\nSignature:\nDate: " + P("DateBefore"),
                    "Name: " + CoordEn() + "\nSignature:\nDate: " + P("DateAfter")
                }
            };
            d.Info.Add(new[] { "Project Name:", ProjectEn() });
            d.Info.Add(new[] { "Date:", P("DateBefore") });
            d.Info.Add(new[] { "Client:", ClientEn() });
            d.Info.Add(new[] { "TPI:", P("Tpi") });
            d.Info.Add(new[] { "Contractor:", ContractorEn() });
            d.Info.Add(new[] { "Faze:", PhaseText() });
            d.Info.Add(new[] { "Vendor:", P("Vendor") });
            d.Info.Add(new[] { "", "" });

            int n = 1;
            foreach (var r in _rows)
            {
                var cp = new[] { r.CpA, r.CpB, r.CpC, r.CpD, r.CpE, r.CpF, r.CpG, r.CpH, r.CpI, r.CpJ, r.CpK, r.CpL };
                var cells = new List<string>
                {
                    n.ToString(), r.PartLabel, r.Qty.HasValue ? r.Qty.ToString() : "",
                    Mark(r.FitUpOk), Mark(r.WeldOk), Mark(r.DimOk)
                };
                cells.AddRange(cp.Select(Mark));
                d.Rows.Add(cells.ToArray());
                n++;
            }
            return new List<ReportDoc> { d };
        }
    }
}

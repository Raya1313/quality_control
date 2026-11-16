using System;

public class Project
{
    public int Id { get; set; }
    public string NameFa { get; set; } = ""; public string NameEn { get; set; } = "";
    public string ClientFa { get; set; } = ""; public string ClientEn { get; set; } = "";
    public string ContractorFa { get; set; } = ""; public string ContractorEn { get; set; } = "";
    public string QcManagerFa { get; set; } = ""; public string QcManagerEn { get; set; } = "";
    public string QaManagerFa { get; set; } = ""; public string QaManagerEn { get; set; } = "";
    public string CoordinatorFa { get; set; } = ""; public string CoordinatorEn { get; set; } = "";
    public DateTime ContractDate { get; set; }
    public int PhaseCount { get; set; }
    public string ReportedPhase { get; set; } = "";
    public byte[]? Logo { get; set; }
}

public class Part
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string PartName { get; set; } = "";
    public string? PartCode { get; set; }
    public string? DrawingCode { get; set; }
    public int Qty { get; set; }
    public decimal? Length { get; set; }
    public decimal? Weight { get; set; }
    public bool IsSaved { get; set; }        // not a column, used by the form
}

public class Connection
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public string ConnectionCode { get; set; } = "";
    public decimal? PlateThickness { get; set; }
    public int? Qty { get; set; }
    public decimal? Weight { get; set; }
    public bool IsSaved { get; set; }
}

// ---------------------------------------------------------------------------
// Sections 2 to 7
// ---------------------------------------------------------------------------

// one part instance (C1/1  3/11) inside a report
public abstract class InstanceRow
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public int PartId { get; set; }
    public int InstanceNo { get; set; }

    // display only, filled by the JOIN in the repositories
    public string PartName { get; set; } = "";
    public int InstanceTotal { get; set; }
    public string PartLabel => PartName + "  " + InstanceNo + "/" + InstanceTotal;
}

public class PartPick
{
    public int PartId { get; set; }
    public string PartName { get; set; } = "";
    public int InstanceNo { get; set; }
    public int InstanceTotal { get; set; }
    public string Label => PartName + "  " + InstanceNo + "/" + InstanceTotal;
}

public class WeldReport
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public DateTime StartDate { get; set; }
    public int GapBefore { get; set; }
    public int GapDuring { get; set; }
    public int DailyCapacity { get; set; }
    public decimal ConnectionPercent { get; set; }
    public string? HallSupervisor { get; set; }
    public string? Hall { get; set; }
    public string? SkeletonTime { get; set; }
    public string? BaseMetal { get; set; }
    public string? WeldConsumable { get; set; }
}

public class WeldRow : InstanceRow
{
    public int ConnectionId { get; set; }
    public DateTime ReportDate { get; set; }
    public DateTime DuringDate { get; set; }
    public decimal? BevelAngle { get; set; }
    public bool JointPrepOk { get; set; }
    public decimal? PlateThickness { get; set; }
    public bool WpsOk { get; set; }
    public bool WelderCtrlOk { get; set; }
    public bool FinalCtrlOk { get; set; }
    public string? WelderCode { get; set; }
    public decimal? AmbientTemp { get; set; }
    public decimal? TravelSpeed { get; set; }
    public decimal? Amperage { get; set; }
    public bool Ready1 { get; set; }
    public bool Ready2 { get; set; }
    public bool Ready3 { get; set; }
    public bool Ready4 { get; set; }
    public bool Ready5 { get; set; }
    public bool Ready6 { get; set; }
    public string? Notes { get; set; }

    public string ConnectionCode { get; set; } = "";
    public string ReportDateText => Calendar.ToText(ReportDate);
    public string DuringDateText => Calendar.ToText(DuringDate);
    public string CutType => PlateThickness > 15 ? "گرم" : "سرد";   // PL>15mm hot, PL<=15mm cold
}

public class DtRow : InstanceRow
{
    public int? Qty { get; set; }
    public bool FitUpOk { get; set; }
    public bool WeldOk { get; set; }
    public bool DimOk { get; set; }
    public bool CpA { get; set; }
    public bool CpB { get; set; }
    public bool CpC { get; set; }
    public bool CpD { get; set; }
    public bool CpE { get; set; }
    public bool CpF { get; set; }
    public bool CpG { get; set; }
    public bool CpH { get; set; }
    public bool CpI { get; set; }
    public bool CpJ { get; set; }
    public bool CpK { get; set; }
    public bool CpL { get; set; }
}

public class VtRow : InstanceRow
{
    public string ItemType { get; set; } = "";
    public bool Porosity { get; set; }
    public bool Undercut { get; set; }
    public bool Crack { get; set; }
    public bool EndCrater { get; set; }
    public bool Lof { get; set; }
    public bool Other { get; set; }
    public string Result { get; set; } = "Accepted";        // Accepted | Repair | AcceptedAfterRepair
    public string? DefectPosition { get; set; }
}

public class UtRow : InstanceRow
{
    public string WeldId { get; set; } = "";
    public string? WelderNo { get; set; }
    public string? ScanSurface { get; set; }
    public decimal? ThicknessMm { get; set; }
    public decimal? TestedLengthMm { get; set; }
    public string? ProbeAngle { get; set; }
    public decimal? IndX { get; set; }
    public decimal? IndY { get; set; }
    public decimal? IndD { get; set; }
    public decimal? IndL { get; set; }
    public string? Interpretation { get; set; }
    public bool Accepted { get; set; } = true;
    public string? Remarks { get; set; }
}

public class PtRow : InstanceRow
{
    public string? Position { get; set; }
    public string? LevelDir { get; set; }
    public string? DiscType { get; set; }
    public decimal? DiscSizeMm { get; set; }
    public string Result { get; set; } = "ACC";             // ACC | REP
    public bool AfterRep { get; set; }
}

public class Ncr
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int PartId { get; set; }
    public int InstanceNo { get; set; }
    public DateTime NcrDate { get; set; }
    public string ProductType { get; set; } = "";
    public string Stage { get; set; } = "";
    public string HallSupervisor { get; set; } = "";
    public string Inspector { get; set; } = "";
    public string Preparer { get; set; } = "";
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "";
    public string TaskDescription { get; set; } = "";
    public string SessionDuration { get; set; } = "";
    public DateTime? Deadline { get; set; }
    public string ExecResult { get; set; } = "";
    public string InspResult { get; set; } = "";
    public int QtyAccepted { get; set; }
    public int QtyRework { get; set; }
    public int QtyReturned { get; set; }
    public int QtyOtherUse { get; set; }
    public int QtyScrap { get; set; }
    public string ReviewDuration { get; set; } = "";
    public string FinalTask { get; set; } = "";

    // display only
    public string PartName { get; set; } = "";
    public int InstanceTotal { get; set; }
    public string PartLabel => PartName + "  " + InstanceNo + "/" + InstanceTotal;
    public string NcrDateText => Calendar.ToText(NcrDate);
}

public class NcrAction
{
    public int Id { get; set; }
    public int NcrId { get; set; }
    public string? Material { get; set; }
    public string? Tools { get; set; }
    public string? Executor { get; set; }
    public string? Execution { get; set; }
}

public class NcrCheck
{
    public int Id { get; set; }
    public int NcrId { get; set; }
    public string? Spec { get; set; }
    public string? Tolerance { get; set; }
    public string? Method { get; set; }
    public string? Sampling { get; set; }
    public string? Notes { get; set; }
}

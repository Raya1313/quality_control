/*
  verify.sql: checks the data the program saved against the rules in the requirements PDF.

  Use it after you have entered a project and filled the forms:
      sqlcmd -S localhost -E -d QC1 -i Database\verify.sql
  or open it in SSMS and press F5. Set @ProjectId below to check one project (NULL = all).

  Status:
    PASS  rule holds
    FAIL  the program let wrong data in (a bug, or a constraint is missing)
    WARN  allowed, but probably not what you want (e.g. a part with no connections)
  "Example" shows the first offending row so you can find it in the form.
*/
USE QC1;
GO

-- Gregorian -> Persian text (yyyy/MM/dd), same format Calendar.ToText writes into ReportParam
CREATE OR ALTER FUNCTION dbo.QcToPersian (@d DATE) RETURNS CHAR(10)
AS
BEGIN
    DECLARE @gy INT = YEAR(@d), @gm INT = MONTH(@d), @gd INT = DAY(@d);
    DECLARE @gy2 INT = CASE WHEN @gm > 2 THEN @gy + 1 ELSE @gy END;
    DECLARE @gdm INT = CHOOSE(@gm, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334);
    DECLARE @days INT = 355666 + 365 * @gy + (@gy2 + 3) / 4 - (@gy2 + 99) / 100 + (@gy2 + 399) / 400 + @gd + @gdm;
    DECLARE @jy INT = -1595 + 33 * (@days / 12053), @jm INT, @jd INT;
    SET @days = @days % 12053;
    SET @jy = @jy + 4 * (@days / 1461);
    SET @days = @days % 1461;
    IF @days > 365
    BEGIN
        SET @jy = @jy + (@days - 1) / 365;
        SET @days = (@days - 1) % 365;
    END
    IF @days < 186
        SELECT @jm = 1 + @days / 31, @jd = 1 + @days % 31;
    ELSE
        SELECT @jm = 7 + (@days - 186) / 30, @jd = 1 + (@days - 186) % 30;
    RETURN CONCAT(@jy, '/', RIGHT('0' + CAST(@jm AS VARCHAR(2)), 2), '/', RIGHT('0' + CAST(@jd AS VARCHAR(2)), 2));
END
GO

DECLARE @ProjectId INT = NULL;   -- NULL = every project

IF OBJECT_ID('tempdb..#r') IS NOT NULL DROP TABLE #r;
CREATE TABLE #r (Id INT IDENTITY, PdfRef VARCHAR(10), [Rule] NVARCHAR(200), Status VARCHAR(4), [Count] INT, Example NVARCHAR(300));

-- each check selects the offending rows; none = PASS, any = FAIL or WARN

/* ===== Section 1: project, parts, connections ===== */

INSERT #r SELECT 'p.1', N'Project has name, client, contractor, QC/QA managers, coordinator',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'project ', Id, N': ', NameFa) x FROM Project
      WHERE (@ProjectId IS NULL OR Id = @ProjectId)
        AND (NameFa = N'' OR ClientFa = N'' OR ContractorFa = N'' OR QcManagerFa = N''
             OR QaManagerFa = N'' OR CoordinatorFa = N'')) q;

INSERT #r SELECT 'p.1', N'Project logo defined (shown on forms and as watermark)',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'project ', Id, N': ', NameFa) x FROM Project
      WHERE (@ProjectId IS NULL OR Id = @ProjectId) AND Logo IS NULL) q;

INSERT #r SELECT 'p.1', N'Reported phase is not above the phase count',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'project ', Id, N': phase ', ReportedPhase, N' of ', PhaseCount) x FROM Project
      WHERE (@ProjectId IS NULL OR Id = @ProjectId)
        AND TRY_CAST(ReportedPhase AS INT) > PhaseCount) q;

INSERT #r SELECT 'p.1', N'No duplicate part names (note 2)',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(MIN(PartName), N' x', COUNT(*)) x FROM Part
      WHERE (@ProjectId IS NULL OR ProjectId = @ProjectId)
      GROUP BY ProjectId, LTRIM(RTRIM(PartName)) HAVING COUNT(*) > 1) q;

INSERT #r SELECT 'p.1', N'Part quantity is at least 1',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(PartName, N' qty ', Qty) x FROM Part
      WHERE (@ProjectId IS NULL OR ProjectId = @ProjectId) AND Qty < 1) q;

INSERT #r SELECT 'p.1', N'Part name uses Latin letters (C, B, R, RA ...)',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT PartName x FROM Part
      WHERE (@ProjectId IS NULL OR ProjectId = @ProjectId)
        AND PartName NOT LIKE '%[A-Za-z]%' COLLATE Latin1_General_BIN) q;

INSERT #r SELECT 'p.2', N'No duplicate connection codes (note 2)',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(ConnectionCode, N' x', COUNT(*)) x FROM Connection
      WHERE (@ProjectId IS NULL OR ProjectId = @ProjectId)
      GROUP BY ProjectId, ConnectionCode HAVING COUNT(*) > 1) q;

INSERT #r SELECT 'p.3', N'Connection has a plate thickness (needed for cut type)',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT ConnectionCode x FROM Connection
      WHERE (@ProjectId IS NULL OR ProjectId = @ProjectId) AND PlateThickness IS NULL) q;

INSERT #r SELECT 'p.2', N'Every part is linked to at least one connection',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT p.PartName x FROM Part p
      WHERE (@ProjectId IS NULL OR p.ProjectId = @ProjectId)
        AND NOT EXISTS (SELECT 1 FROM PartConnection pc WHERE pc.PartId = p.Id)) q;

INSERT #r SELECT 'p.2', N'Links stay inside one project',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' -> ', k.ConnectionCode) x
      FROM PartConnection pc JOIN Part p ON p.Id = pc.PartId JOIN Connection k ON k.Id = pc.ConnectionId
      WHERE (@ProjectId IS NULL OR p.ProjectId = @ProjectId) AND p.ProjectId <> k.ProjectId) q;

/* ===== Section 2: weld checklist ===== */

INSERT #r SELECT 'p.3-4', N'Checklist start date is not before the contract date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'start ', dbo.QcToPersian(w.StartDate), N' < contract ', dbo.QcToPersian(pr.ContractDate)) x
      FROM WeldReport w JOIN Project pr ON pr.Id = w.ProjectId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId) AND w.StartDate < pr.ContractDate) q;

INSERT #r SELECT 'p.3-4', N'No checklist row dated before the start date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty, N' on ', dbo.QcToPersian(r.ReportDate)) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId) AND r.ReportDate < w.StartDate) q;

INSERT #r SELECT 'p.3', N'"During welding" date is not before the "before welding" date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId) AND r.DuringDate < r.ReportDate) q;

INSERT #r SELECT 'p.4', N'No more parts per report date than daily capacity x interval',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(dbo.QcToPersian(r.ReportDate), N': ', COUNT(DISTINCT CONCAT(r.PartId, ':', r.InstanceNo)),
                    N' parts > ', MAX(w.DailyCapacity * w.GapBefore)) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
      GROUP BY r.ReportId, r.ReportDate
      HAVING COUNT(DISTINCT CONCAT(r.PartId, ':', r.InstanceNo)) > MAX(w.DailyCapacity * w.GapBefore)) q;

INSERT #r SELECT 'p.1', N'Checklist instance number is between 1 and the part quantity',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId) AND (r.InstanceNo < 1 OR r.InstanceNo > p.Qty)) q;

INSERT #r SELECT 'p.2', N'Checklist only uses connections linked to that part',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' -> ', k.ConnectionCode) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId
      JOIN Part p ON p.Id = r.PartId JOIN Connection k ON k.Id = r.ConnectionId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
        AND NOT EXISTS (SELECT 1 FROM PartConnection pc WHERE pc.PartId = r.PartId AND pc.ConnectionId = r.ConnectionId)) q;

-- expected rows per instance = CEILING(linked connections x percent / 100), at least 1
INSERT #r SELECT 'p.4', N'Each part instance has the chosen percent of its connections',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty, N': ', COUNT(*), N' rows, expected ',
                    MAX(e.Expected)) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      CROSS APPLY (SELECT IIF(CEILING(COUNT(*) * w.ConnectionPercent / 100.0) < 1, 1,
                              CEILING(COUNT(*) * w.ConnectionPercent / 100.0)) AS Expected
                   FROM PartConnection pc WHERE pc.PartId = r.PartId) e
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
      GROUP BY r.ReportId, p.PartName, p.Qty, r.PartId, r.InstanceNo
      HAVING COUNT(*) <> MAX(e.Expected)) q;

INSERT #r SELECT 'p.4', N'Every instance of every linked part is in the checklist',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', n.k, N'/', p.Qty) x
      FROM WeldReport w
      JOIN Part p ON p.ProjectId = w.ProjectId
      CROSS APPLY (SELECT TOP (p.Qty) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) k FROM sys.all_objects) n
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
        AND EXISTS (SELECT 1 FROM PartConnection pc WHERE pc.PartId = p.Id)
        AND NOT EXISTS (SELECT 1 FROM WeldRow r WHERE r.ReportId = w.Id AND r.PartId = p.Id AND r.InstanceNo = n.k)) q;

INSERT #r SELECT 'p.5', N'Row thickness matches the connection thickness (cut type PL<=15 cold, >15 hot)',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(k.ConnectionCode, N': row ', r.PlateThickness, N' vs ', k.PlateThickness) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId JOIN Connection k ON k.Id = r.ConnectionId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
        AND ISNULL(r.PlateThickness, -1) <> ISNULL(k.PlateThickness, -1)) q;

INSERT #r SELECT 'p.5', N'Checklist rows fully ticked before saving (prep, WPS, welder, final, 1-6)',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty, N' ', k.ConnectionCode) x
      FROM WeldRow r JOIN WeldReport w ON w.Id = r.ReportId
      JOIN Part p ON p.Id = r.PartId JOIN Connection k ON k.Id = r.ConnectionId
      WHERE (@ProjectId IS NULL OR w.ProjectId = @ProjectId)
        AND (r.JointPrepOk & r.WpsOk & r.WelderCtrlOk & r.FinalCtrlOk
             & r.Ready1 & r.Ready2 & r.Ready3 & r.Ready4 & r.Ready5 & r.Ready6) = 0) q;

/* ===== Sections 3-6: DT, VT, UT, PT ===== */

INSERT #r SELECT 'p.6-12', N'No DT/VT/UT/PT start date before the contract date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(h.Kind, N' ', rp.Name, N' ', rp.Value, N' < ', dbo.QcToPersian(pr.ContractDate)) x
      FROM ReportParam rp JOIN InspectionReport h ON h.Id = rp.ReportId JOIN Project pr ON pr.Id = h.ProjectId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId)
        AND rp.Name IN ('StartDate', 'DateBefore', 'DateAfter') AND rp.Value <> ''
        AND rp.Value < dbo.QcToPersian(pr.ContractDate)) q;

INSERT #r SELECT 'p.6', N'DT "after" date is not before the "before" date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'before ', b.Value, N', after ', a.Value) x
      FROM InspectionReport h
      JOIN ReportParam b ON b.ReportId = h.Id AND b.Name = 'DateBefore'
      JOIN ReportParam a ON a.ReportId = h.Id AND a.Name = 'DateAfter'
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND h.Kind = 'DT'
        AND a.Value <> '' AND a.Value < b.Value) q;

INSERT #r SELECT 'p.6-12', N'Every report has a start date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT h.Kind x FROM InspectionReport h
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId)
        AND NOT EXISTS (SELECT 1 FROM ReportParam rp WHERE rp.ReportId = h.Id
                        AND rp.Name IN ('StartDate', 'DateBefore') AND rp.Value <> '')) q;

INSERT #r SELECT 'p.1', N'Inspection rows point at a real instance (1..qty)',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(t.Kind, N' ', p.PartName, N' ', t.InstanceNo, N'/', p.Qty) x
      FROM (SELECT 'DT' Kind, ReportId, PartId, InstanceNo FROM DtRow
            UNION ALL SELECT 'VT', ReportId, PartId, InstanceNo FROM VtRow
            UNION ALL SELECT 'UT', ReportId, PartId, InstanceNo FROM UtRow
            UNION ALL SELECT 'PT', ReportId, PartId, InstanceNo FROM PtRow) t
      JOIN InspectionReport h ON h.Id = t.ReportId JOIN Part p ON p.Id = t.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId)
        AND (t.InstanceNo < 1 OR t.InstanceNo > p.Qty OR p.ProjectId <> h.ProjectId)) q;

INSERT #r SELECT 'p.7', N'VT: a ticked defect means the result is not plain "Accepted"',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM VtRow r JOIN InspectionReport h ON h.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND r.Result = 'Accepted'
        AND (r.Porosity | r.Undercut | r.Crack | r.EndCrater | r.Lof | r.Other) = 1) q;

INSERT #r SELECT 'p.7', N'VT: "Repair" rows say which defect and where',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM VtRow r JOIN InspectionReport h ON h.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND r.Result <> 'Accepted'
        AND ((r.Porosity | r.Undercut | r.Crack | r.EndCrater | r.Lof | r.Other) = 0
             OR ISNULL(r.DefectPosition, N'') = N'')) q;

INSERT #r SELECT 'p.12', N'PT: "Rep." rows have a discontinuity type',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM PtRow r JOIN InspectionReport h ON h.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND r.Result = 'REP' AND ISNULL(r.DiscType, N'') = N'') q;

INSERT #r SELECT 'p.12', N'PT: "After Rep." only ticked on repaired rows',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty) x
      FROM PtRow r JOIN InspectionReport h ON h.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND r.Result = 'ACC' AND r.AfterRep = 1) q;

INSERT #r SELECT 'p.10', N'UT: rejected welds have indication position or interpretation',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(p.PartName, N' ', r.InstanceNo, N'/', p.Qty, N' ', r.WeldId) x
      FROM UtRow r JOIN InspectionReport h ON h.Id = r.ReportId JOIN Part p ON p.Id = r.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND r.Accepted = 0
        AND r.IndX IS NULL AND ISNULL(r.Interpretation, N'') = N'') q;

INSERT #r SELECT 'p.8, 11', N'UT/PT sketch attached',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT h.Kind x FROM InspectionReport h
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId) AND h.Kind IN ('UT', 'PT') AND h.Sketch IS NULL) q;

/* ===== Section 7: NCR ===== */

-- p.13: parts with defects in the forms above need corrective actions recorded in an NCR
INSERT #r SELECT 'p.13', N'Every defective part (VT repair, PT rep., UT rejected) has an NCR',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT DISTINCT CONCAT(t.Kind, N' ', p.PartName, N' ', t.InstanceNo, N'/', p.Qty) x
      FROM (SELECT 'VT' Kind, ReportId, PartId, InstanceNo FROM VtRow WHERE Result <> 'Accepted'
            UNION ALL SELECT 'PT', ReportId, PartId, InstanceNo FROM PtRow WHERE Result = 'REP'
            UNION ALL SELECT 'UT', ReportId, PartId, InstanceNo FROM UtRow WHERE Accepted = 0) t
      JOIN InspectionReport h ON h.Id = t.ReportId JOIN Part p ON p.Id = t.PartId
      WHERE (@ProjectId IS NULL OR h.ProjectId = @ProjectId)
        AND NOT EXISTS (SELECT 1 FROM Ncr n WHERE n.PartId = t.PartId AND n.InstanceNo = t.InstanceNo)) q;

INSERT #r SELECT 'p.13', N'NCR date is not before the contract date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'NCR ', n.Id, N' ', dbo.QcToPersian(n.NcrDate)) x
      FROM Ncr n JOIN Project pr ON pr.Id = n.ProjectId
      WHERE (@ProjectId IS NULL OR n.ProjectId = @ProjectId) AND n.NcrDate < pr.ContractDate) q;

INSERT #r SELECT 'p.13', N'NCR deadline is not before the NCR date',
    IIF(COUNT(*) = 0, 'PASS', 'FAIL'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'NCR ', n.Id) x FROM Ncr n
      WHERE (@ProjectId IS NULL OR n.ProjectId = @ProjectId) AND n.Deadline < n.NcrDate) q;

INSERT #r SELECT 'p.13', N'NCR has description, severity, disposition and signers',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'NCR ', n.Id) x FROM Ncr n
      WHERE (@ProjectId IS NULL OR n.ProjectId = @ProjectId)
        AND (n.Description = N'' OR n.Severity = N'' OR n.TaskDescription = N''
             OR n.Preparer = N'' OR n.Inspector = N'' OR n.FollowUpBy = N'')) q;

INSERT #r SELECT 'p.13', N'NCR quantity table is filled (accepted + rework + returned + other + scrap > 0)',
    IIF(COUNT(*) = 0, 'PASS', 'WARN'), COUNT(*), MIN(x)
FROM (SELECT CONCAT(N'NCR ', n.Id) x FROM Ncr n
      WHERE (@ProjectId IS NULL OR n.ProjectId = @ProjectId)
        AND n.QtyAccepted + n.QtyRework + n.QtyReturned + n.QtyOtherUse + n.QtyScrap = 0) q;

/* ===== result ===== */

SELECT PdfRef, Status, [Count], [Rule], Example FROM #r ORDER BY Id;

SELECT SUM(IIF(Status = 'PASS', 1, 0)) AS Passed,
       SUM(IIF(Status = 'WARN', 1, 0)) AS Warnings,
       SUM(IIF(Status = 'FAIL', 1, 0)) AS Failed
FROM #r;
GO

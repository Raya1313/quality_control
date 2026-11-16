using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    // DT, VT, UT and PT reports share one header table (InspectionReport + ReportParam)
    // and each has its own row table.
    public static class InspectionRepo
    {
        public class Header
        {
            public int Id { get; set; }
            public byte[]? Sketch { get; set; }
            public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        }

        class ParamRow
        {
            public string Name { get; set; } = "";
            public string Value { get; set; } = "";
        }

        public const string DtSql = @"
            INSERT INTO DtRow (ReportId,PartId,InstanceNo,Qty,FitUpOk,WeldOk,DimOk,
                CpA,CpB,CpC,CpD,CpE,CpF,CpG,CpH,CpI,CpJ,CpK,CpL)
            VALUES (@ReportId,@PartId,@InstanceNo,@Qty,@FitUpOk,@WeldOk,@DimOk,
                @CpA,@CpB,@CpC,@CpD,@CpE,@CpF,@CpG,@CpH,@CpI,@CpJ,@CpK,@CpL)";

        public const string VtSql = @"
            INSERT INTO VtRow (ReportId,PartId,InstanceNo,ItemType,Porosity,Undercut,Crack,EndCrater,Lof,Other,
                Result,DefectPosition)
            VALUES (@ReportId,@PartId,@InstanceNo,@ItemType,@Porosity,@Undercut,@Crack,@EndCrater,@Lof,@Other,
                @Result,@DefectPosition)";

        public const string UtSql = @"
            INSERT INTO UtRow (ReportId,PartId,InstanceNo,WeldId,WelderNo,ScanSurface,ThicknessMm,TestedLengthMm,
                ProbeAngle,IndX,IndY,IndD,IndL,Interpretation,Accepted,Remarks)
            VALUES (@ReportId,@PartId,@InstanceNo,@WeldId,@WelderNo,@ScanSurface,@ThicknessMm,@TestedLengthMm,
                @ProbeAngle,@IndX,@IndY,@IndD,@IndL,@Interpretation,@Accepted,@Remarks)";

        public const string PtSql = @"
            INSERT INTO PtRow (ReportId,PartId,InstanceNo,Position,LevelDir,DiscType,DiscSizeMm,Result,AfterRep)
            VALUES (@ReportId,@PartId,@InstanceNo,@Position,@LevelDir,@DiscType,@DiscSizeMm,@Result,@AfterRep)";

        public static Header? LoadHeader(int projectId, string kind)
        {
            using var c = Db.Open();
            var h = c.QueryFirstOrDefault<Header>(
                "SELECT Id, Sketch FROM InspectionReport WHERE ProjectId=@projectId AND Kind=@kind",
                new { projectId, kind });
            if (h == null) return null;

            foreach (var p in c.Query<ParamRow>("SELECT Name, Value FROM ReportParam WHERE ReportId=@id", new { id = h.Id }))
                h.Params[p.Name] = p.Value;
            return h;
        }

        public static List<T> LoadRows<T>(string table, int reportId) where T : InstanceRow
        {
            using var c = Db.Open();
            return c.Query<T>(@"
                SELECT r.*, p.PartName, p.Qty AS InstanceTotal
                FROM " + table + @" r
                JOIN Part p ON p.Id = r.PartId
                WHERE r.ReportId=@reportId
                ORDER BY r.Id", new { reportId }).ToList();
        }

        // one report per project and kind: the old one is replaced (rows and params go with it)
        public static void Save<T>(int projectId, string kind, Dictionary<string, string> p, byte[]? sketch,
                                   string insertSql, IEnumerable<T> rows) where T : InstanceRow
        {
            using var c = Db.Open();
            using var tx = c.BeginTransaction();

            c.Execute("DELETE FROM InspectionReport WHERE ProjectId=@projectId AND Kind=@kind",
                      new { projectId, kind }, tx);

            int id = c.ExecuteScalar<int>(@"
                INSERT INTO InspectionReport (ProjectId,Kind,Sketch) OUTPUT INSERTED.Id
                VALUES (@projectId,@kind,@sketch)", new { projectId, kind, sketch }, tx);

            var prms = p.Select(kv => new { ReportId = id, Name = kv.Key, Value = kv.Value }).ToList();
            c.Execute("INSERT INTO ReportParam (ReportId,Name,Value) VALUES (@ReportId,@Name,@Value)", prms, tx);

            var list = rows.ToList();
            foreach (var r in list) r.ReportId = id;
            c.Execute(insertSql, list, tx);

            tx.Commit();
        }
    }

    public static class NcrRepo
    {
        public static List<Ncr> List(int projectId)
        {
            using var c = Db.Open();
            return c.Query<Ncr>(@"
                SELECT n.*, p.PartName, p.Qty AS InstanceTotal
                FROM Ncr n JOIN Part p ON p.Id = n.PartId
                WHERE n.ProjectId=@projectId ORDER BY n.Id", new { projectId }).ToList();
        }

        public static List<NcrAction> Actions(int ncrId)
        {
            using var c = Db.Open();
            return c.Query<NcrAction>("SELECT * FROM NcrAction WHERE NcrId=@ncrId ORDER BY Id", new { ncrId }).ToList();
        }

        public static List<NcrCheck> Checks(int ncrId)
        {
            using var c = Db.Open();
            return c.Query<NcrCheck>("SELECT * FROM NcrCheck WHERE NcrId=@ncrId ORDER BY Id", new { ncrId }).ToList();
        }

        public static void Save(Ncr n, IEnumerable<NcrAction> actions, IEnumerable<NcrCheck> checks)
        {
            using var c = Db.Open();
            using var tx = c.BeginTransaction();

            if (n.Id == 0)
            {
                n.Id = c.ExecuteScalar<int>(@"
                    INSERT INTO Ncr (ProjectId,PartId,InstanceNo,NcrDate,ProductType,Stage,HallSupervisor,Inspector,
                        Preparer,Description,Severity,TaskDescription,SessionDuration,Deadline,ExecResult,InspResult,
                        QtyAccepted,QtyRework,QtyReturned,QtyOtherUse,QtyScrap,ReviewDuration,FinalTask)
                    OUTPUT INSERTED.Id
                    VALUES (@ProjectId,@PartId,@InstanceNo,@NcrDate,@ProductType,@Stage,@HallSupervisor,@Inspector,
                        @Preparer,@Description,@Severity,@TaskDescription,@SessionDuration,@Deadline,@ExecResult,@InspResult,
                        @QtyAccepted,@QtyRework,@QtyReturned,@QtyOtherUse,@QtyScrap,@ReviewDuration,@FinalTask)", n, tx);
            }
            else
            {
                c.Execute(@"
                    UPDATE Ncr SET PartId=@PartId,InstanceNo=@InstanceNo,NcrDate=@NcrDate,ProductType=@ProductType,
                        Stage=@Stage,HallSupervisor=@HallSupervisor,Inspector=@Inspector,Preparer=@Preparer,
                        Description=@Description,Severity=@Severity,TaskDescription=@TaskDescription,
                        SessionDuration=@SessionDuration,Deadline=@Deadline,ExecResult=@ExecResult,
                        InspResult=@InspResult,QtyAccepted=@QtyAccepted,QtyRework=@QtyRework,QtyReturned=@QtyReturned,
                        QtyOtherUse=@QtyOtherUse,QtyScrap=@QtyScrap,ReviewDuration=@ReviewDuration,FinalTask=@FinalTask
                    WHERE Id=@Id", n, tx);
                c.Execute("DELETE FROM NcrAction WHERE NcrId=@Id", new { n.Id }, tx);
                c.Execute("DELETE FROM NcrCheck WHERE NcrId=@Id", new { n.Id }, tx);
            }

            var a = actions.ToList();
            foreach (var x in a) x.NcrId = n.Id;
            c.Execute("INSERT INTO NcrAction (NcrId,Material,Tools,Executor,Execution) VALUES (@NcrId,@Material,@Tools,@Executor,@Execution)", a, tx);

            var k = checks.ToList();
            foreach (var x in k) x.NcrId = n.Id;
            c.Execute("INSERT INTO NcrCheck (NcrId,Spec,Tolerance,Method,Sampling,Notes) VALUES (@NcrId,@Spec,@Tolerance,@Method,@Sampling,@Notes)", k, tx);

            tx.Commit();
        }

        public static void Delete(int id)
        {
            using var c = Db.Open();
            c.Execute("DELETE FROM Ncr WHERE Id=@id", new { id });
        }
    }
}

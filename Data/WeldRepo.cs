using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace Data
{
    public static class WeldRepo
    {
        public static WeldReport? LoadReport(int projectId)
        {
            using var c = Db.Open();
            return c.QueryFirstOrDefault<WeldReport>(
                "SELECT TOP 1 * FROM WeldReport WHERE ProjectId=@projectId ORDER BY Id DESC",
                new { projectId });
        }

        public static List<WeldRow> LoadRows(int reportId)
        {
            using var c = Db.Open();
            return c.Query<WeldRow>(@"
                SELECT w.*, p.PartName, p.Qty AS InstanceTotal, k.ConnectionCode
                FROM WeldRow w
                JOIN Part p ON p.Id = w.PartId
                JOIN Connection k ON k.Id = w.ConnectionId
                WHERE w.ReportId=@reportId
                ORDER BY w.Id", new { reportId }).ToList();
        }

        // one report per project: the old one is replaced (its rows go with it)
        public static void Save(WeldReport r, IEnumerable<WeldRow> rows)
        {
            using var c = Db.Open();
            using var tx = c.BeginTransaction();

            c.Execute("DELETE FROM WeldReport WHERE ProjectId=@ProjectId", new { r.ProjectId }, tx);

            r.Id = c.ExecuteScalar<int>(@"
                INSERT INTO WeldReport (ProjectId,StartDate,GapBefore,GapDuring,DailyCapacity,
                    ConnectionPercent,HallSupervisor,Hall,SkeletonTime,BaseMetal,WeldConsumable)
                OUTPUT INSERTED.Id
                VALUES (@ProjectId,@StartDate,@GapBefore,@GapDuring,@DailyCapacity,
                    @ConnectionPercent,@HallSupervisor,@Hall,@SkeletonTime,@BaseMetal,@WeldConsumable)", r, tx);

            var list = rows.ToList();
            foreach (var x in list) x.ReportId = r.Id;

            c.Execute(@"
                INSERT INTO WeldRow (ReportId,PartId,InstanceNo,ConnectionId,ReportDate,DuringDate,
                    BevelAngle,JointPrepOk,PlateThickness,WpsOk,WelderCtrlOk,FinalCtrlOk,
                    WelderCode,AmbientTemp,TravelSpeed,Amperage,
                    Ready1,Ready2,Ready3,Ready4,Ready5,Ready6,Notes)
                VALUES (@ReportId,@PartId,@InstanceNo,@ConnectionId,@ReportDate,@DuringDate,
                    @BevelAngle,@JointPrepOk,@PlateThickness,@WpsOk,@WelderCtrlOk,@FinalCtrlOk,
                    @WelderCode,@AmbientTemp,@TravelSpeed,@Amperage,
                    @Ready1,@Ready2,@Ready3,@Ready4,@Ready5,@Ready6,@Notes)", list, tx);

            tx.Commit();
        }
    }
}

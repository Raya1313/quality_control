using Dapper;

namespace Data
{
    public static class ProjectRepo
    {

        public static Project? LoadLast()
        {
            using var c = Db.Open();
            return c.QueryFirstOrDefault<Project>("SELECT TOP 1 * FROM Project ORDER BY Id DESC");
        }
        public static int Insert(Project p)
        {
            using var c = Db.Open();
            return c.ExecuteScalar<int>(@"
            INSERT INTO Project (NameFa,NameEn,ClientFa,ClientEn,ContractorFa,ContractorEn,
                QcManagerFa,QcManagerEn,QaManagerFa,QaManagerEn,CoordinatorFa,CoordinatorEn,
                ContractDate,PhaseCount,ReportedPhase,Logo)
            OUTPUT INSERTED.Id
            VALUES (@NameFa,@NameEn,@ClientFa,@ClientEn,@ContractorFa,@ContractorEn,
                @QcManagerFa,@QcManagerEn,@QaManagerFa,@QaManagerEn,@CoordinatorFa,@CoordinatorEn,
                @ContractDate,@PhaseCount,@ReportedPhase,@Logo)", p);
        }
    }
}
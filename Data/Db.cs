using Microsoft.Data.SqlClient;

namespace Data
{
    public static class Db
    {
        const string Cs = @"Server=localhost;Database=QC1;Trusted_Connection=True;TrustServerCertificate=True";

        public static SqlConnection Open()
        {
            var c = new SqlConnection(Cs);
            c.Open();
            return c;
        }
    }
}
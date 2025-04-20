namespace KitItSimple.DbClient.Console
{
    using GenericDbClient;

    using System;
    using System.Data.SqlClient;

    internal class Program
    {
        private const string CONNECTION_STRING
            = "Data Source=.;"
            + "Integrated Security=True;"
            + "Connect Timeout=30;"
            + "Encrypt=False;"
            + "TrustServerCertificate=True;"
            + "ApplicationIntent=ReadWrite;"
            + "MultiSubnetFailover=False;";

        private static void Main()
        {
            DBClient.AddLogger(Console.WriteLine);

            var connection = DBConnection<SqlConnection>.ConfigureConnection(sqlConnection
                => sqlConnection.ConnectionString = new SqlConnectionStringBuilder
                {
                    ConnectTimeout = 30_000,
                    DataSource = ".",
                    IntegratedSecurity = true,
                    Encrypt = false,
                    TrustServerCertificate = true,
                }
                .ConnectionString);
            var now = DateTime.Now;

            var result = connection
                .CreateCommand($@"SELECT @{nameof(now)}")
                .SetParameter(nameof(now), DateTime.Now)
                .ExecuteScalar<DateTime>();

            result = connection
                .BeginTransaction(transaction =>
                {
                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(now)}")
                        .SetParameter(nameof(now), DateTime.Now)
                        .ExecuteScalar<DateTime>();

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(now)}")
                        .SetParameter(nameof(now), DateTime.Now)
                        .ExecuteScalar<DateTime>();

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(now)}")
                        .SetParameter(nameof(now), DateTime.Now)
                        .ExecuteScalar<DateTime>();

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(now)}")
                        .SetParameter(nameof(now), null)
                        .ExecuteScalar<DateTime>();

                    transaction.CommitTransaction();

                    return DateTime.Now;
                });
        }
    }
}

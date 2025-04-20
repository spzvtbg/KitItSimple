namespace KitItSimple.DbClient
{
    using System;
    using System.Data.Common;

    public class DBConnection<T> where T : DbConnection, new()
    {
        private readonly string connectionString;
        private readonly T dbConnection;

        private DBConnection(T dbConnection)
        {
            this.dbConnection = dbConnection;
            this.connectionString = this.dbConnection.ConnectionString;
        }

        public void BeginTransaction(Action<DBTransaction> dbTransactionBody)
        {
            using (this.dbConnection)
            {
                this.dbConnection.ConnectionString = this.connectionString;

                this.dbConnection.Open();

                using (var dbTransaction = this.dbConnection.BeginTransaction())
                {
                    dbTransactionBody(dbTransaction);
                }
            }
        }

        public TResult BeginTransaction<TResult>(Func<DBTransaction, TResult> dbTransactionBody)
        {
            var result = default(TResult);

            using (this.dbConnection)
            {
                this.dbConnection.ConnectionString = this.connectionString;

                this.dbConnection.Open();

                using (var dbTransaction = this.dbConnection.BeginTransaction())
                {
                    result = dbTransactionBody(dbTransaction);
                }
            }

            return result;
        }

        public DBCommand CreateCommand(string commandText)
        {
            this.dbConnection.ConnectionString = this.connectionString;
            var dbCommand = this.dbConnection.CreateCommand();

            return new DBConnectionCommand(dbCommand, commandText);
        }

        public static DBConnection<T> ConfigureConnection(Action<T> dbConnectionBuilder)
        {
            var dbConnection = new T();

            dbConnectionBuilder?.Invoke(dbConnection);

            return new DBConnection<T>(dbConnection);
        }
    }
}

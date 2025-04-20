namespace KitItSimple.DbClient
{
    using System;
    using System.Data;

    public class DBConnection<T> where T : IDbConnection, new()
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
            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.ConnectionString = this.connectionString;

                    this.dbConnection.Open();

                    using (var dbTransaction = this.dbConnection.BeginTransaction())
                    {
                        dbTransactionBody(new DBTransaction(dbTransaction));
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }
        }

        public TResult BeginTransaction<TResult>(Func<DBTransaction, TResult> dbTransactionBody)
        {
            var result = default(TResult);

            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.ConnectionString = this.connectionString;

                    this.dbConnection.Open();

                    using (var dbTransaction = this.dbConnection.BeginTransaction())
                    {
                        result = dbTransactionBody(new DBTransaction(dbTransaction));
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }

            return result;
        }

        public DBCommand CreateCommand(string commandText)
        {
            this.dbConnection.ConnectionString = this.connectionString;

            return new DBConnectionCommand(this.dbConnection, commandText);
        }

        public static DBConnection<T> ConfigureConnection(Action<T> dbConnectionBuilder)
        {
            var dbConnection = new T();

            if (dbConnectionBuilder != null)
            {
                try
                {
                    dbConnectionBuilder(dbConnection);
                }
                catch (Exception e)
                {
                    e.Log();
                }
            }

            return new DBConnection<T>(dbConnection);
        }
    }
}

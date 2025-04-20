namespace GenericDbClient
{
    using System;
    using System.Data;

    /// <summary>
    /// Generic, lightweight and fluent ADO.NET wrapper 
    /// designed to simplify database access in .NET projects 
    /// using any provider (SQL Server, PostgreSQL, SQLite, etc)
    /// </summary>
    /// <typeparam name="T">The provider specific IDbConnection implementation</typeparam>
    public class DBConnection<T> where T : IDbConnection, new()
    {
        private readonly string connectionString;
        private readonly T dbConnection;

        private DBConnection(T dbConnection)
        {
            this.dbConnection = dbConnection;
            this.connectionString = this.dbConnection.ConnectionString;
        }

        /// <summary>
        /// <inheritdoc cref="IDbConnection.BeginTransaction()"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="IDbConnection.BeginTransaction()"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="IDbConnection.CreateCommand()"/>
        /// </summary>
        public DBCommand CreateCommand(string commandText)
        {
            this.dbConnection.ConnectionString = this.connectionString;

            return new DBConnectionCommand(this.dbConnection, commandText);
        }

        /// <summary>
        /// Allows the callers to configure provider specific IDbConnection implementation.
        /// </summary>
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

namespace GenericDbClient
{
    using System.Data;

    public class DBTransaction
    {
        private readonly IDbTransaction dbTransaction;

        internal DBTransaction(IDbTransaction dbTransaction) => this.dbTransaction = dbTransaction;

        public void CommitTransaction() => this.dbTransaction.Commit();

        /// <summary>
        /// <inheritdoc cref="IDbConnection.CreateCommand()"/>
        /// </summary>
        public DBCommand CreateCommand(string commandText)
        {
            var dbCommand = this.dbTransaction.Connection.CreateCommand();
            dbCommand.Transaction = this.dbTransaction;

            return new DBTransactionCommand(dbCommand, commandText);
        }

        public void RollbackTransaction() => this.dbTransaction.Rollback();
    }
}

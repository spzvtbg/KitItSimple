namespace KitItSimple.DbClient
{
    using System.Data.Common;

    public class DBTransaction
    {
        private readonly DbTransaction dbTransaction;

        internal DBTransaction(DbTransaction dbTransaction) => this.dbTransaction = dbTransaction;

        public void CommitTransaction() => this.dbTransaction.Commit();

        public DBCommand CreateCommand(string commandText)
        {
            var dbCommand = this.dbTransaction.Connection.CreateCommand();
            dbCommand.Transaction = this.dbTransaction;

            return new DBTransactionCommand(dbCommand, commandText);
        }

#if NET5_0_OR_GREATER
        public void ReleaseSavePoint(string name) => this.dbTransaction.Release(name);
#endif

        public void RollbackTransaction() => this.dbTransaction.Rollback();

#if NET5_0_OR_GREATER
        public void CreateSavePoint(string name) => this.dbTransaction.Save(name);
#endif

        public static implicit operator DBTransaction(DbTransaction dbTransaction)
            => new DBTransaction(dbTransaction);
    }
}

namespace GenericDbClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class DBTransactionCommand : DBCommand
    {
        internal DBTransactionCommand(IDbCommand dbCommand, string commandText)
        {
            this.dbCommand = dbCommand;
            this.dbCommand.CommandText = commandText;

            this.dbCommand.Parameters.Clear();
        }

        public override int ExecuteNonQuery()
        {
            var rowsAffected = default(int);

            using (this.dbCommand)
            {
                rowsAffected = this.dbCommand.ExecuteNonQuery();

                this.dbCommand.Log();
            }

            return rowsAffected;
        }

        public override void ExecuteReader(Action<DBReader> readHandler)
        {
            using (this.dbCommand)
            {
                using (var dataReader = this.dbCommand.ExecuteReader())
                {
                    var dbReader = new DBReader(dataReader);

                    while (dbReader.Read())
                    {
                        readHandler(dbReader);
                    }

                    this.dbCommand.Log();
                }
            }
        }

        public override List<T> ExecuteReader<T>(Func<DBReader, T> readHandler)
        {
            var results = new List<T>();

            using (this.dbCommand)
            {
                using (var dataReader = this.dbCommand.ExecuteReader())
                {
                    var dbReader = new DBReader(dataReader);

                    while (dbReader.Read())
                    {
                        var value = readHandler(dbReader);

                        results.Add(value);
                    }

                    this.dbCommand.Log();
                }
            }

            return results;
        }

        public override T ExecuteScalar<T>()
        {
            var result = default(T);

            using (this.dbCommand)
            {
                result = this.dbCommand
                    .ExecuteScalar()
                    .TryParseValueOrDefault<T>();

                this.dbCommand.Log();
            }

            return result;
        }
    }
}

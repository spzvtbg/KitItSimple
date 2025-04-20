namespace KitItSimple.DbClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    public class DBConnectionCommand : DBCommand
    {
        internal DBConnectionCommand(DbCommand dbCommand, string commandText) : base(dbCommand, commandText)
        {
        }

        public override int ExecuteNonQuery()
        {
            var rowsAffected = default(int);

            using (var dbConnection = this.dbCommand.Connection)
            {
                dbConnection.Open();

                using (this.dbCommand)
                {
                    rowsAffected = this.dbCommand.ExecuteNonQuery();
                }
            }

            return rowsAffected;
        }

        public override void ExecuteReader(Action<DBReader> readHandler)
        {
            using (var dbConnection = this.dbCommand.Connection)
            {
                dbConnection.Open();

                using (this.dbCommand)
                {
                    using (var dbDataReader = this.dbCommand.ExecuteReader())
                    {
                        DBReader dBReader = dbDataReader;

                        while (dBReader.Read())
                        {
                            readHandler(dBReader);
                        }
                    }
                }
            }
        }

        public override List<T> ExecuteReader<T>(Func<DBReader, T> readHandler)
        {
            var results = new List<T>();

            using (var dbConnection = this.dbCommand.Connection)
            {
                dbConnection.Open();

                using (this.dbCommand)
                {
                    using (var dbDataReader = this.dbCommand.ExecuteReader())
                    {
                        DBReader dBReader = dbDataReader;

                        while (dBReader.Read())
                        {
                            var value = readHandler(dBReader);

                            results.Add(value);
                        }
                    }
                }
            }

            return results;
        }

        public override T ExecuteScalar<T>()
        {
            var result = default(T);

            using (var dbConnection = this.dbCommand.Connection)
            {
                dbConnection.Open();

                using (this.dbCommand)
                {
                    result = this.dbCommand
                        .ExecuteScalar()
                        .TryParseValueOrDefault<T>();
                }
            }

            return result;
        }
    }
}

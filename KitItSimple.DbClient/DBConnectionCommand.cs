namespace KitItSimple.DbClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class DBConnectionCommand : DBCommand
    {
        private readonly IDbConnection dbConnection;

        internal DBConnectionCommand(IDbConnection dbConnection, string commandText)
        {
            this.dbConnection = dbConnection;
            this.dbCommand = this.dbConnection.CreateCommand();
            this.dbCommand.CommandText = commandText;
        }

        public override int ExecuteNonQuery()
        {
            var rowsAffected = default(int);

            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.Open();

                    using (this.dbCommand)
                    {
                        rowsAffected = this.dbCommand.ExecuteNonQuery();

                        this.dbCommand.Log();
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }

            return rowsAffected;
        }

        public override void ExecuteReader(Action<DBReader> readHandler)
        {
            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.Open();

                    using (this.dbCommand)
                    {
                        using (var dataReader = this.dbCommand.ExecuteReader())
                        {
                            var dbReader = new DBReader(dataReader);

                            while (dbReader.Read())
                            {
                                readHandler(dbReader);
                            }
                        }

                        this.dbCommand.Log();
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }
        }

        public override List<T> ExecuteReader<T>(Func<DBReader, T> readHandler)
        {
            var results = new List<T>();

            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.Open();

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
                        }

                        this.dbCommand.Log();
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }

            return results;
        }

        public override T ExecuteScalar<T>()
        {
            var result = default(T);

            try
            {
                using (this.dbConnection)
                {
                    this.dbConnection.Open();

                    using (this.dbCommand)
                    {
                        result = this.dbCommand
                            .ExecuteScalar()
                            .TryParseValueOrDefault<T>();

                        this.dbCommand.Log();
                    }
                }
            }
            catch (Exception e)
            {
                e.Log();
            }

            return result;
        }
    }
}

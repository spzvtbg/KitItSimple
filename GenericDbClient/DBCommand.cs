namespace GenericDbClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public abstract class DBCommand
    {
        protected IDbCommand dbCommand;

        public abstract int ExecuteNonQuery();

        public abstract void ExecuteReader(Action<DBReader> readHandler);

        public abstract List<T> ExecuteReader<T>(Func<DBReader, T> readHandler);

        public abstract T ExecuteScalar<T>();

        public DBCommand SetCommandTimeout(int commandTimeout)
        {
            this.dbCommand.CommandTimeout = commandTimeout;

            return this;
        }

        public DBCommand SetCommandType(CommandType commandType)
        {
            this.dbCommand.CommandType = commandType;

            return this;
        }

        public DBCommand SetParameter(string name, object value)
        {
#if NET40_OR_GREATER || NET5_0_OR_GREATER 
            if (!string.IsNullOrWhiteSpace(name))
#else
            if (!string.IsNullOrEmpty(name))
#endif
            {
                if (value == null)
                {
                    value = DBNull.Value;
                }

                var parameter = this.dbCommand.CreateParameter();
                parameter.ParameterName = name;
                parameter.Value = value;

                _ = this.dbCommand.Parameters.Add(parameter);
            }

            return this;
        }
    }
}

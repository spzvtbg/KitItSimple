namespace KitItSimple.DbClient
{
    using System.Data.Common;

    public class DBReader
    {
        private readonly DbDataReader dbDataReader;

        private DBReader(DbDataReader dbDataReader) => this.dbDataReader = dbDataReader;

        public T GetValue<T>(int columnIndex)
        {
            if (0 <= columnIndex && columnIndex < this.dbDataReader.FieldCount && !this.dbDataReader.IsDBNull(columnIndex))
            {
                return this.dbDataReader
                    .GetValue(columnIndex)
                    .TryParseValueOrDefault<T>();
            }

            return default;
        }

        public bool Read() => this.dbDataReader.Read();

        public static implicit operator DBReader(DbDataReader dbDataReader)
            => new DBReader(dbDataReader);
    }
}
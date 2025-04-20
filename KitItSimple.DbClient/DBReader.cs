namespace KitItSimple.DbClient
{
    using System.Data;

    public class DBReader
    {
        private readonly IDataReader dataReader;

        internal DBReader(IDataReader dataReader) => this.dataReader = dataReader;

        public T GetValue<T>(int columnIndex)
        {
            if (0 <= columnIndex && columnIndex < this.dataReader.FieldCount && !this.dataReader.IsDBNull(columnIndex))
            {
                return this.dataReader
                    .GetValue(columnIndex)
                    .TryParseValueOrDefault<T>();
            }

            return default;
        }

        public bool Read() => this.dataReader.Read();
    }
}
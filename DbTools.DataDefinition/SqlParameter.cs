namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class SqlParameter : SqlElementWithNameAndType, ISqlParameter
    {
        public SqlParameter(DatabaseDefinition databaseDefinition)
        {
            DatabaseDefinition = databaseDefinition;
        }

        protected override DatabaseDefinition DatabaseDefinition { get; }

        public static implicit operator SqlParameter(SqlColumn column)
        {
            var parameter = new SqlParameter(column.Table.DatabaseDefinition)
            {
                Name = column.Name
            };

            foreach (var type in column.Types)
                parameter.Types.Add(type.Key, type.Value);

            return parameter;
        }

        public SqlParameter Copy(string newName)
        {
            var parameter = new SqlParameter(DatabaseDefinition)
            {
                Name = newName
            };

            foreach (var type in Types)
                parameter.Types.Add(type.Key, type.Value);

            return parameter;
        }
    }
}
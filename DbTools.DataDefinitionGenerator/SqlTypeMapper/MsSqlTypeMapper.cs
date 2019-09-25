namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class MsSqlTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            return type switch
            {
                SqlType.DateTime => "DATETIME2",

                _ => base.GetType(type),
            };
        }
    }
}
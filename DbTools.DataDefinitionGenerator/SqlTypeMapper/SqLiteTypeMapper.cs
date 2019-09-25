namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class SqLiteTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            return type switch
            {
                SqlType.DateTime => "TEXT /* datetime */",

                _ => base.GetType(type),
            };
        }
    }
}
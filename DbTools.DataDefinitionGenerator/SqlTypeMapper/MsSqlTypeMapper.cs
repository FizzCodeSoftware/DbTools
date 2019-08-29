namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class MsSqlTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            switch (type)
            {
                case SqlType.DateTime:
                    return "DATETIME2";

                default:
                    return base.GetType(type);
            }
        }
    }
}
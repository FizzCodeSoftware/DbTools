namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class SqLiteTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            switch (type)
            {
                case SqlType.DateTime:
                    return "TEXT /* datetime */";

                default:
                    return base.GetType(type);
            }
        }
    }
}
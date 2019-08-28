namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class OracleTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            switch (type)
            {
                case SqlType.NVarchar:
                    return "VARCHAR2";

                default:
                    return base.GetType(type);
            }
        }
    }
}
namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class OracleTypeMapper : GenericSqlTypeMapper
    {
        public override string GetType(SqlType type)
        {
            return type switch
            {
                SqlType.NVarchar => "VARCHAR2",

                _ => base.GetType(type),
            };
        }
    }
}
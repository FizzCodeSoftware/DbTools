using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public class SqlColumnCustomProperty : SqlColumnProperty
{
    public SqlColumnCustomProperty(SqlColumn sqlColumn)
        : base(sqlColumn)
    {
    }
}
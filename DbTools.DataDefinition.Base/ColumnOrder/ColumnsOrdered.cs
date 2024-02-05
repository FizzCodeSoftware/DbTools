using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnsOrdered : OrderedDictionaryWithInsert<string, SqlColumn>
{
    public override void Add(SqlColumn item)
    {
        Throw.InvalidOperationExceptionIfNull(item.Name, "SqlColumn.Name");
        Add(item.Name, item);
    }
}

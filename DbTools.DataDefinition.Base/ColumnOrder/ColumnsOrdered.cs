namespace FizzCode.DbTools.DataDefinition.Base;

public class ColumnsOrdered : OrderedDictionaryWithInsert<string, SqlColumn>
{
    public override void Add(SqlColumn item)
    {
        Add(item.Name, item);
    }
}

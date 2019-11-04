namespace FizzCode.DbTools.DataDefinition
{
    public class ColumnsOrdered : OrderedDictionaryWithInsert<string, SqlColumn>
    {
        public override void Add(SqlColumn item)
        {
            Add(item.Name, item);
        }
    }
}

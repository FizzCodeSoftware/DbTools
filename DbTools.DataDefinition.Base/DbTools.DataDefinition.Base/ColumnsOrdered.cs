namespace FizzCode.DbTools.DataDefinition.Base
{
    public class ColumnsOrdered : OrderedDictionaryWithInsert<string, SqlColumn>
    {
        public override void Add(SqlColumn item)
        {
            Add(item.Name, item);
        }
    }

    public class ViewColumnsOrdered : OrderedDictionaryWithInsert<string, SqlViewColumn>
    {
        public override void Add(SqlViewColumn item)
        {
            Add(item.Name, item);
        }
    }
}

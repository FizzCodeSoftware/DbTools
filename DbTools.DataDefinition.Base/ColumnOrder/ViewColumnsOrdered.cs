namespace FizzCode.DbTools.DataDefinition.Base
{
    public class ViewColumnsOrdered : OrderedDictionaryWithInsert<string, SqlViewColumn>
    {
        public override void Add(SqlViewColumn item)
        {
            Add(item.Name, item);
        }
    }
}

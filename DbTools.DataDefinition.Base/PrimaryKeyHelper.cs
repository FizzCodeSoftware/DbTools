namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Linq;

    public static class PrimaryKeyHelper
    {
        public static void SetPK(this SqlTable table, SqlColumn column, AscDesc order = AscDesc.Asc, string name = null)
        {
            var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();
            if (pk == null)
            {
                pk = new PrimaryKey(table, name);
                table.Properties.Add(pk);
            }

            pk.SqlColumns.Add(new ColumnAndOrder(column, order));
        }
    }
}
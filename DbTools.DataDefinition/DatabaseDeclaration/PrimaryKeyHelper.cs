namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public static class PrimaryKeyHelper
    {
        public static void SetPK(this SqlTableDeclaration table, SqlColumnDeclaration column, AscDesc order = AscDesc.Asc, string name = null)
        {
            var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();
            if (pk == null)
            {
                pk = new PrimaryKey((SqlTable)table, name);

                if (name == null)
                {
                    var pkNaming = table.DatabaseDeclaration?.NamingStrategies.GetNamingStrategy<IPrimaryKeyNamingStrategy>();
                    pkNaming?.SetPrimaryKeyName(pk);
                }

                if(pk.Name == null)
                    table.DelayedNamingTasks.Add(new DelayedNamingPrimaryKey(pk));

                table.Properties.Add(pk);
            }

            pk.SqlColumns.Add(new ColumnAndOrder(column, order));
        }
    }
}
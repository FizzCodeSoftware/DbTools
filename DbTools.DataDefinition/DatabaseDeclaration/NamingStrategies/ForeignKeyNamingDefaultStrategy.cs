namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    public class ForeignKeyNamingDefaultStrategy : IForeignKeyNamingStrategy
    {
        public virtual void SetFKName(ForeignKey fk)
        {
            if (fk.SqlTable.Name == null)
                return;

            fk.Name = $"FK_{fk.SqlTable.Name}__{string.Join("__", fk.ForeignKeyColumns.Select(y => y.ForeignKeyColumn.Name))}";
        }

        public virtual void SetFKColumnsNames(ForeignKey fk, string prefix)
        {
            foreach (var fkColumn in fk.ForeignKeyColumns)
            {
                string columnName;
                if (prefix != null)
                {
                    if (fkColumn.PrimaryKeyColumn.Name == null)
                        return;

                    columnName = $"{prefix}{fkColumn.PrimaryKeyColumn.Name}";
                }
                else
                {
                    if (fk.PrimaryKey.SqlTable.Name == null || fkColumn.PrimaryKeyColumn.Name == null)
                        return;

                    columnName = $"{fk.PrimaryKey.SqlTable.Name}{fkColumn.PrimaryKeyColumn.Name}";
                }

                fkColumn.ForeignKeyColumn.Name = columnName;
            }
        }
    }
}

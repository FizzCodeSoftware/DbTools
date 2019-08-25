using System.Collections.Generic;
using System.Linq;

namespace FizzCode.DbTools.DataDefinition
{
    public class SqlTableDeclaration : SqlTable
    {
        public List<DelayedNamingTask> DelayedNamingTasks { get; } = new List<DelayedNamingTask>();

        public SqlTableDeclaration()
            : base((SchemaAndTableName)null)
        {
        }

        public void SetLazyProperties(SchemaAndTableName schemaAndTableName, DatabaseDeclaration databaseDeclaration)
        {
            SchemaAndTableName = schemaAndTableName;
            DatabaseDeclaration = databaseDeclaration;
        }

        internal DatabaseDeclaration DatabaseDeclaration { get; private set; }

        public List<SqlColumnLazyForeignKey> LazyForeignKeys()
        {
            var result = new List<SqlColumnLazyForeignKey>();

            foreach (var column in Columns.Values.Where(c => c is SqlColumnLazyForeignKey))
                result.Add((SqlColumnLazyForeignKey)column);

            return result;
        }

        public new SqlColumnDeclaration this[string columnName] => (SqlColumnDeclaration)Columns[columnName];
    }
}

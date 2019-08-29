namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using FizzCode.DbTools.DataDefinition;

    public class SqLiteGenerator : GenericSqlGenerator
    {
        public override ISqlTypeMapper SqlTypeMapper { get; } = new SqLiteTypeMapper();

        protected override string GuardKeywords(string name)
        {
            return $"\"{name}\"";
        }

        public override string DropAllTables()
        {
            throw new System.NotImplementedException();
        }

        public override SqlStatementWithParameters TableExists(SqlTable table)
        {
            throw new System.NotImplementedException();
        }

        public override string DropAllIndexes()
        {
            throw new System.NotImplementedException();
        }

        public override string CreateTable(SqlTable table)
        {
            return CreateTableInternal(table, true);
        }

        // SqLite does not support ALTER TABLE ... ADD CONSTRAINT
        public override string CreateForeignKeys(SqlTable table)
        {
            return "";
        }
    }
}
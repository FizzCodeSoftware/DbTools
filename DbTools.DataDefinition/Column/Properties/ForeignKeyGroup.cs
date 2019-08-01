namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyGroup
    {
        public string ColumnName { get; }
        public SqlColumnDeclaration PKColumn { get; }

        public ForeignKeyGroup(string name, SqlColumnDeclaration pkColumn)
        {
            ColumnName = name;
            PKColumn = pkColumn;
        }
    }
}
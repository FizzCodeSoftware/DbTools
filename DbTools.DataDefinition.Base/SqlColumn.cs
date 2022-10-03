namespace FizzCode.DbTools.DataDefinition.Base
{
    public class SqlColumn : SqlColumnBase
    {
        public SqlTable Table { get => (SqlTable)SqlTableOrView; set => SqlTableOrView = value; }

        public SqlColumn SetPK()
        {
            Table.SetPK(this);
            return this;
        }

        public SqlColumn SetIdentity()
        {
            Properties.Add(new Identity(this));
            return this;
        }

        public SqlColumn CopyTo(SqlColumn column)
        {
            var copy = base.CopyTo(column);
            return (SqlColumn)copy;
        }
    }
}
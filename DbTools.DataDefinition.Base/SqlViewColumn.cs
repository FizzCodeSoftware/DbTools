namespace FizzCode.DbTools.DataDefinition.Base;

public class SqlViewColumn : SqlColumnBase
{
    public SqlView View { get => (SqlView)SqlTableOrView; set => SqlTableOrView = value; }
}
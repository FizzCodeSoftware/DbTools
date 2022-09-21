namespace FizzCode.DbTools.DataDefinition.Base;

public class SqlColumnFKRegistration : SqlColumn
{
    public ForeignKeyRegistrationNonExsistingColumn FKRegistration { get; set; }

    public SqlColumnFKRegistration(string name, ForeignKeyRegistrationNonExsistingColumn fk)
    {
        Name = name;
        FKRegistration = fk;
    }
}

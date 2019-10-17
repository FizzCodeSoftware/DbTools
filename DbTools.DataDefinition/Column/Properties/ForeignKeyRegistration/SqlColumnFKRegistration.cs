namespace FizzCode.DbTools.DataDefinition
{
    internal class SqlColumnFKRegistration : SqlColumn
    {
        public ForeignKeyRegistrationNonExsistingColumn FKRegistration { get; set; }

        public SqlColumnFKRegistration(string name, ForeignKeyRegistrationNonExsistingColumn fk)
        {
            Name = name;
            FKRegistration = fk;
        }
    }
}

namespace FizzCode.DbTools.TestBase
{
    using System;
    using FizzCode.DbTools.DataDefinition;

    internal class SqlConnectionKeyAndDatabaseDefinitionTypeAsKey
    {
        private readonly string _connectionStringKey;
        private readonly string _databaseDefinitionTypeFullName;
        internal SqlConnectionKeyAndDatabaseDefinitionTypeAsKey(string connectionStringKey, DatabaseDefinition dd)
        {
            _connectionStringKey = connectionStringKey;
            _databaseDefinitionTypeFullName = dd.GetType().FullName;
        }

        public override string ToString()
        {
            return _connectionStringKey + "_" + _databaseDefinitionTypeFullName;
        }

        public override bool Equals(object obj)
        {
            return obj is SqlConnectionKeyAndDatabaseDefinitionTypeAsKey s && s.ToString() == ToString();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ToString());
        }
    }
}
using System;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;

namespace FizzCode.DbTools.TestBase;
internal class SqlConnectionKeyAndDatabaseDefinitionTypeAsKey
{
    private readonly string _connectionStringKey;
    private readonly string _databaseDefinitionTypeFullName;
    internal SqlConnectionKeyAndDatabaseDefinitionTypeAsKey(string connectionStringKey, DatabaseDefinition dd)
    {
        _connectionStringKey = connectionStringKey;
        var fullName = dd.GetType().FullName;
        Throw.InvalidOperationExceptionIfNull(fullName, "dd.GetType().FullName");
        _databaseDefinitionTypeFullName = fullName!;
    }

    public override string ToString()
    {
        return _connectionStringKey + "_" + _databaseDefinitionTypeFullName;
    }

    public override bool Equals(object? obj)
    {
        return obj is SqlConnectionKeyAndDatabaseDefinitionTypeAsKey s && s.ToString() == ToString();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ToString());
    }
}
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDeclaration;

public class DefaultValueNamingDefaultStrategy : IDefaultValueNamingStrategy
{
    public void SetDefaultValueName(DefaultValue defaultValue)
    {
        Throw.InvalidOperationExceptionIfNull(defaultValue.SqlColumn.SqlTableOrView);
        Throw.InvalidOperationExceptionIfNull(defaultValue.SqlColumn.SqlTableOrView.SchemaAndTableName);
        Throw.InvalidOperationExceptionIfNull(defaultValue.SqlColumn.Name);
        defaultValue.Name = $"DF_{defaultValue.SqlColumn.SqlTableOrView.SchemaAndTableName.TableName}_{defaultValue.SqlColumn.Name}";
    }
}
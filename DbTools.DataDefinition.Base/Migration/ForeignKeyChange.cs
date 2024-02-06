namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class ForeignKeyChange : ForeignKeyMigration
{
    public required ForeignKey NewForeignKey { get; init; }

    public ForeignKeyInternalColumnChanges? ForeignKeyInternalColumnChanges { get; set; }
    public List<SqlEngineVersionSpecificPropertyMigration> SqlEngineVersionSpecificPropertyChanges { get; } = [];
}

public class ForeignKeyInternalColumnChanges : IMigration
{
}

public abstract class SqlEngineVersionSpecificPropertyMigration : IMigration
{
    public SqlEngineVersionSpecificProperty SqlEngineVersionSpecificProperty { get; set; }
}

public class SqlEngineVersionSpecificPropertyNew : SqlEngineVersionSpecificPropertyMigration
{
}

public class SqlEngineVersionSpecificPropertyDelete : SqlEngineVersionSpecificPropertyMigration
{
}

public class SqlEngineVersionSpecificPropertyChange : SqlEngineVersionSpecificPropertyMigration
{
    public SqlEngineVersionSpecificProperty NewSqlEngineVersionSpecificProperty { get; set; }
}

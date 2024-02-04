namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class ForeignKeyChange : ForeignKeyMigration
{
    public ForeignKey NewForeignKey { get; set; }

    public ForeignKeyInternalColumnChanges ForeignKeyInternalColumnChanges { get; set; }
    public List<SqlEngineVersionSpecificPropertyMigration> SqlEngineVersionSpecificPropertyChanges { get; } = new List<SqlEngineVersionSpecificPropertyMigration>();
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

using FizzCode.DbTools.DataDefinition.Base.Interfaces;

namespace FizzCode.DbTools.DataDefinition.Base;
public abstract class SqlElementWithNameAndType
{
    public string Name { get; set; }
    public SqlTypes Types { get; } = new SqlTypes();

    protected abstract IDatabaseDefinition DatabaseDefinition { get; }

    public ISqlType? Type
    {
        get
        {
            if (DatabaseDefinition?.MainVersion != null)
                return Types[DatabaseDefinition.MainVersion];

            if (Types.Count == 1)
                return Types.First().Value;

            return null;
        }
    }
}
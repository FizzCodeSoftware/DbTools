using FizzCode.DbTools.DataDefinition.Base.Interfaces;

namespace FizzCode.DbTools.DataDefinition.Base;
public abstract class SqlElementWithNameAndType
{
    /// <summary>
    /// The Name of the <see cref="SqlElementWithNameAndType">Sql element</see>.
    /// Note that with declaration, first this will be null, and will be set from the constructor of <see cref="DatabaseDeclaration">.
    /// </summary>
    public string? Name { get; set; }
    public SqlTypes Types { get; } = [];

    protected abstract IDatabaseDefinition? DatabaseDefinition { get; }

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
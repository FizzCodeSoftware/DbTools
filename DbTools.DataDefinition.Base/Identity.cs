using System.Globalization;

namespace FizzCode.DbTools.DataDefinition.Base;
public class Identity : SqlColumnProperty
{
    public int Increment { get; set; } = 1;
    public int Seed { get; set; } = 1;

    public Identity(SqlColumn sqlColumn)
        : base(sqlColumn)
    {
    }

    public override string ToString()
    {
        return $"{SqlColumn.Name} ({Seed.ToString(CultureInfo.InvariantCulture)}, {Increment.ToString(CultureInfo.InvariantCulture)})";
    }
}
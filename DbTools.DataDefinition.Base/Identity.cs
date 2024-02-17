using System.Globalization;

namespace FizzCode.DbTools.DataDefinition.Base;
public class Identity(SqlColumn sqlColumn)
    : SqlColumnProperty(sqlColumn)
{
    public int Increment { get; set; } = 1;
    public int Seed { get; set; } = 1;

    public override string ToString()
    {
        return $"{SqlColumn.Name} ({Seed.ToString(CultureInfo.InvariantCulture)}, {Increment.ToString(CultureInfo.InvariantCulture)})";
    }
}
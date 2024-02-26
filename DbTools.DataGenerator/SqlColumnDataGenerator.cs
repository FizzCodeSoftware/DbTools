using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataGenerator;
/// <summary>
/// Column Property containing information about how to generate data for the column.
/// <see cref="Generator"/> holds a reference to the data generator.
/// </summary>
public class SqlColumnDataGenerator(SqlColumn sqlColumn, GeneratorBase generator) : SqlColumnCustomProperty(sqlColumn)
{
    public GeneratorBase Generator { get; } = generator;
}

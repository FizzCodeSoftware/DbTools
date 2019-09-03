namespace FizzCode.DbTools.DataGenerator
{
    using FizzCode.DbTools.DataDefinition;

    /// <summary>
    /// Column Property containing information about how to generate data for the column.
    /// <see cref="SqlColumnDataGenerator.Generator"/> holds a reference to the data generator.
    /// </summary>
    public class SqlColumnDataGenerator : SqlColumnCustomProperty
    {
        public GeneratorBase Generator { get; }

        public SqlColumnDataGenerator(SqlColumn sqlColumn, GeneratorBase generator) : base(sqlColumn)
        {
            Generator = generator;
        }
    }
}

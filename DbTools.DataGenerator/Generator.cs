namespace FizzCode.DbTools.DataGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class Generator
    {
        private readonly GeneratorContext Context;

        public Generator(GeneratorContext context)
        {
            Context = context;
        }

        // Get a SqlTable
        // * generate rows
        // - either object[]
        // - rows
        //   DataDefinitionExecuter.Row
        // * make use in Etl as generating rows
        // * create insert or update existing
        // * ensure FKs
        // * add some governing infos (type of data like name, adress, number ranges etc.)
        public IEnumerable<Row> Generate(SqlTable table, int numberOfRows)
        {
            for (var i = 0; i < numberOfRows; i++)
            {
                yield return Generate(table);
            }
        }

        public Row Generate(SqlTable table)
        {
            var row = new Row();

            foreach (var column in table.Columns.Values)
            {
                GenerateValue(row, column);
            }

            return row;
        }

        private void GenerateValue(Row row, SqlColumn column)
        {
            var sqlColumnDataGenerator = column.Properties.OfType<SqlColumnDataGenerator>().FirstOrDefault();

            var generator = sqlColumnDataGenerator?.Generator;

            if (generator == null)
            {
                if (!column.IsNullable)
                    generator = GetDefaultGenerator(column);
            }

            if (generator != null)
            {
                generator.SetContext(Context);
                row.Add(column.Name, generator.Get());
            }
        }

        private GeneratorBase GetDefaultGenerator(SqlColumn column)
        {
            switch (column.Type)
            {
                case SqlType.NVarchar:
                    return new GeneratorString(1, column.Length.Value + 1);
                case SqlType.Int32:
                    return new GeneratorInt32();
                case SqlType.Date:
                    return new GeneratorDateMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1));
                case SqlType.DateTime:
                    return new GeneratorDateTimeMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1));
                default:
                    throw new NotImplementedException($"Unhandled SqlType: {Enum.GetName(typeof(SqlType), column.Type)}");
            }
        }
    }
}

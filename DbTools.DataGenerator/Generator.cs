namespace FizzCode.DbTools.DataGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class Generator
    {
        private readonly GeneratorContext Context;

        // TODO
        private readonly SqlVersion Version;

        public Generator(GeneratorContext context)
        {
            Context = context;
            Version = new Generic1();
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

            foreach (var column in table.Columns)
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
                if (!column.Types[Version].IsNullable)
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
            var type = column.Types[Version];
            return type.SqlTypeInfo.DbType switch
            {
                "NVARCHAR" => new GeneratorString(1, type.Length.Value + 1),
                "INT32" => new GeneratorInt32(),
                "DATE" => new GeneratorDateMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),
                "DATETIME" => new GeneratorDateTimeMinMax(new DateTime(1950, 1, 1), new DateTime(2050, 1, 1)),
                _ => throw new NotImplementedException($"Unhandled type: {type.SqlTypeInfo.DbType}"),
            };
        }
    }
}

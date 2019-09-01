namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public class OracleGenerator : MsSqlGenerator
    {
        public OracleGenerator(Settings settings) : base(settings)
        {
        }

        public override ISqlTypeMapper SqlTypeMapper { get; } = new OracleTypeMapper();

        protected override string GuardKeywords(string name)
        {
            return $"\"{ name}\"";
        }

        protected override void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
        {
            // TODO REVERSE index
            sb.Append(" GENERATED ALWAYS AS IDENTITY START WITH ")
                .Append(identity.Seed)
                .Append(" INCREMENT BY ")
                .Append(identity.Increment);
        }

        protected override void CreateTablePrimaryKey(SqlTable table, StringBuilder sb)
        {
            // TODO NO CLUSTERED/NONCLUSTERED
            // TODO NO ASC/DESC
            // example: CONSTRAINT [PK_dbo.AddressShort] PRIMARY KEY ([Id] )
            var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();

            if (pk == null || pk.SqlColumns.Count == 0)
                return;

            sb.Append(", CONSTRAINT ")
                .Append(GuardKeywords(pk.Name))
                .Append(" PRIMARY KEY ")
                .AppendLine("(")
                .AppendLine(string.Join(", \r\n", pk.SqlColumns.Select(c => GuardKeywords(c.SqlColumn.Name))))
                .Append(")");
        }

        public SqlStatementWithParameters IfExists(string table, string column, object value)
        {
            return new SqlStatementWithParameters($@"
SELECT CASE WHEN MAX({column}) IS NULL THEN 1 ELSE 0 END
FROM {table}
WHERE {column} = @{column}", value);
        }

        public override string CreateForeignKeys(SqlTable table)
        {
            /* example: ALTER TABLE [dbo].[Dim_Currency] ADD CONSTRAINT [FK_Dim_Currency_Dim_CurrencyGroup] FOREIGN KEY([Dim_CurrencyGroupId])
            REFERENCES[dbo].[Dim_CurrencyGroup]([Dim_CurrencyGroupId])
s            */

            // TODO initially deferred / DEFERRABLE 

            var allFks = table.Properties.OfType<ForeignKey>().ToList();

            if (allFks.Count == 0)
                return null;

            var sb = new StringBuilder();

            foreach (var fk in allFks)
            {
                sb.Append("ALTER TABLE ")
                    .Append(SchemaAndTableName(table.SchemaAndTableName, GuardKeywords))
                    .Append(" ADD ")
                    .Append(ForeignKeyGeneratorHelper.FKConstraint(fk, GuardKeywords))
                    .AppendLine(";");
            }

            return sb.ToString();
        }
    }
}

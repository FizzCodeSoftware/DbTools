namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public class OracleGenerator12c : GenericSqlGenerator
    {
        public OracleGenerator12c(Context context)
            : base(context)
        {
            Version = SqlVersions.Oracle12c;
        }

        protected override string GuardKeywords(string name)
        {
            return $"\"{ name}\"";
        }

        public override SqlStatementWithParameters CreateSchema(string schemaName)
        {
            // TODO password
            var sqlStatementWithParameters = new SqlStatementWithParameters(@$"
CREATE USER ""{schemaName}"" IDENTIFIED BY sa123;
GRANT CONNECT, DBA TO ""{schemaName}"";
GRANT CREATE SESSION TO ""{schemaName}"";
GRANT UNLIMITED TABLESPACE TO ""{schemaName}""");

            //sqlStatementWithParameters.Parameters.Add("@schemaName", schemaName);

            return sqlStatementWithParameters;
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

        public static SqlStatementWithParameters IfExists(string table, string column, object value)
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
                sb.Append(GrantReferenceAcrossSchemas(fk));
                sb.Append("ALTER TABLE ")
                    .Append(GetSimplifiedSchemaAndTableName(table.SchemaAndTableName))
                    .Append(" ADD ")
                    .Append(FKConstraint(fk))
                    .AppendLine(";");
            }

            return sb.ToString();
        }

        private string GrantReferenceAcrossSchemas(ForeignKey fk)
        {
            if (fk.SqlTable.SchemaAndTableName.Schema != fk.ReferredTable.SchemaAndTableName.Schema)
            {
                return $"GRANT REFERENCES ON {GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName)} TO \"{fk.SqlTable.SchemaAndTableName.Schema}\";";
            }

            return null;
        }

        public override string DropAllForeignKeys()
        {
            throw new NotImplementedException();
        }

        public override string DropAllViews()
        {
            throw new NotImplementedException();
        }

        public override string DropAllTables()
        {
            throw new NotImplementedException();
        }

        public override string DropAllIndexes()
        {
            throw new NotImplementedException();
        }

        public override SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false)
        {
            var cascade = hard ? " CASCADE" : "";
            return string.Join(Environment.NewLine, schemaNames.Select(x => "DROP USER " + GuardKeywords(x) + $"{cascade}" + ";"));
        }
    }
}

namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    public class OracleGenerator : MsSqlGenerator
    {
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

        public SqlStatementWithParameters IfExists(SqlStatementWithParameters ifExistsCondition, SqlStatementWithParameters ifExists, SqlStatementWithParameters ifNotExists)
        {
            var sql = string.Format(SqlIfExists, ifExistsCondition.Statement, ifExists.Statement, ifNotExists.Statement);
            var unionParameters = ifExistsCondition.Parameters.Union(ifExists.Parameters.Union(ifNotExists.Parameters)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            var sqls = new SqlStatementWithParameters(sql, unionParameters);

            return sqls;
        }

        private const string SqlIfExists = @"
DECLARE
    l_exst number(1);
BEGIN
    SELECT CASE 
        WHEN EXISTS({0})
        THEN 1
        ELSE 0
    END INTO l_exst
    FROM dual;

    IF l_exst = 1 
    THEN
        {1}
    ELSE
        {2}
    END IF;
END;";
    }
}

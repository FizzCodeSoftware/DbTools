namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using System.Linq;

    public class StoredProcedure
    {
        public StoredProcedure(string sqlStatementBody, params SqlParameter[] sqlParameters)
        {
            StoredProcedureBodies.Add(GenericVersion.Generic1, sqlStatementBody);
            SpParameters = sqlParameters.ToList();
        }

        public StoredProcedure(SqlEngineVersion version, string sqlStatementBody, params SqlParameter[] sqlParameters)
        {
            StoredProcedureBodies.Add(version, sqlStatementBody);
            SpParameters = sqlParameters.ToList();
        }

        public StoredProcedureBodies StoredProcedureBodies { get; } = new();

        public IDatabaseDefinition? DatabaseDefinition { get; set; }
        public SchemaAndTableName? SchemaAndSpName { get; set; }

        public List<SqlParameter> SpParameters { get; } = new List<SqlParameter>();
    }
}

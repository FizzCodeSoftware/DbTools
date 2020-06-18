namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class StoredProcedure
    {
        public StoredProcedure(string sqlStatementBody, params SpParameter[] spParameters)
        {
            SqlStatementBody = sqlStatementBody;
            SpParameters = spParameters.ToList();

            foreach(var spParameter in spParameters)
                spParameter.StoredProcedure = this;
        }

        public string  SqlStatementBody { get; set; }

        public DatabaseDefinition DatabaseDefinition { get; set; }
        public SchemaAndTableName SchemaAndSpName { get; set; }

        public List<SpParameter> SpParameters { get; } = new List<SpParameter>();
    }
}

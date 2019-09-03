namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Data.Common;

    public class OracleDbCommandPreparer
    {
        public DbCommand PrepareSqlCommand(DbCommand dbCommand)
        {
            dbCommand.GetType().GetProperty("BindByName").SetValue(dbCommand, true, null);
            ReplaceNamedParameterPrefixes(dbCommand);
            return dbCommand;
        }

        private void ReplaceNamedParameterPrefixes(DbCommand dbCommand)
        {
            dbCommand.CommandText = dbCommand.CommandText.Replace(" @", " :"); //replace named parameter indicators

            foreach (var paramter in dbCommand.Parameters)
            {
                var dbParameter = (DbParameter)paramter;
                dbParameter.ParameterName = ":" + dbParameter.ParameterName.TrimStart('@');
            }
        }
    }
}
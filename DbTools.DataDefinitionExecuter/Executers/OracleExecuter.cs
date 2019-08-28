namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class OracleExecuter : MsSqlExecuter
    {
        protected override SqlDialect SqlDialect => SqlDialect.Oracle;

        public OracleExecuter(ConnectionStringSettings connectionStringSettings, ISqlGenerator sqlGenerator) : base(connectionStringSettings, sqlGenerator)
        {
        }

        public override DbConnection OpenConnectionMaster()
        {
            return OpenConnection();
        }

        public override string GetDatabase(DbConnectionStringBuilder builder)
        {
            var oracleDatabaseNameKey = ConnectionStringSettings.Name + "_Database_Name";
            var oracleDatabaseName = ConfigurationManager.AppSettings[oracleDatabaseNameKey];
            return oracleDatabaseName;
        }
    }
}
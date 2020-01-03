namespace FizzCode.DbTools.DataDefinitionExecuter
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public abstract class DatabaseTask
    {
        protected DatabaseTask(SqlExecuter sqlExecuter)
        {
            Executer = sqlExecuter;
        }

        protected SqlExecuter Executer { get; }
    }
}
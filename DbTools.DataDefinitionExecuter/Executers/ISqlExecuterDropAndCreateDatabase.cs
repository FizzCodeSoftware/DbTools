namespace FizzCode.DbTools.DataDefinitionExecuter
{
    public interface ISqlExecuterDropAndCreateDatabase : ISqlExecuter
    {
        void CreateDatabase();
        void DropDatabaseIfExists();
        void DropDatabase();
    }
}
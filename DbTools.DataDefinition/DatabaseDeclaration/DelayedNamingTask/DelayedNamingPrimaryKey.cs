namespace FizzCode.DbTools.DataDefinition
{
    public class DelayedNamingPrimaryKey : DelayedNamingTask
    {
        private readonly PrimaryKey _pk;

        public DelayedNamingPrimaryKey(PrimaryKey pk)
        {
            _pk = pk;
        }

        public override void Resolve(NamingStrategiesDictionary namingStrategies)
        {
            var pkNaming = namingStrategies.GetNamingStrategy<IPrimaryKeyNamingStrategy>();
            pkNaming.SetPrimaryKeyName(_pk);
        }
    }
}

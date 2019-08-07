namespace FizzCode.DbTools.DataDefinition
{
    public class DelayedNamingIndex : DelayedNamingTask
    {
        private readonly Index _index;

        public DelayedNamingIndex(Index index)
        {
            _index = index;
        }

        public override void Resolve(NamingStrategiesDictionary namingStrategies)
        {
            var indexNaming = namingStrategies.GetNamingStrategy<IIndexNamingStrategy>();
            indexNaming.SetIndexName(_index);
        }
    }
}

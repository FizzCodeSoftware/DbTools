namespace FizzCode.DbTools.DataDefinition
{
    public class DelayedNamingForeignKey : DelayedNamingTask
    {
        private readonly ForeignKey _fk;

        public DelayedNamingForeignKey(ForeignKey fk)
        {
            _fk = fk;
        }

        public override void Resolve(NamingStrategiesDictionary namingStrategies)
        {
            var fkNaming = namingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();
            fkNaming.SetFKName(_fk);
        }
    }
}

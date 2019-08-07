namespace FizzCode.DbTools.DataDefinition
{
    public class DelayedNamingForeignKeyColumn : DelayedNamingTask
    {
        private readonly ForeignKey _fk;

        public DelayedNamingForeignKeyColumn(ForeignKey fk)
        {
            _fk = fk;
        }

        public override void Resolve(NamingStrategiesDictionary namingStrategies)
        {
            var fkNaming = namingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();
            fkNaming.SetFKColumnsNames(_fk, NameBuildingParts[0]);
        }
    }
}

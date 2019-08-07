namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public abstract class DelayedNamingTask
    {
        public readonly List<string> NameBuildingParts = new List<string>();
        public abstract void Resolve(NamingStrategiesDictionary namingStrategies);
    }
}

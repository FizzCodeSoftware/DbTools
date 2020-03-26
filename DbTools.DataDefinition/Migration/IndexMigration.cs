namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class IndexMigration : IMigration
    {
        public Index Index { get; set; }
    }
}

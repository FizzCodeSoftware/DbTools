namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class SpParameter : SqlParameter
    {
        public StoredProcedure StoredProcedure { get; set; }

        protected override DatabaseDefinition DatabaseDefinition => StoredProcedure.DatabaseDefinition;
    }
}
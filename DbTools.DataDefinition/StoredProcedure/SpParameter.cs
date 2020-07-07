namespace FizzCode.DbTools.DataDefinition
{
    /*public class SpParameter : SqlParameter
    {
        //public StoredProcedure StoredProcedure { get; set; }

        //protected override DatabaseDefinition DatabaseDefinition => StoredProcedure.DatabaseDefinition;

        public SpParameter(DatabaseDefinition databaseDefinition)
        {
            DatabaseDefinition = databaseDefinition;
        }

        protected override DatabaseDefinition DatabaseDefinition { get; }

        public static implicit operator SpParameter(SqlColumn column)
        {
            var spParameter = new SpParameter(column.Table.DatabaseDefinition)
            {
                Name = column.Name
            };

            foreach (var type in column.Types)
                spParameter.Types.Add(type.Key, type.Value);

            return spParameter;
        }
    }*/
}
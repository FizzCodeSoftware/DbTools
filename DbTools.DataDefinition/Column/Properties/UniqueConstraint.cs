namespace FizzCode.DbTools.DataDefinition
{
    public class UniqueConstraint : IndexBase
    {
        public UniqueConstraint(SqlTable sqlTable, string name)
            : base(sqlTable, name, true)
        {
        }

#pragma warning disable CA1822 // Mark members as static
        public new bool Unique
#pragma warning restore CA1822 // Mark members as static
        {
            get => true;

            set
            {
                if (!value)
                    throw new System.ArgumentException("Unique Constraint is always Unique.");
            }
        }

        public override string ToString()
        {
            return $"{GetColumnsInString()} on {SqlTable.SchemaAndTableName}";
        }
    }
}

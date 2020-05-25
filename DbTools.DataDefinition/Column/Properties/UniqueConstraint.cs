namespace FizzCode.DbTools.DataDefinition
{
    public class UniqueConstraint : IndexBase
    {
        public UniqueConstraint(SqlTable sqlTable, string name)
            : base(sqlTable, name, true)
        {
        }

        public new bool Unique
        {
            get
            {
                return true;
            }

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

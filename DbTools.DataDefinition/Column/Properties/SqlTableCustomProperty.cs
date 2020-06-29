namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlTableCustomProperty : SqlTableProperty
    {
        protected SqlTableCustomProperty()
            : base(null)
        {
        }

        protected SqlTableCustomProperty(SqlTable sqlTable)
            : base(sqlTable)
        {
        }

        /// <summary>
        /// Override to provide the constructor parameters for the CSharp generator.
        /// This is only needed if the property has mandatory constructor paramters, other than the SqlTable.
        /// </summary>
        /// <returns>The constructor parametrs as CSharp code.</returns>
        public virtual string GenerateCSharpConstructorParameters()
        {
            return string.Empty;
        }
    }
}

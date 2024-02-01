namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;

    public interface IDatabaseDeclaration
    {
        void CreateRegisteredForeignKeys(SqlTable sqlTable);
        void AddAutoNaming(List<SqlTable> tables);
    }
}
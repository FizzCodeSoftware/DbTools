using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition.Base;
public interface IDatabaseDeclaration
{
    void CreateRegisteredForeignKeys(SqlTable sqlTable);
    void AddAutoNaming(List<SqlTable> tables);
}
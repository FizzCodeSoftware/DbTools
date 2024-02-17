using System.Collections.Generic;
using System.Globalization;
using FizzCode.DbTools.DataDeclaration;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class ForeignKeyNamingMsSqlDefaultStrategy : ForeignKeyNamingDefaultStrategy
{
    private readonly Dictionary<string, ForeignKey> _generatedNames = [];
    private readonly Dictionary<string, List<ForeignKey>> _renames = [];

    public override void SetFKName(ForeignKey fk)
    {
        if (fk.SqlTable.SchemaAndTableName == null || fk.ReferredTable?.SchemaAndTableName is null)
            return;

        var fkName = fk.SqlTable.SchemaAndTableName.TableName + "__" + fk.ReferredTable.SchemaAndTableName.TableName;
        if (fkName.Length > 110)
        {
            fkName = fkName.Substring(0, 110);
        }

        if (_generatedNames.TryGetValue(fkName, out var firstFk))
        {
            if (!_renames.TryGetValue(fkName, out var renameList))
            {
                renameList =
                [
                    firstFk,
                ];

                _renames.Add(fkName, renameList);
                firstFk.Name += "_1";
            }

            renameList.Add(fk);
            fk.Name = fkName + "_" + renameList.Count.ToString("D", CultureInfo.InvariantCulture);
        }
        else
        {
            _generatedNames.Add(fkName, fk);
            fk.Name = fkName;
        }
    }
}
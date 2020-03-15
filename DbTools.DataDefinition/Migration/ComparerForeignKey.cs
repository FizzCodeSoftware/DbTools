namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ComparerForeignKey
    {
        private static List<ForeignKeyMigration> CompareForeignKeys(SqlTable tableOriginal, SqlTable tableNew)
        {
            var changes = new List<ForeignKeyMigration>();

            foreach (var fkOriginal in tableOriginal.Properties.OfType<ForeignKey>())
            {
                if (!tableNew.Properties.OfType<ForeignKey>().Any(fkNew => fkNew.Name == fkOriginal.Name))
                {
                    changes.Add(new ForeignKeyDelete()
                    {
                        // ForeignKey = fkOriginal.CopyTo(new ForeignKey())
                        ForeignKey = fkOriginal
                    });
                }
            }

            // TODO detect name change? (same FK elements but different name?)
            foreach (var fkNew in tableNew.Properties.OfType<ForeignKey>())
            {
                var fkOriginal = tableOriginal.Properties.OfType<ForeignKey>().FirstOrDefault(fkOriginal => fkOriginal.Name == fkNew.Name);
                if (fkOriginal == null)
                {
                    changes.Add(new ForeignKeyNew()
                    {
                        ForeignKey = fkNew
                    });
                }
                else
                {
                    // compare ReferredTable
                    // compare columns
                    // compare SqlEngineVersionSpecificProperties - generally for SqlTableProperty
                    if (fkOriginal.ReferredTable.SchemaAndTableName != fkNew.ReferredTable.SchemaAndTableName)
                    {


                        changes.Add(new ForeignKeyChange()
                        {
                            ForeignKey = fkOriginal,
                            NewForeignKey = fkNew
                        });
                    }
                }
            }

            return changes;
        }

        public static bool CompareForeignKeyColumns(ForeignKey fkOriginal, ForeignKey fkNew)
        {
            if (fkOriginal.ForeignKeyColumns.Count != fkNew.ForeignKeyColumns.Count)
                return false;

            foreach (var fkColumnMapOriginal in fkOriginal.ForeignKeyColumns)
            {
                fkNew.ForeignKeyColumns.First(fkcm => fkcm.)
            }

            return true;
        }
    }
}

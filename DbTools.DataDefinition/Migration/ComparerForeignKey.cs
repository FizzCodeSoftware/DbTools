namespace FizzCode.DbTools.DataDefinition.Migration
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ComparerForeignKey
    {
        public static List<ForeignKeyMigration> CompareForeignKeys(SqlTable tableOriginal, SqlTable tableNew)
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
                    var fkChanged = false;
                    var foreignKeyChange = new ForeignKeyChange()
                    {
                        ForeignKey = fkOriginal,
                        NewForeignKey = fkNew
                    };

                    // compare ReferredTable
                    // compare columns
                    // compare SqlEngineVersionSpecificProperties - generally for SqlTableProperty
                    if (fkOriginal.ReferredTable.SchemaAndTableName != fkNew.ReferredTable.SchemaAndTableName)
                        fkChanged = true;

                    if (!CompareForeignKeyColumns(fkOriginal, fkNew))
                    {
                        fkChanged = true;
                        foreignKeyChange.ForeignKeyInternalColumnChanges = new ForeignKeyInternalColumnChanges();
                    }

                    var sqlEngineVersionSpecificPropertyChanges = CompareSqlEngineVersionSpecificPropertyChange(fkOriginal.SqlEngineVersionSpecificProperties, fkNew.SqlEngineVersionSpecificProperties);
                    if (sqlEngineVersionSpecificPropertyChanges.Count > 0)
                    {
                        fkChanged = true;
                        foreignKeyChange.SqlEngineVersionSpecificPropertyChanges.AddRange(sqlEngineVersionSpecificPropertyChanges);
                    }

                    if (fkChanged)
                        changes.Add(foreignKeyChange);
                }
            }

            return changes;
        }

        public static bool CompareForeignKeyColumns(ForeignKey fkOriginal, ForeignKey fkNew)
        {
            if (fkOriginal.ForeignKeyColumns.Count != fkNew.ForeignKeyColumns.Count)
                return false;

            for (var i = 0; i < fkOriginal.ForeignKeyColumns.Count; i++)
            {
                var fkColumnOriginal = fkOriginal.ForeignKeyColumns[i];
                var fkColumnNew = fkNew.ForeignKeyColumns[i];

                if (Comparer.ColumnChanged(fkColumnOriginal.ForeignKeyColumn, fkColumnNew.ForeignKeyColumn))
                    return false;

                if (Comparer.ColumnChanged(fkColumnOriginal.ReferredColumn, fkColumnNew.ReferredColumn))
                    return false;
            }

            return true;
        }

        public static List<SqlEngineVersionSpecificPropertyMigration> CompareSqlEngineVersionSpecificPropertyChange(SqlEngineVersionSpecificProperties propertiesOriginal, SqlEngineVersionSpecificProperties propertiesNew)
        {
            var changes = new List<SqlEngineVersionSpecificPropertyMigration>();

            foreach (var propertyOriginal in propertiesOriginal)
            {
                var propertyNew = propertiesNew.FirstOrDefault(p => p.Version == propertyOriginal.Version && p.Name == propertyOriginal.Name);
                if (propertyNew == null)
                {
                    changes.Add(new SqlEngineVersionSpecificPropertyDelete()
                    {
                        SqlEngineVersionSpecificProperty = propertyOriginal
                    });
                }
            }

            foreach (var propertyNew in propertiesNew)
            {
                var propertyOriginal = propertiesOriginal.FirstOrDefault(p => p.Version == propertyNew.Version && p.Name == propertyNew.Name);
                if (propertyOriginal == null)
                {
                    changes.Add(new SqlEngineVersionSpecificPropertyNew()
                    {
                        SqlEngineVersionSpecificProperty = propertyOriginal
                    });
                }
                else if (propertyNew.Value != propertyOriginal.Value)
                {
                    changes.Add(new SqlEngineVersionSpecificPropertyChange()
                    {
                        SqlEngineVersionSpecificProperty = propertyOriginal,
                        NewSqlEngineVersionSpecificProperty = propertyNew
                    });
                }
            }

            return changes;
        }
    }
}

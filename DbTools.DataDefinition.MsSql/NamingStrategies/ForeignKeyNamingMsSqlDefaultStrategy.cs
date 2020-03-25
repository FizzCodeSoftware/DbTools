﻿namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Globalization;
    using System.Linq;

    public class ForeignKeyNamingMsSqlDefaultStrategy : ForeignKeyNamingDefaultStrategy
    {
        public override void SetFKName(ForeignKey fk)
        {
            if (fk.SqlTable.SchemaAndTableName == null || fk.ReferredTable.SchemaAndTableName == null)
                return;

            var fkName = fk.SqlTable.SchemaAndTableName.TableName + "__" + fk.ReferredTable.SchemaAndTableName.TableName;
            if (fkName.Length > 120)
            {
                fkName = fkName.Substring(0, 120);
            }

            var sameNameFks = fk.SqlTable.Properties
                .OfType<ForeignKey>()
                .Where(x => x != fk && x.Name?.StartsWith(fkName, StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList();

            foreach (var sameFk in sameNameFks)
            {
                if (sameFk.Name == fkName)
                    sameFk.Name += "_1";
            }

            fk.Name = sameNameFks.Count > 0
                ? $"{fkName}_{(sameNameFks.Count + 1).ToString("D", CultureInfo.InvariantCulture)}"
                : fkName;
        }
    }
}
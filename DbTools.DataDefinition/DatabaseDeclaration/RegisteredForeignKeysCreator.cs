namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;
    using System.Text;

    internal static class RegisteredForeignKeysCreator
    {
        internal static void UniqueKeySingleColumn(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToTableWithUniqueKeySingleColumn fkRegistration)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);
            var referredUniqueKey = GetReferredUniqueIndex(referredTable);

            // TODO check if Unique index is multi column

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredUniqueKey.SqlColumns[0].SqlColumn;

            var col = new SqlColumn();
            pkColumn.CopyTo(col);

            col.Table = sqlTable;
            col.Types.SetAllNullable(fkRegistration.IsNullable);

            col.Name = fkRegistration.SingleFkColumnName;

            var placeHolderColumn = sqlTable
                .Columns
                .OfType<SqlColumnFKRegistration>()
                .FirstOrDefault(c => c.FKRegistration == fkRegistration);

            var order = sqlTable.Columns.GetOrder(placeHolderColumn.Name);
            sqlTable.Columns.Remove(placeHolderColumn.Name);

            sqlTable.Columns.Add(col.Name, col, order);

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, pkColumn));
        }

        public static void UniqueKey(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToTableWithUniqueKey fkRegistration, IForeignKeyNamingStrategy fkNaming)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);
            var referredUniqueIndex = GetReferredUniqueIndex(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var placeHolderColumn = sqlTable.Columns.OfType<SqlColumnFKRegistration>().FirstOrDefault(c => c.FKRegistration == fkRegistration);
            var order = sqlTable.Columns.GetOrder(placeHolderColumn.Name);
            sqlTable.Columns.Remove(placeHolderColumn.Name);

            foreach (var pkColumn in referredUniqueIndex.SqlColumns.Select(x => x.SqlColumn))
            {
                var col = new SqlColumn();
                pkColumn.CopyTo(col);

                col.Table = sqlTable;

                col.Types.SetAllNullable(fkRegistration.IsNullable);

                col.Name = fkNaming.GetFkToPkColumnName(pkColumn, fkRegistration.NamePrefix);

                sqlTable.Columns.Add(col.Name, col, order++);

                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, pkColumn));
            }
        }

        public static void PrimaryKeyExistingColumn(DatabaseDefinition definiton, SqlTable sqlTable, ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn fkRegistration)
        {
            var referredTable = definiton.GetTable(fkRegistration.ReferredTableName);
            var referredUniqueIndex = GetReferredUniqueIndex(referredTable, fkRegistration.SingleReferredColumnName);

            CheckValidity(fkRegistration, referredUniqueIndex);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredUniqueIndex.SqlColumns[0].SqlColumn;
            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkRegistration.SingleFkColumn, pkColumn));
        }

        private static void CheckValidity(ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn fkRegistration, IndexBase referredUnique)
        {
            if (referredUnique.SqlColumns.Count > 1)
            {
                var messageDescriptionSb = new StringBuilder();
                try
                {
                    messageDescriptionSb.Append("FK: ");
                    messageDescriptionSb.AppendLine(fkRegistration.SingleFkColumn.ToString());
                    messageDescriptionSb.Append("UK: ");
                    foreach (var referredColumn in referredUnique.SqlColumns)
                        messageDescriptionSb.AppendLine(referredColumn.SqlColumn.ToString());
                }
                finally
                {
                }

                throw new InvalidForeignKeyRegistrationException("Single column FK refers to multi column PK or unique index or unique constraint. " + messageDescriptionSb.ToString());
            }
        }

        public static void ReferredTableExistingColumns(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToReferredTableExistingColumns fkRegistration)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            foreach (var fkGroup in fkRegistration.Map)
            {
                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(sqlTable.Columns[fkGroup.ColumnName], referredTable[fkGroup.ReferredColumnName]));
            }
        }

        public static void ReferredTable(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToReferredTable fkRegistration)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var placeHolderColumn = sqlTable.Columns.OfType<SqlColumnFKRegistration>().FirstOrDefault(c => c.FKRegistration == fkRegistration);
            var order = sqlTable.Columns.GetOrder(placeHolderColumn.Name);
            sqlTable.Columns.Remove(placeHolderColumn.Name);

            foreach (var fkGroup in fkRegistration.Map)
            {
                var col = new SqlColumn();
                referredTable.Columns[fkGroup.ReferredColumnName].CopyTo(col);

                col.Table = sqlTable;
                col.Types.SetAllNullable(fkRegistration.IsNullable);

                col.Name = fkGroup.ColumnName;

                sqlTable.Columns.Add(col.Name, col, order++);

                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, referredTable[fkGroup.ReferredColumnName]));
            }
        }

        private static IndexBase GetReferredUniqueIndex(SqlTable referredTable, string referredColumnName)
        {
            if (referredColumnName == null)
                return GetReferredUniqueIndex(referredTable);

            var pkCandidate = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault(pk => pk.SqlColumns.Any(c => c.SqlColumn.Name == referredColumnName) && pk.SqlColumns.Count == 1);
            var uiCandidates = referredTable.Properties.OfType<Index>().Where(i => i.Unique && i.SqlColumns.Any(c => c.SqlColumn.Name == referredColumnName) && i.SqlColumns.Count == 1);
            var ucCandidates = referredTable.Properties.OfType<UniqueConstraint>().Where(uc => uc.SqlColumns.Any(c => c.SqlColumn.Name == referredColumnName) && uc.SqlColumns.Count == 1);

            var count = (pkCandidate != null ? 1 : 0) + uiCandidates.Count() + ucCandidates.Count();

            if (count == 0)
                throw new InvalidForeignKeyRegistrationException("Can't define Foreign Key without finding the target column in primary key, unique index or unique constraint.");

            var uniqueIndex = pkCandidate as IndexBase ?? uiCandidates.FirstOrDefault() as IndexBase ?? ucCandidates.First() as IndexBase;

            /*if (uniqueIndex.SqlColumns.Count > 1)
                throw new InvalidForeignKeyRegistrationException("Can't define Foreign Key with single referencing column referring unique key wit multiple columns.");*/

            return uniqueIndex;
        }

        private static IndexBase GetReferredUniqueIndex(SqlTable referredTable)
        {
            var pkCandidate = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
            var uiCandidates = referredTable.Properties.OfType<Index>().Where(i => i.Unique);
            var ucCandidates = referredTable.Properties.OfType<UniqueConstraint>();

            var count = (pkCandidate != null ? 1 : 0) + uiCandidates.Count() + ucCandidates.Count();

            if (count > 1)
                throw new InvalidForeignKeyRegistrationException("Can't define Foreign Key registration without target column in primary key, unique index or unique constraint.");

            var uniqueIndex = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault()
                ?? referredTable.Properties.OfType<Index>().FirstOrDefault(i => i.Unique) as IndexBase
                ?? referredTable.Properties.OfType<UniqueConstraint>().FirstOrDefault();

            if (uniqueIndex == null)
                throw new InvalidForeignKeyRegistrationException("Can't define Foreign Key registration against a table without primary key, unique index or unique constraint.");

            return uniqueIndex;
        }

        private static ForeignKey ReplaceFKRegistrationWithNewFK(SqlTable sqlTable, ForeignKeyRegistrationBase fkRegistration, SqlTable referredTable)
        {
            sqlTable.Properties.Remove(fkRegistration);
            var fk = new ForeignKey(sqlTable, referredTable, fkRegistration.Name);
            fk.SqlEngineVersionSpecificProperties.AddRange(fkRegistration.SqlEngineVersionSpecificProperties);
            sqlTable.Properties.Add(fk);
            return fk;
        }
    }
}

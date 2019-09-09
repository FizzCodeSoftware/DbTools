using System;
using System.Linq;

namespace FizzCode.DbTools.DataDefinition
{
    internal class RegisteredForeignKeysCreator
    {
        internal static void PrimaryKeySingleColum(Tables tables, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn fkRegistration)
        {
            var referredTable = tables[fkRegistration.ReferredTableName];
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredPk.SqlColumns.First().SqlColumn;

            var col = new SqlColumn();
            pkColumn.CopyTo(col);

            col.Table = sqlTable;
            col.IsNullable = fkRegistration.IsNullable;
            col.Name = fkRegistration.SingleFkColumnName;

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, pkColumn));
        }

        public static void PrimaryKey(Tables tables, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKey fkRegistration, IForeignKeyNamingStrategy fkNaming)
        {
            var referredTable = tables[fkRegistration.ReferredTableName];
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            foreach (var pkColumn in referredPk.SqlColumns.Select(x => x.SqlColumn))
            {
                var col = new SqlColumn();
                pkColumn.CopyTo(col);

                col.Table = sqlTable;
                col.IsNullable = fkRegistration.IsNullable;

                col.Name = fkNaming.GetFkToPkColumnName(pkColumn, fkRegistration.NamePrefix);

                sqlTable.Columns.Add(col.Name, col);

                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, pkColumn));
            }
        }

        public static void PrimaryKeyExistingColumn(Tables tables, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn fkRegistration)
        {
            var referredTable = tables[fkRegistration.ReferredTableName];
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredPk.SqlColumns.First().SqlColumn;
            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkRegistration.SingleFkColumn, pkColumn));
        }

        public static void ReferredTableExistingColumns(Tables tables, SqlTable sqlTable, ForeignKeyRegistrationToReferredTableExistingColumns fkRegistration)
        {
            var referredTable = tables[fkRegistration.ReferredTableName];

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            foreach (var fkGroup in fkRegistration.Map)
            {
                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(sqlTable.Columns[fkGroup.ColumnName], referredTable[fkGroup.ReferredColumnName]));
            }
        }

        public static void ReferredTable(Tables tables, SqlTable sqlTable, ForeignKeyRegistrationToReferredTable fkRegistration)
        {
            var referredTable = tables[fkRegistration.ReferredTableName];

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            foreach (var fkGroup in fkRegistration.Map)
            {
                var col = new SqlColumn();
                referredTable.Columns[fkGroup.ReferredColumnName].CopyTo(col);

                col.Table = sqlTable;
                col.IsNullable = fkRegistration.IsNullable;

                col.Name = fkGroup.ColumnName;

                sqlTable.Columns.Add(col.Name, col);

                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, referredTable[fkGroup.ReferredColumnName]));
            }
        }

        private static PrimaryKey GetReferredPK(SqlTable referredTable)
        {
            var referredPk = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
            if (referredPk == null)
                throw new Exception("Can't define ForeignKeyRegistrationToTableWithPrimaryKey against a table without primary key!");
            return referredPk;
        }

        private static ForeignKey ReplaceFKRegistrationWithNewFK(SqlTable sqlTable, ForeignKeyRegistrationBase fkRegistration, SqlTable referredTable)
        {
            sqlTable.Properties.Remove(fkRegistration);
            var fk = new ForeignKey(sqlTable, referredTable, fkRegistration.Name);
            sqlTable.Properties.Add(fk);
            return fk;
        }
    }
}

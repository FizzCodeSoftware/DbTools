using System;
using System.Collections.Generic;
using System.Linq;
using FizzCode.DbTools.Configuration;

namespace FizzCode.DbTools.DataDefinition
{
    internal static class RegisteredForeignKeysCreator
    {
        internal static void PrimaryKeySingleColum(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn fkRegistration, Dictionary<SqlVersion, TypeMapper> TypeMappers)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredPk.SqlColumns[0].SqlColumn;

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

            CreateOtherTypes(sqlTable, TypeMappers, col);

            sqlTable.Columns.Add(col.Name, col, order);

            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(col, pkColumn));
        }

        private static void CreateOtherTypes(SqlTable sqlTable, Dictionary<SqlVersion, TypeMapper> TypeMappers, SqlColumn col)
        {
            foreach (var typeMapper in TypeMappers)
            {
                var othertype = typeMapper.Value.MapFromGeneric1(col.Types[new Configuration.Generic1()]);
                SqlColumnHelper.Add(typeMapper.Key, sqlTable, col.Name, othertype);
            }
        }

        public static void PrimaryKey(DatabaseDefinition definition, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKey fkRegistration, IForeignKeyNamingStrategy fkNaming)
        {
            var referredTable = definition.GetTable(fkRegistration.ReferredTableName);
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var placeHolderColumn = sqlTable.Columns.OfType<SqlColumnFKRegistration>().FirstOrDefault(c => c.FKRegistration == fkRegistration);
            var order = sqlTable.Columns.GetOrder(placeHolderColumn.Name);
            sqlTable.Columns.Remove(placeHolderColumn.Name);

            foreach (var pkColumn in referredPk.SqlColumns.Select(x => x.SqlColumn))
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

        public static void PrimaryKeyExistingColumn(DatabaseDefinition definiton, SqlTable sqlTable, ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn fkRegistration)
        {
            var referredTable = definiton.GetTable(fkRegistration.ReferredTableName);
            var referredPk = GetReferredPK(referredTable);

            var fk = ReplaceFKRegistrationWithNewFK(sqlTable, fkRegistration, referredTable);

            var pkColumn = referredPk.SqlColumns[0].SqlColumn;
            fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkRegistration.SingleFkColumn, pkColumn));
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

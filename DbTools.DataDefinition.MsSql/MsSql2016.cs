namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using FizzCode.DbTools.DataDefinition;

    public static class MsSql2016
    {
        private static SqlColumn Add(SqlType sqlType)
        {
            var sqlColumn = new SqlColumn
            {
                Table = new SqlTable() // dummy Sql Table
            };
            sqlColumn.Types.Add(MsSqlVersion.MsSql2016, sqlType);

            return sqlColumn;
        }

        public static SqlColumn AddChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Char,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.NChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddVarChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.VarChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNVarChar(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.NVarChar,
                Length = length,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBit(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Bit,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddTinyInt(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.TinyInt,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddSmallInt(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.SmallInt,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddInt(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Int,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBigInt(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.BigInt,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDecimal(int length, int scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Decimal,
                IsNullable = isNullable,
                Length = length,
                Scale = scale
            };

            return Add(sqlType);
        }

        public static SqlColumn AddNumeric(int length, int scale, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Numeric,
                IsNullable = isNullable,
                Length = length,
                Scale = scale
            };

            return Add(sqlType);
        }

        public static SqlColumn AddMoney(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Money,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddSmallMoney(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.SmallMoney,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddFloat(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Float,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddReal(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Real,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDate(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Date,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddTime(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Time,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDateTime(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.DateTime,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDateTime2(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.DateTime2,
                IsNullable = isNullable,
                Length = length
            };

            return Add(sqlType);
        }

        public static SqlColumn AddDateTimeOffset(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.DateTimeOffset,
                IsNullable = isNullable,
                Length = length
            };

            return Add(sqlType);
        }

        public static SqlColumn AddSmallDateTime(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.SmallDateTime,
                IsNullable = isNullable
            };

            return Add(sqlType);
        }

        public static SqlColumn AddBinary(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Binary,
                IsNullable = isNullable,
                Length = length
            };

            return Add(sqlType);
        }

        public static SqlColumn AddVarBinary(int length, bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.VarBinary,
                IsNullable = isNullable,
                Length = length
            };

            return Add(sqlType);
        }

        public static SqlColumn AddImage(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Image,
                IsNullable = isNullable,
            };

            return Add(sqlType);
        }

        public static SqlColumn AddText(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Text,
                IsNullable = isNullable,
            };

            return Add(sqlType);
        }
        public static SqlColumn AddNText(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.NText,
                IsNullable = isNullable,
            };

            return Add(sqlType);
        }

        public static SqlColumn AddUniqueIdentifier(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.UniqueIdentifier,
                IsNullable = isNullable,
            };

            return Add(sqlType);
        }

        public static SqlColumn AddXml(bool isNullable = false)
        {
            var sqlType = new SqlType
            {
                SqlTypeInfo = MsSqlType2016.Xml,
                IsNullable = isNullable,
            };

            return Add(sqlType);
        }

        public static SqlColumn SetForeignKeyTo(string referredTableName, string fkName = null)
        {
            var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

            var singleFkColumn = new SqlColumn
            {
                Table = new SqlTable()
            };

            var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, null, fkName);

            singleFkColumn.Table.Properties.Add(fk);

            return singleFkColumn;
        }

        public static UniqueConstraint AddUniqueConstraint(params string[] columnNames)
        {
            var table = new SqlTable(); // dummy SqlTable
            var uc = new UniqueConstraint(table, null);

            foreach (var columnName in columnNames)
                uc.SqlColumns.Add(new ColumnAndOrderRegistration(columnName, AscDesc.Asc));

            return uc;
        }
    }
}
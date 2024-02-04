using System.Collections.Generic;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Generic1;
public static class Generic1
{
    private static SqlColumn Add(SqlType sqlType)
    {
        var sqlColumn = new SqlColumn
        {
            Table = new SqlTable() // dummy SqlTable
        };
        sqlColumn.Types.Add(GenericVersion.Generic1, sqlType);

        return sqlColumn;
    }

    public static SqlColumn AddChar(int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Char,
            Length = length,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddNChar(int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.NChar,
            Length = length,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddVarChar(int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.VarChar,
            Length = length,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddNVarChar(int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.NVarChar,
            Length = length,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddFloatSmall(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.FloatSmall,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddFloatLarge(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.FloatLarge,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddBit(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Bit,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddByte(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Byte,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddInt16(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Int16,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddInt32(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Int32,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddInt64(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Int64,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddNumber(int? length, int? scale, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Number,
            Length = length,
            Scale = scale,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddDate(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Date,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddTime(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.Time,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn AddDateTime(bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = GenericSqlType1.DateTime,
            IsNullable = isNullable
        };

        return Add(sqlType);
    }

    public static SqlColumn SetForeignKeyTo(string referredTableName, string fkName = null)
    {
        var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);

        var singleFkColumn = new SqlColumn
        {
            Table = new SqlTable() // dummy SqlTable
        };

        var fk = new ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn(singleFkColumn, referredTableNameWithSchema, null, fkName);

        singleFkColumn.Table.Properties.Add(fk);

        return singleFkColumn;
    }

    //public static ForeignKey SetForeignKeyTo(string referredTableName, IEnumerable<SqlEngineVersionSpecificProperty> properties, ColumnReference[] columnReferences)

    public static ForeignKey SetForeignKeyTo(string referredTableName, ColumnReference[] columnReferences)
    {
        var table = new SqlTable(); // dummy SqlTable

        var map = new List<ColumnReference>(columnReferences);
        var referredTableNameWithSchema = new SchemaAndTableName(referredTableName);
        var fk = new ForeignKeyRegistrationToReferredTableExistingColumns(table, referredTableNameWithSchema, null, map);

        table.Properties.Add(fk);

        return fk;
    }

    public static Index AddIndex(params string[] columnNames)
    {
        var table = new SqlTable(); // dummy SqlTable
        var index = new Index(table, null);

        foreach (var columnName in columnNames)
            index.SqlColumns.Add(new ColumnAndOrderRegistration(columnName, AscDesc.Asc));

        return index;
    }

    public static Index AddIndex(bool unique, params string[] columnNames)
    {
        var table = new SqlTable(); // dummy SqlTable
        var index = new Index(table, null, unique);

        foreach (var columnName in columnNames)
            index.SqlColumns.Add(new ColumnAndOrderRegistration(columnName, AscDesc.Asc));

        return index;
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
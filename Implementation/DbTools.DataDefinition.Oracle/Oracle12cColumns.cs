using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public static class Oracle12cColumns
{
    private static SqlColumn Add(SqlTable table, string name, SqlType sqlType)
    {
        return SqlColumnHelper.Add(OracleVersion.Oracle12c, table, name, sqlType);
    }

    public static SqlColumn AddChar(this SqlTable table, string name, int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.Char,
            Length = length,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddNChar(this SqlTable table, string name, int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.NChar,
            Length = length,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddVarChar(this SqlTable table, string name, int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.VarChar,
            Length = length,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddVarChar2(this SqlTable table, string name, int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.VarChar2,
            Length = length,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddNVarChar2(this SqlTable table, string name, int length, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.NVarChar2,
            Length = length,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddBinaryFloat(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.BinaryFloat,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddBinaryDouble(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.BinaryDouble,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddBlob(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.Blob,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddClob(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.Clob,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddNClob(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.NClob,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddBfile(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.BFile,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddLong(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.Long,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddLongRaw(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.LongRaw,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddDate(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.Date,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddTimeStampWithTimeZone(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.TimeStampWithTimeZone,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }

    public static SqlColumn AddTimeStampWithLocalTimeZone(this SqlTable table, string name, bool isNullable = false)
    {
        var sqlType = new SqlType
        {
            SqlTypeInfo = OracleType12c.TimeStampWithLocalTimeZone,
            IsNullable = isNullable
        };

        return Add(table, name, sqlType);
    }
}
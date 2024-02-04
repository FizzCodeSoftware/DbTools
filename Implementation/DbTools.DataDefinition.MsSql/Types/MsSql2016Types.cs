namespace FizzCode.DbTools.DataDefinition.MsSql2016;

public class SqlChar : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlNChar : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlVarChar : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => true;
}

public class SqlNVarChar : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => true;
}

public class SqlText : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool Deprecated => true;
}

public class SqlNText : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool Deprecated => true;
}

public class SqlBit : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlTinyInt : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlSmallInt : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlInt : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlBigInt : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlDecimal : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => true;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlNumeric : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => true;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlMoney : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlSmallMoney : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlFloat : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlReal : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlDate : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlTime : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlDateTime : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlDateTime2 : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlDateTimeOffset : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlSmallDateTime : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlBinary : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlVarBinary : MsSqlType2016
{
    public override bool HasLength => true;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => true;
}

public class SqlImage : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
    public override bool Deprecated => true;
}

public class SqlXml : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlUniqueIdentifier : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}

public class SqlRowVersion : MsSqlType2016
{
    public override bool HasLength => false;
    public override bool HasScale => false;
    public override bool IsMaxLengthAllowed => false;
}
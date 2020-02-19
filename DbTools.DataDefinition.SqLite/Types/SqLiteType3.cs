#pragma warning disable CA1720 // Identifier contains type name

namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    public abstract class SqLiteType3 : SqLiteType
    {
        public override bool HasLength => false;
        public override bool HasScale => false;

        public static SqlText Text { get; } = new SqlText();
        public static SqlReal Real { get; } = new SqlReal();
        public static SqlInteger Integer { get; } = new SqlInteger();
        public static SqlBlob Blob { get; } = new SqlBlob();
    }

    public class SqlText : SqLiteType3
    {
    }

    public class SqlReal : SqLiteType3
    {
    }

    public class SqlInteger : SqLiteType3
    {
    }

    public class SqlBlob : SqLiteType3
    {
    }
}
namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    public abstract class SqLiteType3 : SqLiteType
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public abstract class SqLiteType : SqlTypeInfo
    {
    }


    public class Text : SqLiteType3
    {
    }

    public class Real : SqLiteType3
    {
    }

    public class Integer : SqLiteType3
    {
    }

    public class Blob : SqLiteType3
    {
    }
}
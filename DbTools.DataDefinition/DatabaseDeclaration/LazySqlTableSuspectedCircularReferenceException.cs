namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class LazySqlTableSuspectedCircularReferenceException : InvalidOperationException
    {
        public LazySqlTableSuspectedCircularReferenceException(InvalidOperationException innerException)
            : base(null, innerException)
        { }
    }
}

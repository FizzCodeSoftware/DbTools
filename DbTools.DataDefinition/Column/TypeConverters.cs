namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using FizzCode.DbTools.Configuration;

    public class TypeConverters : Dictionary<SqlVersion, TypeConverter>
    {
        public static TypeConverters All
        {
            get
            {
                var typeConverters = new TypeConverters();
                /*{
                    { new Common.MsSql2016(), new MsSql2016.MsSql2016() },
                    { new Common.Oracle12c(), new Oracle12c.Oracle12c() },
                    { new Common.SqLite3(), new SqLite3.SqLite3() }
                };*/

                return typeConverters;
            }
        }
    }
}
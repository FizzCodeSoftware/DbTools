namespace FizzCode.DbTools.SqlExecuter
{
    using System.Data.Common;
    using System.Linq;

    public static class DbConnectionStringBuilderHelper
    {
        public static object ValueOfKey(this DbConnectionStringBuilder builder, string key)
        {
            var index = builder.Keys.Cast<string>().Select((k, i) => new { Index = i, Key = k })
                .Single(item => item.Key == key)
                .Index;

            var result = builder.Values.Cast<object>().Skip(index).Take(1).First();

            return result;
        }

        public static T ValueOfKey<T>(this DbConnectionStringBuilder builder, string key)
        {
            return (T)ValueOfKey(builder, key);
        }
    }
}
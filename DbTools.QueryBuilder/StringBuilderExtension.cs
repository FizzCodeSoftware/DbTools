namespace FizzCode.DbTools.QueryBuilder
{
    using System.Text;

    public static class StringBuilderExtension
    {
        public static StringBuilder AppendComma(this StringBuilder sb, string value)
        {
            if (!string.IsNullOrEmpty(value))
                sb.Append(", ");

            sb.Append(value);

            return sb;
        }

        public static StringBuilder AppendSpace(this StringBuilder sb, string value)
        {
            if (sb.Length > 0)
                sb.Append(" ");

            sb.Append(value);

            return sb;
        }
    }
}

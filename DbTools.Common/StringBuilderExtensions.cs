namespace FizzCode.DbTools.Common
{
    using System.Text;

    public static class StringBuilderExtensions
    {
        private const int IndentationSpaces = 4;

        public static StringBuilder Append(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * IndentationSpaces))
                .Append(value);
            return sb;
        }

        public static StringBuilder AppendLine(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * IndentationSpaces))
                .AppendLine(value);
            return sb;
        }

        public static string Spaces(int level)
        {
            return new string(' ', level * IndentationSpaces);
        }
    }
}
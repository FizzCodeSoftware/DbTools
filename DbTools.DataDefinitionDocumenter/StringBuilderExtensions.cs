namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Text;

    internal static class StringBuilderExtensions
    {
        public static int indentationSpaces = 4;

        public static StringBuilder Append(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * indentationSpaces))
                .Append(value);
            return sb;
        }

        public static StringBuilder AppendLine(this StringBuilder sb, int level, string value)
        {
            sb.Append(new string(' ', level * indentationSpaces))
                .AppendLine(value);
            return sb;
        }
    }
}
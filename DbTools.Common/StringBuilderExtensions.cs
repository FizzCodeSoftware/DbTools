using System.Text;

namespace FizzCode.DbTools.Common;
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

#pragma warning disable RCS1224 // Make method an extension method.
    public static string Spaces(int level)
#pragma warning restore RCS1224 // Make method an extension method.
    {
        return new string(' ', level * IndentationSpaces);
    }
}
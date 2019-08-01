namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.DataDefinition;

    internal static class ForeignKeyGeneratorHelper
    {
        // todo: why do we have different implementation if we are using static helpers?
        internal static string FKConstraint(ForeignKey fk, Func<string, string> guard)
        {
            var sb = new StringBuilder();
            sb.Append("CONSTRAINT ")
                .Append(fk.Name)
                .Append(" FOREIGN KEY ")
                .Append("(")
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(fkc => $"{guard(fkc.ForeignKeyColumn.Name)}")))
                .Append(")")
                .Append(" REFERENCES ")
                .Append(guard(fk.PrimaryKey.SqlTable.Name))
                .Append("(")
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(pkc => $"{guard(pkc.PrimaryKeyColumn.Name)}")))
                .Append(")");

            return sb.ToString();
        }
    }
}

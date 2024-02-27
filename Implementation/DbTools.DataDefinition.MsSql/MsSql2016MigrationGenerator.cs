using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.DataDefinition.SqlGenerator;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class MsSql2016MigrationGenerator(ContextWithLogger context)
    : AbstractSqlMigrationGenerator(context, new MsSql2016Generator(context))
{
    public override string GenerateColumnChange(SqlColumn columnOriginal, SqlColumn columnNew)
    {
        var typeOld = columnOriginal.Types[MsSqlVersion.MsSql2016];
        var typeNew = columnNew.Types[MsSqlVersion.MsSql2016];
        
        var sb = new StringBuilder();

        var defaultValueOld = columnOriginal.Properties.OfType<DefaultValue>().FirstOrDefault();
        var defaultValueNew = columnNew.Properties.OfType<DefaultValue>().FirstOrDefault();
        var isDefaultValueChange = defaultValueOld != defaultValueNew;

        if (Comparer.ColumnChanged(columnNew, columnOriginal))
        {
            sb.Append("ALTER COLUMN ");

            sb.Append(Generator.GuardKeywords(columnNew.Name!))
                .Append(' ');

            sb.Append(Generator.GenerateType(typeNew));
        }

        // TODO remove identity?
        var identity = columnNew.Properties.OfType<Identity>().FirstOrDefault();
        if (identity != null)
        {
           ((MsSql2016Generator)Generator).GenerateCreateColumnIdentity(sb, identity);
        }

        if (isDefaultValueChange)
        {
            if (!(defaultValueOld is not null && defaultValueNew is null))
            {
                sb.Append(((MsSql2016Generator)Generator).GenerateDefault(columnNew));

                if (typeNew.IsNullable)
                    sb.Append(" NULL");
                else
                    sb.Append(" NOT NULL");
            }
            else
            {
                sb.Append(" DROP CONSTRAINT ");
                sb.Append(Throw.IfNull(defaultValueOld.Name));
            }
        }

        return sb.ToString();
    }

    protected virtual void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
    {
        sb.Append(" IDENTITY(")
            .Append(identity.Seed)
            .Append(',')
            .Append(identity.Increment)
            .Append(')');
    }
}
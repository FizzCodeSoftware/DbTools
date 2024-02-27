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
    public override string GenerateColumnChange(ColumnChange columnChange)
    {
        var typeOld = columnChange.SqlColumn.Types[MsSqlVersion.MsSql2016];
        var typeNew = columnChange.SqlColumnChanged.Types[MsSqlVersion.MsSql2016];

        var sb = new StringBuilder();

        var isDefaultValueChange = columnChange.SqlColumnPropertyMigrations.OfType<DefaultValueMigration>().Any();
        var isIdentityChange = columnChange.SqlColumnPropertyMigrations.OfType<IdentityMigration>().Any();

        if (Comparer.ColumnChanged(columnChange.SqlColumnChanged, columnChange.SqlColumn)
            || isIdentityChange)
        {
            sb.Append("ALTER COLUMN ");

            sb.Append(Generator.GuardKeywords(columnChange.SqlColumnChanged.Name!))
                .Append(' ');

            if (!isIdentityChange)
                sb.Append(Generator.GenerateType(typeNew));
        }

        if (isIdentityChange)
        {
            // var identityOld = columnChange.SqlColumn.Properties.OfType<Identity>().FirstOrDefault();
            var identityNew = columnChange.SqlColumnChanged.Properties.OfType<Identity>().FirstOrDefault();

            if (identityNew is not null)
                ((MsSql2016Generator)Generator).GenerateCreateColumnIdentity(sb, identityNew);

            if (identityNew is null)
            {
                // TODO drop and recreate
                throw new System.NotImplementedException("Cannot remove identity constraint.");
            }
        }

        if (isDefaultValueChange)
        {
            var defaultValueOld = columnChange.SqlColumn.Properties.OfType<DefaultValue>().FirstOrDefault();
            var defaultValueNew = columnChange.SqlColumnChanged.Properties.OfType<DefaultValue>().FirstOrDefault();

            if (!(defaultValueOld is not null && defaultValueNew is null))
            {
                sb.Append(((MsSql2016Generator)Generator).GenerateDefault(columnChange.SqlColumnChanged));

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
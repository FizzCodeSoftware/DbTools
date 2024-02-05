using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Migration;
using FizzCode.DbTools.DataDefinition.SqlGenerator;

namespace FizzCode.DbTools.DataDefinition.Oracle12c;
public class Oracle12cMigrationGenerator(ContextWithLogger context)
    : AbstractSqlMigrationGenerator(context, new Oracle12cGenerator(context))
{
    public override string CreateColumns(params ColumnNew[] columnNews)
    {
        var tableNames = columnNews.Select(c => c.Table.SchemaAndTableName).Distinct();

        if (tableNames.Count() != 1)
            throw new ArgumentOutOfRangeException(nameof(columnNews), "All columns should be on the same table.");

        var tableName = tableNames.First();

        var columnsToAdd = columnNews.Select(c => c.Name).ToList();

        var sb = new StringBuilder();
        sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));
        sb.Append("ADD (");

        var idx = 0;
        foreach (var columnNew in columnNews)
        {
            if (idx++ > 0)
                sb.AppendLine(",");

            sb.AppendLine(Generator.GenerateCreateColumn(columnNew.SqlColumn));
        }

        sb.Append(')');

        return sb.ToString();
    }

    public override string DropColumns(params ColumnDelete[] columnDeletes)
    {
        var tableNames = columnDeletes.Select(c => c.Table.SchemaAndTableName).Distinct();

        if (tableNames.Count() != 1)
            throw new ArgumentOutOfRangeException(nameof(columnDeletes), "All columns should be on the same table.");

        var tableName = tableNames.First();

        var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.Name)).ToList();

        var sb = new StringBuilder();
        sb.Append("ALTER TABLE ").AppendLine(Generator.GetSimplifiedSchemaAndTableName(tableName));

        if (columnsToDelete.Count > 1)
        {
            sb.Append("DROP (");
            var idx = 0;
            foreach (var column in columnsToDelete)
            {
                if (idx++ > 0)
                    sb.Append(", ");

                sb.Append(column);
            }

            sb.Append(')');
        }
        else
        {
            sb.Append("DROP COLUMN ");
            sb.Append(columnsToDelete[0]);
        }

        return sb.ToString();
    }

    public override SqlStatementWithParameters ChangeColumns(params ColumnChange[] columnChanges)
    {
        var tableName = CheckSameTable(columnChanges);

        if (columnChanges.Length == 1)
        {
            return $@"
ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(tableName)}
MODIFY {GenerateColumnChange(columnChanges[0].SqlColumn, columnChanges[0].NewNameAndType)}";
        }

        var sbStatements = new StringBuilder();
        // TODO Options ShouldMigrateColumnChangesAllAtOnce
        // TODO multiple -> temp table
        // TODO drop constraints then re add them
        foreach (var columnChange in columnChanges)
        {
            sbStatements.AppendLine(ChangeColumns(columnChange).Statement);
        }

        return new SqlStatementWithParameters(sbStatements.ToString());
    }

    public string GenerateColumnChange(SqlColumn columnOriginal, SqlColumn columnNew)
    {
        var typeOld = columnOriginal.Types[OracleVersion.Oracle12c];
        var typeNew = columnNew.Types[OracleVersion.Oracle12c];

        var sb = new StringBuilder();
        sb.Append(Generator.GuardKeywords(columnOriginal.Name));

        if ((typeOld.SqlTypeInfo.HasLength && typeOld.Length != typeNew.Length)
            || (typeOld.SqlTypeInfo.HasScale && typeOld.Scale != typeNew.Scale))
        {
            sb.Append(' ')
            .Append(typeNew.SqlTypeInfo.SqlDataType);

            if (typeNew.Scale.HasValue)
            {
                if (typeNew.Length != null)
                {
                    sb.Append('(')
                        .Append(typeNew.Length?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(", ")
                        .Append(typeNew.Scale?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(')');
                }
                else
                {
                    sb.Append('(')
                        .Append(typeNew.Scale?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(')');
                }
            }
            else if (typeNew.Length.HasValue)
            {
                sb.Append('(')
                    .Append(typeNew.Length?.ToString("D", CultureInfo.InvariantCulture))
                    .Append(')');
            }
        }

        // TODO not possible to remove identity in Oracle and MS SQL
        /*var identityOld = columnNew.Properties.OfType<Identity>().FirstOrDefault();
        var identityNew = columnNew.Properties.OfType<Identity>().FirstOrDefault();

        if (identityNew != identityOld && identityNew != null)
        {
            GenerateCreateColumnIdentity(sb, identityNew);
        }*/

        var defaultValueOld = columnOriginal.Properties.OfType<DefaultValue>().FirstOrDefault();
        var defaultValueNew = columnNew.Properties.OfType<DefaultValue>().FirstOrDefault();
        if (defaultValueOld != defaultValueNew)
        {
            sb.Append(" DEFAULT(")
                .Append(defaultValueNew.Value)
                .Append(')');
        }

        if (typeOld.IsNullable != typeNew.IsNullable)
        {
            if (typeNew.IsNullable)
                sb.Append(" NULL");
            else
                sb.Append(" NOT NULL");
        }

        sb.Append(';');

        return sb.ToString();
    }
    /*
    public override string CreatePrimaryKey(PrimaryKeyNew primaryKeyNew)
    {
        ALTER TABLE buses ADD CONSTRAINT PK_BUSES PRIMARY KEY(Bus_no);
        var pkColumnsList = primaryKeyNew.PrimaryKey.SqlColumns.Select(c => c.SqlColumn.Name).ToList();
        var pkColumns = string.Join(", ", pkColumnsList);
        // TODO Properties (PKs, FKs, Indexes, Defaults, Descriptions)
        return $"ALTER TABLE {Generator.GetSimplifiedSchemaAndTableName(primaryKeyNew.PrimaryKey.SqlTable.SchemaAndTableName)} ADD CONSTRAINT PRIMARY KEY ({pkColumns})";

    }*/
}
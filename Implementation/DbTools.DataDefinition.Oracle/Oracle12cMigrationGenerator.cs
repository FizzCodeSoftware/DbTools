using System;
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

        var tableName = tableNames.First()!;

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

        var tableName = tableNames.First()!;

        var columnsToDelete = columnDeletes.Select(c => Generator.GuardKeywords(c.Name!)).ToList();

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

    public override string GenerateColumnChange(ColumnChange columnChange)
    {
        var typeOld = columnChange.SqlColumn.Types[OracleVersion.Oracle12c];
        var typeNew = columnChange.SqlColumnChanged.Types[OracleVersion.Oracle12c];

        var sb = new StringBuilder();
        sb.Append("MODIFY ");
        sb.Append(Generator.GuardKeywords(columnChange.SqlColumn.Name!));

        var defaultValueOld = columnChange.SqlColumn.Properties.OfType<DefaultValue>().FirstOrDefault();
        var defaultValueNew = columnChange.SqlColumnChanged.Properties.OfType<DefaultValue>().FirstOrDefault();
        var isDefaultValueChange = defaultValueOld != defaultValueNew;

        if (Comparer.ColumnChanged(columnChange.SqlColumn, columnChange.SqlColumnChanged)
            || isDefaultValueChange)
        {
            Generator.GenerateType(typeNew);
        }

        // TODO not possible to remove identity in Oracle and MS SQL

        var identity = columnChange.SqlColumnChanged.Properties.OfType<Identity>().FirstOrDefault();
        if (identity != null)
        {
            ((Oracle12cGenerator)Generator).GenerateCreateColumnIdentity(sb, identity);
        }

        if (isDefaultValueChange)
        {
            if (!(defaultValueOld is not null && defaultValueNew is null))
            {
                Throw.InvalidOperationExceptionIfNull(defaultValueNew);
                // TODO clenup default change, add tests
                sb.Append(" DEFAULT(")
                    .Append(defaultValueNew.Value)
                    .Append(')');
            }
            else
            {
                sb.Append(" DEFAULT NULL");
            }
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
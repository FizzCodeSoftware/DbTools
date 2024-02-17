using System;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinitionDocumenter;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class MsSql2016CSharpTypedWriter(GeneratorContext context, Type typeMapperType, string databaseName)
    : AbstractCSharpTypedWriter(context, MsSqlVersion.MsSql2016, typeMapperType, databaseName)
{
    protected override string GetColumnCreationMethod(SqlColumn column)
    {
        var type = column.Types[Version];
        return type.SqlTypeInfo switch
        {
            SqlChar _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlNChar _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddNChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlVarChar _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddVarChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlNVarChar _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddNVarChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlNText _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddNText)}(",
            SqlFloat _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddFloat)}(",
            SqlReal _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddReal)}(",
            SqlBit _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddBit)}(",
            SqlSmallInt _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddSmallInt)}(",
            SqlTinyInt _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddTinyInt)}(",
            SqlInt _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddInt)}(",
            SqlBigInt _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddBigInt)}(",
            SqlDecimal _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddDecimal)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlNumeric _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddNumeric)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlMoney _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddMoney)}(",
            SqlSmallMoney _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddSmallMoney)}(",
            SqlDate _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddDate)}(",
            SqlTime _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddTime)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlDateTime _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddDateTime)}(",
            SqlDateTime2 _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddDateTime2)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlDateTimeOffset _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddDateTimeOffset)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlSmallDateTime _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddSmallDateTime)}(",
            SqlBinary _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddBinary)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlVarBinary _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddVarBinary)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlImage _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddImage)}(",
            SqlXml _ => $"{nameof(MsSql2016)}.{nameof(MsSql2016.AddXml)}(",
            SqlUniqueIdentifier _ => $"{nameof(MsSql2016)}.AddUniqueIdentifier(",
            _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
        };
    }

    protected override void AddForeignKeySettingsSingleColumn(StringBuilder sb, DocumenterHelper helper, ForeignKey fkOnColumn)
    {
        var tableName = helper.GetSimplifiedSchemaAndTableName(fkOnColumn.ReferredTable, DatabaseDeclarationConst.SchemaTableNameSeparator.ToString(CultureInfo.InvariantCulture));

        if (fkOnColumn.SqlEngineVersionSpecificProperties.Count() == 1
            && fkOnColumn.SqlEngineVersionSpecificProperties.First().Name == "Nocheck"
            && fkOnColumn.SqlEngineVersionSpecificProperties.First().Value == "true")
        {
            sb.Append(".SetForeignKeyToColumnNoCheck(nameof(")
                .Append(DatabaseName)
                .Append('.')
                .Append(tableName)
                .Append("), nameof(")
                .Append(tableName)
                .Append("Table.")
                .Append(fkOnColumn.ForeignKeyColumns[0].ReferredColumn.Name)
                .Append("))");
        }
        else
        {
            base.AddForeignKeySettingsSingleColumn(sb, helper, fkOnColumn);
        }
    }
}
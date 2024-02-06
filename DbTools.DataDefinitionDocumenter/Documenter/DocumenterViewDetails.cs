using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using System.Collections.Generic;
using System.Linq;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public partial class Documenter
{
    private void AddViews(List<KeyValuePair<string, SqlView>> _sqlViewsByCategory, List<KeyValuePair<string, SqlView>> _skippedSqlViewsByCategory, bool hasCategories)
    {
        foreach (var viewKvp in _sqlViewsByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableName?.Schema).ThenBy(t => t.Value!.SchemaAndTableNameSafe.TableName))
        {
            Context.Logger.Log(LogSeverity.Verbose, "Generating {ViewName}.", "Documenter", viewKvp.Value.SchemaAndTableName ?? "");
            var category = viewKvp.Key;
            var view = viewKvp.Value;
            AddViewToViewList(category, view, hasCategories);

            Throw.InvalidOperationExceptionIfNull(view.SchemaAndTableName);

            var sheetColor = GetColor(view.SchemaAndTableName);
            if (sheetColor != null)
                DocumenterWriter.SetSheetColor(Helper.GetSimplifiedSchemaAndTableName(view.SchemaAndTableName), sheetColor.Value);

            AddViewHeader(hasCategories, category, view);

            AddViewDetails(category, view, hasCategories);
        }

        WriteLine("Views");

        foreach (var tableKvp in _skippedSqlViewsByCategory.OrderBy(kvp => kvp.Key).ThenBy(t => t.Value.SchemaAndTableNameSafe.Schema).ThenBy(t => t.Value.SchemaAndTableNameSafe.TableName))
        {
            var category = tableKvp.Key;
            var view = tableKvp.Value;
            AddViewToViewList(category, view, hasCategories);
        }
    }

    protected void AddViewDetails(string category, SqlView view, bool hasCategories)
    {
        foreach (var column in view.Columns)
        {
            AddColumnsToViewSheet(column);
        }
    }
}

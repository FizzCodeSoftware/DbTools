using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public partial class Documenter
{
    private void WriteViewsAndAllColumnsHeaderLine(bool hasCategories)
    {
        var tablesColumns = new List<string>();
        if (hasCategories)
            tablesColumns.Add("Category");
        tablesColumns.AddRange(new [] { "Schema", "View Name", "Link", "Number of columns", "Description" });

        WriteLine("Views", tablesColumns.ToArray());

        var allColumns = new List<string>();
        if (hasCategories)
            allColumns.Add("Category");

        allColumns.AddRange(new[] {"Schema", "View Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls" });

        if (Context.DocumenterSettings.NoInternalDataTypes)
            allColumns.Remove("Data Type (DbTools)");
    }
}

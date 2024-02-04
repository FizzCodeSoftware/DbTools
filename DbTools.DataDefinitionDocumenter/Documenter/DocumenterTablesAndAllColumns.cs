namespace FizzCode.DbTools.DataDefinitionDocumenter;

using System.Collections.Generic;

public partial class Documenter
{
    private void WriteTablesAndAllColumnsHeaderLine(bool hasCategories)
    {
        var tablesColumns = new List<string>();
        if (hasCategories)
            tablesColumns.Add("Category");
        tablesColumns.AddRange(new[] { "Schema", "Table Name", "Link", "Number of columns", "Description" });

        WriteLine("Tables", tablesColumns.ToArray());

        var allColumns = new List<string>();
        if (hasCategories)
            allColumns.Add("Category");

        allColumns.AddRange(new[] {"Schema", "Table Name", "Column Name", "Data Type (DbTools)", "Data Type", "Column Length", "Column Scale", "Allow Nulls", "Primary Key", "Identity", "Default Value", "Description" });

        if (Context.DocumenterSettings.NoInternalDataTypes)
            allColumns.Remove("Data Type (DbTools)");

        WriteLine("All columns", allColumns.ToArray());
    }
}

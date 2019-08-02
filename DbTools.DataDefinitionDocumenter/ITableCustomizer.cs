namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public interface ITableCustomizer
    {
        bool ShouldSkip(string tableName);
        string Category(string tableName);
        string BackGroundColor(string tableName);
    }
}

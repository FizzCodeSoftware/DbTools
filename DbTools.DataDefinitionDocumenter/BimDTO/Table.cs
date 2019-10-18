namespace FizzCode.DbTools.DataDefinitionDocumenter.BimDTO
{
    using System.Collections.Generic;

    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; } = new List<Column>();
        public List <Partition> Partitions { get; } = new List<Partition>();
    }

    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string SourceColumn { get; set; }
    }

    public class Partition
    {
        public string Name { get; set; }
        public string DataView { get; set; }
        public PartitionSource Source { get; set; }
    }

    public class PartitionSource
    {
        public string Type { get; set; }
        public List<string> Expression { get; } = new List<string>();

    }
}

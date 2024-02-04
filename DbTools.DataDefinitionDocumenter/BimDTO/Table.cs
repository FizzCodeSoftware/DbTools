using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;
public class Table
{
    public string Name { get; set; }
    public List<Column> Columns { get; } = new List<Column>();
    public List<Partition> Partitions { get; } = new List<Partition>();
}

public class Column
{
    public string Name { get; set; }
    public string DataType { get; set; }
    // TODO needs on PK?
    public bool IsKey { get; set; }
    public string SourceColumn { get; set; }
    // TODO leave out if null
    public string FormatString { get; set; }
    // TODO leave out if null
    public List<Annotation> Annotations { get; } = new List<Annotation>();
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

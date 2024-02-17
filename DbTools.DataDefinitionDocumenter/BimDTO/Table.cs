using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;
public class Table
{
    public required string Name { get; init; }
    public List<Column> Columns { get; } = [];
    public List<Partition> Partitions { get; } = [];
}

public class Column
{
    public required string Name { get; init; }
    public required string DataType { get; init; }
    // TODO needs on PK?
    public bool IsKey { get; set; }
    public required string SourceColumn { get; init; }
    // TODO leave out if null
    public string? FormatString { get; set; }
    // TODO leave out if null
    public List<Annotation> Annotations { get; } = [];
}

public class Partition
{
    public required string Name { get; init; }
    public required string DataView { get; init; }
    public required PartitionSource Source { get; init; }
}

public class PartitionSource
{
    public required string Type { get; init; }
    public List<string> Expression { get; } = [];
}

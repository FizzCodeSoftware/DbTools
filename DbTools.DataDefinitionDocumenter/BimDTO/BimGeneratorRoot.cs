using System.Collections.Generic;
using System.Globalization;

namespace FizzCode.DbTools.DataDefinitionDocumenter.BimDTO;
public class BimGeneratorRoot
{
    public string Name { get; set; } = "SemanticModel";
    public int CompatibilityLevel { get; set; } = 1400;
    public BimGeneratorModel Model { get; set; } = new BimGeneratorModel();
    public string Id { get; set; } = "SemanticModel";
}

public class BimGeneratorModel
{
    public string Culture { get; set; } = "hu-HU";
    public List<DataSource> DataSources { get; } = [];
    public List<Table> Tables { get; } = [];
    public List<Relationship> Relationships { get; } = [];
    public List<Annotation> Annotations { get; } = [];
}

public class DataSource
{
    public string Type { get; set; } = "structured";
    public required string Name { get; init; }
    public required ConnectionDetails ConnectionDetails { get; init; }
    public required Credential Credential { get; init; }
}

public class ConnectionDetails
{
    public string Protocol { get; set; } = "tds";
    public required Address Address { get; init; }
    public string? Authentication { get; set; }
    public string? Query { get; set; }
}

public class Address
{
    public string Server { get; set; } = "localhost";
    public required string Database { get; init; }
}

public class Credential
{
    public string AuthenticationKind { get; set; } = "UsernamePassword";
    public string Kind { get; set; } = "SQL";
    public required string Path { get; init; }
    public required string Username { get; init; }
    public bool EncryptConnection { get; set; }
}

public class Annotation
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}

public class Relationship
{
    public required string Name { get; init; }
    public required string FromTable { get; init; }
    public required string FromColumn { get; init; }
    public required string ToTable { get; init; }
    public required string ToColumn { get; init; }
    public bool IsActive { get; set; }

    public override string ToString()
    {
        return FromTable + "." + FromColumn + " -> " + ToTable + "." + ToColumn + " (" + IsActive.ToString(CultureInfo.InvariantCulture) + ")";
    }
}
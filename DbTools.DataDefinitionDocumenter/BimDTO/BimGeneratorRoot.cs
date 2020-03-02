namespace FizzCode.DbTools.DataDefinitionDocumenter.BimDTO
{
    using System.Collections.Generic;
    using System.Globalization;

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
        public List<DataSource> DataSources { get; } = new List<DataSource>();
        public List<Table> Tables { get; } = new List<Table>();
        public List<Relationship> Relationships { get; } = new List<Relationship>();
        public List<Annotation> Annotations { get; } = new List<Annotation>();
    }

    public class DataSource
    {
        public string Type { get; set; } = "structured";
        public string Name { get; set; }
        public ConnectionDetails ConnectionDetails { get; set; }
        public Credential Credential { get; set; }
    }

    public class ConnectionDetails
    {
        public string Protocol { get; set; } = "tds";
        public Address Address { get; set; }
        public string Authentication { get; set; }
        public string Query { get; set; }
    }

    public class Address
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; }
    }

    public class Credential
    {
        public string AuthenticationKind { get; set; } = "UsernamePassword";
        public string Kind { get; set; } = "SQL";
        public string Path { get; set; }
        public string Username { get; set; }
        public bool EncryptConnection { get; set; }
    }

    public class Annotation
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Relationship
    {
        public string Name { get; set; }
        public string FromTable { get; set; }
        public string FromColumn { get; set; }
        public string ToTable { get; set; }
        public string ToColumn { get; set; }
        public bool IsActive { get; set; }

        public override string ToString()
        {
            return FromTable + "." + FromColumn + " -> " + ToTable + "." + ToColumn + " (" + IsActive.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }
}
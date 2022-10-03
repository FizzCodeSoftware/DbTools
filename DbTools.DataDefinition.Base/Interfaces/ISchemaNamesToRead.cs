namespace FizzCode.DbTools.DataDefinition.Base.Interfaces
{
    using System.Collections.Generic;

    public interface ISchemaNamesToRead
    {
        bool All { get; set; }
        bool AllDefault { get; set; }
        bool AllNotSystem { get; set; }
        List<string> SchemaNames { get; set; }
    }
}
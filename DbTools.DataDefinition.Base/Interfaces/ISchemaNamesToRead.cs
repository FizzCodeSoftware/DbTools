using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition.Base.Interfaces;
public interface ISchemaNamesToRead
{
    bool All { get; set; }
    bool AllDefault { get; set; }
    bool AllNotSystem { get; set; }
    List<string> SchemaNames { get; set; }
}
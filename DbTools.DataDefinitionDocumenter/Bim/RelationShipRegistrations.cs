namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System.Collections.Generic;
    using System.Linq;

    public class RelationShipRegistrations
    {
        private readonly Dictionary<string, Dictionary<string, BimRelationship>> _fromTo = new Dictionary<string, Dictionary<string, BimRelationship>>();

        public Dictionary<string, BimRelationship> GetByFromTable(string fromTableName)
        {
            return _fromTo[fromTableName];
        }

        public List<string> FromTables()
        {
            return _fromTo.Keys.ToList();
        }

        public void Add(BimRelationship relationShipRegistration)
        {
            if (!_fromTo.ContainsKey(relationShipRegistration.FromTableName))
                _fromTo.Add(relationShipRegistration.FromTableName, new Dictionary<string, BimRelationship>());

            _fromTo[relationShipRegistration.FromTableName].Add(relationShipRegistration.ToKey, relationShipRegistration);
        }
    }
}

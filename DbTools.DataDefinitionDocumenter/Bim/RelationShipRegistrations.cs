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
            if (!_fromTo.ContainsKey(relationShipRegistration.FromTableSchemaAndTableName))
                _fromTo.Add(relationShipRegistration.FromTableSchemaAndTableName, new Dictionary<string, BimRelationship>());

            _fromTo[relationShipRegistration.FromTableSchemaAndTableName].Add(relationShipRegistration.ToKey, relationShipRegistration);
        }

        public bool Contains(BimRelationship bimRelationship)
        {
            if (!_fromTo.Keys.Contains(bimRelationship.FromTableSchemaAndTableName.ToString()))
                return false;

            var tos = _fromTo[bimRelationship.FromTableSchemaAndTableName];

            if(!tos.Keys.Contains(bimRelationship.ToKey))
                return false;

            return true;
        }
    }
}

using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyToPrimaryKey : ForeignKey
    {
        public bool IsNullable { get; set; }
        public string NamePrefix { get; set; }
        public List<ForeignKeyGroup> Map { get; set; }

        public ForeignKeyToPrimaryKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string name, List<ForeignKeyGroup> map = null)
            : base(table, referredTableName, name)
        {
            IsNullable = isNullable;
            NamePrefix = namePrefix;
            Map = map;
        }
    }
}

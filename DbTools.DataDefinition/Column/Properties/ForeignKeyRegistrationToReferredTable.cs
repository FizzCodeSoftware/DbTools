using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn : ForeignKeyBase
    {
        public string SingleFkColumnName { get; set; }
        public bool IsNullable { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn(SqlTable table, SchemaAndTableName referredTableName, string singleFkColumnName, bool isNullable, string fkName) : base(table, referredTableName, fkName)
        {
            SingleFkColumnName = singleFkColumnName;
            IsNullable = isNullable;
        }
    }

    public class ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn : ForeignKeyBase
    {
        public SqlColumn SingleFkColumn { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn(SqlColumn singleFkColumn, SchemaAndTableName referredTableName, string fkName) : base(singleFkColumn.Table, referredTableName, fkName)
        {
            SingleFkColumn = singleFkColumn;
        }
    }

    public class ForeignKeyRegistrationToTableWithPrimaryKey : ForeignKeyBase
    {
        public bool IsNullable { get; set; }
        public string NamePrefix { get; set; }

        public ForeignKeyRegistrationToTableWithPrimaryKey(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string fkName) : base(table, referredTableName, fkName)
        {
            IsNullable = isNullable;
            NamePrefix = namePrefix;
        }
    }

    public class ForeignKeyRegistrationToReferredTable : ForeignKeyBase
    {
        public bool IsNullable { get; set; }
        public List<ForeignKeyGroup> Map { get; set; }

        public ForeignKeyRegistrationToReferredTable(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string fkName, List<ForeignKeyGroup> map) : base(table, referredTableName, fkName)
        {
            IsNullable = isNullable;
            Map = map;
        }
    }

    public class ForeignKeyRegistrationToReferredTableExistingColumns : ForeignKeyBase
    {
        public List<ForeignKeyGroup> Map { get; set; }

        public ForeignKeyRegistrationToReferredTableExistingColumns(SqlTable table, SchemaAndTableName referredTableName, string fkName, List<ForeignKeyGroup> map) : base(table, referredTableName, fkName)
        {
            Map = map;
        }
    }

    public class ForeignKeyRegistrationToReferredTableX : ForeignKey
    {
        public bool IsNullable { get; set; }
        public string NamePrefix { get; set; }

        public List<ForeignKeyGroup> Map { get; set; }

        public ForeignKeyRegistrationToReferredTableX(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string namePrefix, string fkName, List<ForeignKeyGroup> map)
            : base(table, referredTableName, fkName)
        {
            IsNullable = isNullable;
            NamePrefix = namePrefix;
            Map = map;
        }
    }
}

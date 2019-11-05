namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SqlColumn
    {
        public SqlTable Table { get; set; }
        public string Name { get; set; }
        public SqlType Type { get; set; }
        public bool IsNullable { get; set; }
        public int? Length { get; set; }
        public int? Precision { get; set; }

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ?? (_properties = new List<SqlColumnProperty>());

        public override string ToString()
        {
            return $"{Name} {Enum.GetName(typeof(SqlType), Type)} on {Table.SchemaAndTableName}";
        }

        public T CopyTo<T>(T column) where T : SqlColumn
        {
            column.Name = Name;
            column.Type = Type;
            column.Table = Table;
            column.IsNullable = IsNullable;
            column.Length = Length;
            column.Precision = Precision;
            return column;
        }

        public SqlColumn SetPK()
        {
            Table.SetPK(this);
            return this;
        }

        public SqlColumn SetIdentity()
        {
            Properties.Add(new Identity(this));
            return this;
        }

        public bool HasProperty<T>()
            where T : SqlColumnProperty
        {
            return Properties.Any(x => x is T);
        }
    }
}
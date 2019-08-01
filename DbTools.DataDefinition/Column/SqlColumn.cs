namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;

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
            return $"{Name} {Enum.GetName(typeof(SqlType), Type)} on {Table.Name}";
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
    }
}
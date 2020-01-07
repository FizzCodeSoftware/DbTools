namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class Tables : ICollection<SqlTable>
    {
        private SortedList<int, SqlTable> _sorted = new SortedList<int, SqlTable>();
        private readonly Dictionary<string, SqlTable> byName = new Dictionary<string, SqlTable>();

        public int Count => byName.Count;

        public bool IsReadOnly => false;

        public void Add(SqlTable item)
        {
            byName.Add(item.SchemaAndTableName.SchemaAndName, item);
        }

        public void Clear()
        {
            byName.Clear();
            _sorted.Clear();
        }

        public bool Contains(SqlTable item)
        {
            return byName.ContainsValue(item);
        }

        public bool ContainsKey(SchemaAndTableName schemaAndTableName)
        {
            return byName.ContainsKey(schemaAndTableName);
        }

        public void CopyTo(SqlTable[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<SqlTable> GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public bool Remove(SqlTable item)
        {
            _sorted.RemoveAt(_sorted.IndexOfValue(item));
            return byName.Remove(item.SchemaAndTableName.SchemaAndName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public SqlTable this[string schemaAndTableName] => byName[schemaAndTableName];

        private void EnsureSorted()
        {
            if (_sorted.Count != byName.Count)
            {
                _sorted = TableSorter.GetSortedTables(byName.Values.ToList());
            }
        }

        public List<SqlTable> ToList()
        {
            EnsureSorted();
            return _sorted.Values.ToList();
        }
    }
}

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

        public void Add(SqlTable table)
        {
            byName.Add(table.Name, table);
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

        public void CopyTo(SqlTable[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<SqlTable> GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public bool Remove(SqlTable table)
        {
            _sorted.RemoveAt(_sorted.IndexOfValue(table));
            return byName.Remove(table.Name);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public SqlTable this[string name]
        {
            get
            {
                return byName[name];
            }
        }

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

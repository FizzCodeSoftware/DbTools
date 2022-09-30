namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class AbstractTables<T> : ICollection<T> where T: SqlTableOrView
    {
        protected SortedList<int, T> _sorted = new();
        protected readonly Dictionary<string, T> byName = new();

        public int Count => byName.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            byName.Add(item.SchemaAndTableName.SchemaAndName, item);
        }

        public void Clear()
        {
            byName.Clear();
            _sorted.Clear();
        }

        public bool Contains(T item)
        {
            return byName.ContainsValue(item);
        }

        public bool ContainsKey(SchemaAndTableName schemaAndTableName)
        {
            return byName.ContainsKey(schemaAndTableName);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public bool Remove(T item)
        {
            _sorted.RemoveAt(_sorted.IndexOfValue(item));
            return byName.Remove(item.SchemaAndTableName.SchemaAndName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            EnsureSorted();
            return _sorted.Values.GetEnumerator();
        }

        public T this[string schemaAndTableName] => byName[schemaAndTableName];

        protected abstract void EnsureSorted();

        public List<T> ToList()
        {
            EnsureSorted();
            return _sorted.Values.ToList();
        }
    }
}

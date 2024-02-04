using System.Collections;

namespace FizzCode.DbTools.DataDefinition.Base;
public abstract class OrderedDictionaryWithInsert<TKey, TValue> : ICollection<TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();
    private readonly SortedDictionary<int, TKey> _order = new();
    private int _maxOrder;

    public abstract void Add(TValue item);

    public void Add(TKey key, TValue value)
    {
        Add(key, value, ++_maxOrder);
    }

    private readonly object syncRoot = new();

    public void Add(TKey key, TValue value, int order)
    {
        _dictionary.Add(key, value);

        lock (syncRoot)
        {
            var newOrder = order;
            if (_order.ContainsKey(order))
            {
                newOrder = order;
                var lostToIncrementKeys = _order.Keys.Where(k => k >= newOrder).OrderByDescending(k => k).ToList();
                foreach (var higherOrderKey in lostToIncrementKeys)
                {
                    var dictionaryKey = _order[higherOrderKey];
                    _order.Remove(higherOrderKey);
                    _order.Add(higherOrderKey + 1, dictionaryKey);
                }
            }

            _order.Add(newOrder, key);
        }
    }

    /*public IEnumerable<TValue> Values
    {
        get
        {
            return this;
        }
    }*/

    public int Count => _dictionary.Count;

    public bool IsReadOnly => false;

    public void Clear()
    {
        _order.Clear();
        _dictionary.Clear();
    }

    public IEnumerator<TValue> GetEnumerator()
    {
        foreach (var kvp in _order)
        {
            yield return _dictionary[kvp.Value];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Contains(TValue item)
    {
        return _dictionary.ContainsValue(item);
    }

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        foreach (var kvp in _order)
        {
            array[arrayIndex++] = _dictionary[kvp.Value];
        }
    }

    public bool Remove(TValue item)
    {
        throw new NotImplementedException();
    }

    public bool Remove(TKey key)
    {
        if (_dictionary.ContainsKey(key))
        {
            lock (syncRoot)
            {
                _dictionary.Remove(key);
                _order.Remove(GetOrder(key));
            }

            return true;
        }

        return false;
    }

    public TValue this[TKey key] => _dictionary[key];

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    public int GetOrder(TKey key)
    {
        var order = _order.FirstOrDefault(i => i.Value.Equals(key)).Key;
        return order;
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
}

using System.Collections;
using System.Collections.Generic;

namespace GraphExt
{
    public class BiDictionary<T1, T2> : IDictionary<T1, T2>
    {
        private readonly IDictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly IDictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public bool ContainsKey(T1 key)
        {
            return _forward.ContainsKey(key);
        }

        public bool ContainsValue(T2 value)
        {
            return _reverse.ContainsKey(value);
        }

        public bool Remove(T1 key)
        {
            return Remove(key, _forward, _reverse);
        }

        public bool RemoveValue(T2 value)
        {
            return Remove(value, _reverse, _forward);
        }

        private bool Remove<TKey, TValue>(TKey key, IDictionary<TKey, TValue> forward, IDictionary<TValue, TKey> reverse)
        {
            var hasValue = forward.TryGetValue(key, out var value);
            if (hasValue)
            {
                forward.Remove(key);
                reverse.Remove(value);
            }
            return hasValue;
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return _forward.TryGetValue(key, out value);
        }

        public bool TryGetKey(T2 value, out T1 key)
        {
            return _reverse.TryGetValue(value, out key);
        }

        public T2 this[T1 key]
        {
            get => _forward[key];
            set => _forward[key] = value;
        }

        public T1 GetKey(T2 value) => _reverse[value];
        public void SetKey(T2 value, T1 key) => _reverse[value] = key;

        public ICollection<T1> Keys => _forward.Keys;
        public ICollection<T2> Values => _forward.Values;

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<T1, T2> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item)
        {
            return _forward.Contains(item);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            _forward.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            _reverse.Remove(new KeyValuePair<T2, T1>(item.Value, item.Key));
            return _forward.Remove(item);
        }

        public int Count => _forward.Count;
        public bool IsReadOnly => _forward.IsReadOnly;
    }
}
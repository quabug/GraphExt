using System.Collections;
using System.Collections.Generic;

namespace GraphExt
{
    public interface IReadOnlyBiDictionary<T1, T2> : IReadOnlyDictionary<T1, T2>
    {
        IReadOnlyDictionary<T1, T2> Forward { get; }
        IReadOnlyDictionary<T2, T1> Reverse { get; }
        bool ContainsValue(T2 value);
        bool TryGetKey(T2 value, out T1 key);
        T1 GetKey(T2 value);
    }

    public interface IBiDictionary<T1, T2> : IDictionary<T1, T2>
    {
        bool TryGetKey(T2 value, out T1 key);
        T1 GetKey(T2 value);
        bool ContainsValue(T2 value);
        bool RemoveValue(T2 value);
        void SetKey(T2 value, T1 key);
    }

    public class BiDictionary<T1, T2> : IBiDictionary<T1, T2>, IReadOnlyBiDictionary<T1, T2>
    {
        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public IReadOnlyDictionary<T1, T2> Forward => _forward;
        public IReadOnlyDictionary<T2, T1> Reverse => _reverse;

        public BiDictionary() {}

        public BiDictionary(IEnumerable<(T1, T2)> items)
        {
            foreach (var (key, value) in items) Add(key, value);
        }

        public BiDictionary(IEnumerable<KeyValuePair<T1, T2>> items)
        {
            foreach (var item in items) Add(item.Key, item.Value);
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Set(T1 t1, T2 t2)
        {
            if (_forward.TryGetValue(t1, out var value2)) _reverse.Remove(value2);
            if (_reverse.TryGetValue(t2, out var value1)) _forward.Remove(value1);
            _forward[t1] = t2;
            _reverse[t2] = t1;
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
            set => Set(key, value);
        }

        IEnumerable<T1> IReadOnlyDictionary<T1, T2>.Keys => _forward.Keys;
        IEnumerable<T2> IReadOnlyDictionary<T1, T2>.Values => _forward.Values;

        public ICollection<T1> Keys => _forward.Keys;
        public ICollection<T2> Values => _forward.Values;

        public T1 GetKey(T2 value) => _reverse[value];
        public void SetKey(T2 value, T1 key) => _reverse[value] = key;

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
            return ((IDictionary<T1, T2>)_forward).Contains(item);
        }

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex)
        {
            ((IDictionary<T1, T2>)_forward).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T1, T2> item)
        {
            _reverse.Remove(item.Value);
            return _forward.Remove(item.Key);
        }

        public int Count => _forward.Count;
        public bool IsReadOnly => ((IDictionary<T1, T2>)_forward).IsReadOnly;
    }
}
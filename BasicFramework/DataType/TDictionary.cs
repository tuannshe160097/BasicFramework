using BasicFramework.DataType.Exception;
using BasicFramework.DataType.Interface;
using BasicFramework.JsonConverter;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace BasicFramework.DataType
{
    public class TDictionary : IHierarchicalLinkedData, IDictionary<string, object?>
    {
        private Dictionary<string, object?> _data = new();

        public object? this[string key]
        {
            get
            {
                object? result;

                _data.TryGetValue(key, out result);

                return result;
            }
            set
            {
                ThrowIfReadOnly();

                (value as IHierarchicalLinkedData)?.SetParent(this);

                _data[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        public ICollection<string> Keys => _data.Keys;

        public ICollection<object?> Values => _data.Values;

        public bool IsReadOnly { get; private set; } = false;

        public IHierarchicalLinkedData? Parent { get; private set; } = null;

        public dynamic Children => _data.ToArray();

        public TDictionary()
        {

        }

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly) throw new ReadOnlyException();
        }

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() 
        { 
            return GetEnumerator(); 
        }

        public void Add(KeyValuePair<string, object?> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            ThrowIfReadOnly();

            foreach (var item in _data.Values)
            {
                (item as IHierarchicalLinkedData)?.UnsetParent();
            }

            _data.Clear();
        }

        public bool Contains(KeyValuePair<string, object?> item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object?> item)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, object? value)
        {
            ThrowIfReadOnly();

            (value as IHierarchicalLinkedData)?.SetParent(this);

            _data.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            ThrowIfReadOnly();

            var temp = _data[key];

            (temp as IHierarchicalLinkedData)?.UnsetParent();

            return _data.Remove(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
        {
            return _data.TryGetValue(key, out value);
        }

        public void Lock() 
        {
            IsReadOnly = true;

            foreach (var item in _data.Values)
            {
                (item as IHierarchicalLinkedData)?.Lock();
            }
        }

        void IHierarchicalLinkedData.SetParent(IHierarchicalLinkedData parent)
        {
            Parent = parent;
        }

        void IHierarchicalLinkedData.UnsetParent()
        {
            Parent = null;
        }
    }
}

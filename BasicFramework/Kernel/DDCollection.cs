using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Reflection;

namespace BasicFramework.Kernel 
{
    public class DDCollection : DynamicObject, IDictionary<string, object?>
    {
        private Dictionary<string, object?> _data = new();

        public object? this[string key]
        {
            get
            {
                return GetMember(key);
            }
            set
            {
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

        public bool IsReadOnly => false;

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            result = GetMember(binder.Name);

            return true;
        }

        private object? GetMember(string key)
        {
            object? result;

            if (!_data.TryGetValue(key, out result))
            {
                result = null;
            }

            return result;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            _data[binder.Name] = value;

            return true;
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
            _data.Add(item.Key, item.Value);
        }

        public void Clear()
        {
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
            return _data.Remove(item.Key);
        }

        public void Add(string key, object? value)
        {
            _data.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _data.Remove(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value)
        {
            return _data.TryGetValue(key, out value);
        }
    }
}

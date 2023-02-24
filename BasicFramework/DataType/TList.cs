using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicFramework.DataType.Exception;
using BasicFramework.DataType.Interface;
using Newtonsoft.Json.Linq;

namespace BasicFramework.DataType
{
    public class TList : IHierarchicalLinkedData, IList<object?>
    {
        private List<object?> _data = new();

        public object? this[string key]
        {
            get
            {
                return null;
            }
            set
            {
                throw new StringKeyException();
            }
        }

        public object? this[int index] 
        { 
            get 
            {
                object? result = _data.ElementAtOrDefault(index);

                return result;
            } 
            set 
            {
                ThrowIfReadOnly();

                (value as IHierarchicalLinkedData)?.SetParent(this);

                _data[index] = value;
            }
        }

        public bool IsReadOnly { get; private set; } = false;

        public int Count => _data.Count;

        public IHierarchicalLinkedData? Parent { get; private set; } = null;

        public dynamic Children => _data.ToArray();

        public TList()
        {

        }

        private void ThrowIfReadOnly()
        {
            if (IsReadOnly) throw new ReadOnlyException();
        }

        public void Add(object? item)
        {
            ThrowIfReadOnly();

            (item as IHierarchicalLinkedData)?.SetParent(this);

            _data.Add(item);
        }

        public void Clear()
        {
            ThrowIfReadOnly();

            foreach (var item in _data)
            {
                (item as IHierarchicalLinkedData)?.UnsetParent();
            }

            _data.Clear();
        }

        public bool Contains(object? item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(object?[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public int IndexOf(object? item)
        {
            return _data.IndexOf(item);
        }

        public void Insert(int index, object? item)
        {
            ThrowIfReadOnly();

            (item as IHierarchicalLinkedData)?.SetParent(this);

            _data.Insert(index, item);
        }

        public bool Remove(object? item)
        {
            ThrowIfReadOnly();

            int index = IndexOf(item);

            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            ThrowIfReadOnly();

            (_data[index] as IHierarchicalLinkedData)?.UnsetParent();

            _data.RemoveAt(index);
        }

        public IEnumerator<object?> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Lock()
        {
            IsReadOnly = true;

            foreach (var item in _data)
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

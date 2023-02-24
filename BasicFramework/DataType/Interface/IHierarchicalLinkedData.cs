using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicFramework.DataType.Interface
{
    public interface IHierarchicalLinkedData
    {
        public IHierarchicalLinkedData? Parent { get; }
        public dynamic Children { get; }
        public bool IsReadOnly { get; }
        public void Lock();

        internal void SetParent(IHierarchicalLinkedData parent);
        internal void UnsetParent();
    }
}

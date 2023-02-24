using BasicFramework.JsonConverter;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Xml.Linq;

namespace BasicFramework.DataType
{
    [JsonConverter(typeof(TObjectConverter))]
    public class TObject : DynamicObject
    {
        private TDictionary _dict = new();
        private TList _list = new();

        private bool _isList = false;

        public int Count => _isList ? _list.Count : _dict.Count;

        public object? this[object key]
        {
            get 
            {
                return Get(key);
            }
            set 
            { 
                Set(key, value);
            }
        }

        public TObject(bool isList = false)
        {
            _isList = isList;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            try
            {
                result = Get(binder.Name);

                return true;
            }
            catch 
            {
                result = null;

                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            try
            {
               Set(binder.Name, value);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            Type type = _isList ? typeof(TList) : typeof(TDictionary);
            object target = _isList ? _list : _dict;

            try
            {
                result = type.InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, target, args);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private object? Get(object key)
        {
            if (_isList)
            {
                return _list[(int)key];
            }
            else
            {
                return _dict[(string)key];
            }
        }

        private void Set(object key, object? value)
        {
            if (_isList)
            {
                _list[(int)key] = value;
            }
            else
            {
                _dict[(string)key] = value;
            }
        }
    }
}

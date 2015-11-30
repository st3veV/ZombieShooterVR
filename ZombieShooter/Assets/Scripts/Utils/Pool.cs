using System;
using System.Collections.Generic;

namespace Utils
{
    public class Pool<T>
    {
        private readonly List<T> _objects;
        private readonly Func<T> _createFunction;

        public Pool()
        {
            _objects = new List<T>();
        }

        public Pool(Func<T> createFunc)
        {
            _createFunction = createFunc;
            _objects = new List<T>();
        }

        public bool HasItems()
        {
            return _objects.Count > 0;
        }

        public T Get(out bool isNew)
        {
            T clone = default(T);
            while (ObjUtil.IsNull(clone) && _objects.Count > 0)
            {
                clone = _objects[_objects.Count - 1];
                _objects.Remove(clone);
            }
            isNew = false;
            if (clone == null && _createFunction != null)
            {
                isNew = true;
                clone = _createFunction();
            }
            return clone;
        }

        public T Get()
        {
            bool isNew;
            return Get(out isNew);
        }

        public void Add(T obj)
        {
            _objects.Add(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }
    }
}
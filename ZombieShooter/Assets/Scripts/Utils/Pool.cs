using System;
using System.Collections.Generic;

namespace Utils
{
    public class Pool<T>
    {
        private readonly List<T> _objects;

        public Pool()
        {
            _objects = new List<T>();
        }
        
        public virtual bool HasItems()
        {
            return _objects.Count > 0;
        }
        
        public virtual T Get()
        {
            T clone = default(T);
            while (clone == null && _objects.Count > 0)
            {
                clone = _objects[_objects.Count - 1];
                _objects.Remove(clone);
            }
            return clone;
        }

        public virtual void Add(T obj)
        {
            _objects.Add(obj);
        }

        public virtual void Clear()
        {
            _objects.Clear();
        }
    }
}
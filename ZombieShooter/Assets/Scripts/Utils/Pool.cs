using System.Collections.Generic;

public class Pool<T>
{
    private readonly List<T> _objects;

    public Pool()
    {
        _objects = new List<T>();
    }

    public bool HasItems()
    {
        return _objects.Count > 0;
    }

    public T Get()
    {
        T clone = default(T);
        while (clone == null && _objects.Count > 0)
        {
            clone = _objects[_objects.Count - 1];
            _objects.Remove(clone);
        }
        return clone;
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

using UnityEngine;

namespace Utils
{
    public class AutoObjectWrapperPool<T> : Pool<T> where T : AutoObjectWrapper<T>
    {

        private readonly GameObject _objectToClone;
        private readonly GameObject _container;

        public AutoObjectWrapperPool(GameObject objectToClone, GameObject container)
        {
            _objectToClone = objectToClone;
            _container = container;
        }
        
        private T CreateGameObject()
        {
            GameObject clone = GameObject.Instantiate(_objectToClone);
            clone.transform.SetParent(_container.transform);
            T wrapper = AutoObjectWrapper<T>.Create(clone);
            return wrapper;
        }

        public T Get(out bool isNew)
        {
            T clone = base.Get();
            while (clone == null && HasItems())
            {
                clone = base.Get();
            }
            isNew = false;
            if (clone == null)
            {
                isNew = true;
                clone = CreateGameObject();
            }
            return clone;
        }

        public override T Get()
        {
            bool isNew;
            return Get(out isNew);
        }

        public override void Clear()
        {
            while (HasItems())
            {
                GameObject.Destroy(base.Get());
            }
            base.Clear();
        }
    }
}
using System;
using UnityEngine;

namespace Utils
{
    public class GameObjectPool:Pool<GameObject>
    {
        private readonly GameObject _objectToClone;
        private readonly GameObject _container;

        public GameObjectPool(GameObject objectToClone, GameObject container)
        {
            _objectToClone = objectToClone;
            _container = container;
        }

        private GameObject CreateGameObject()
        {
            GameObject clone = GameObject.Instantiate(_objectToClone);
            clone.transform.SetParent(_container.transform);
            return clone;
        }

        public GameObject Get(out bool isNew)
        {
            GameObject clone = base.Get();
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

        public override GameObject Get()
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
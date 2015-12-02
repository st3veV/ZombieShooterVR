using UnityEngine;

namespace Utils
{
    public class AutoObjectWrapper<T> : AutoObject<T> where T : AutoObjectWrapper<T>
    {
        protected GameObject OriginalPrefab;

        public static T Create(GameObject originalPrefab)
        {
            var component = originalPrefab.AddComponent<T>();
            component.SetPrefab(originalPrefab);
            return component;
        }

        protected virtual void SetPrefab(GameObject originalPrefab)
        {
            OriginalPrefab = originalPrefab;
        }
    }

}
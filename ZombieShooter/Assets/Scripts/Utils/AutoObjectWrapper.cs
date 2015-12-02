using System;
using System.Collections.Generic;
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
        
        protected void ExecuteListeners(List<Action<T>> listeners)
        {
            if (listeners.Count > 0)
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    Action<T> listener = listeners[i];
                    listener((T) this);
                }
            }
        }
    }

}
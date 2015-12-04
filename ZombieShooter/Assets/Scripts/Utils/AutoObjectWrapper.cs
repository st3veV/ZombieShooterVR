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

        private readonly List<List<Action<T>>> _listenersInExecution = new List<List<Action<T>>>();
        private readonly Dictionary<List<Action<T>>,List<Action<T>>> _listenersToRemove = new Dictionary<List<Action<T>>, List<Action<T>>>();

        protected void ExecuteListeners(List<Action<T>> listeners)
        {
            _listenersInExecution.Add(listeners);
            if (listeners.Count > 0)
            {
                var nullListeners = new List<int>();
                for (var i = 0; i < listeners.Count; i++)
                {
                    Action<T> listener = listeners[i];
                    if (listener != null)
                    {
                        listener((T) this);
                    }
                    else
                    {
                        nullListeners.Add(i);
                    }
                }
                if (nullListeners.Count > 0)
                {
                    for (var i = nullListeners.Count - 1; i >= 0; i--)
                    {
                        listeners.RemoveAt(nullListeners[i]);
                    }
                }
                if (_listenersToRemove.ContainsKey(listeners))
                {
                    for (var i = 0; i < _listenersToRemove[listeners].Count; i++)
                    {
                        var listener = _listenersToRemove[listeners][i];
                        listeners.Remove(listener);
                    }
                    _listenersToRemove.Remove(listeners);
                }
            }
            _listenersInExecution.Remove(listeners);
        }

        protected void RemoveListener(Action<T> listener, List<Action<T>> fromListeners)
        {
            if (_listenersInExecution.Contains(fromListeners))
            {
                if (!_listenersToRemove.ContainsKey(fromListeners))
                {
                    _listenersToRemove.Add(fromListeners, new List<Action<T>>());
                }
                _listenersToRemove[fromListeners].Add(listener);
            }
            else
            {
                fromListeners.Remove(listener);
            }
        }
    }

}
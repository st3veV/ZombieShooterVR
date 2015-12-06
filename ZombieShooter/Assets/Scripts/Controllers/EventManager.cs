using System;
using System.Collections.Generic;

namespace Controllers
{
    public class EventManager:Singleton<EventManager>
    {
        private readonly List<Action> _updateListeners = new List<Action>();
        private readonly List<int> _updateListenersToRemove = new List<int>();

        public int NumListeners = 0;

        void Update()
        {
            for (var i = 0; i < _updateListeners.Count; i++)
            {
                var listener = _updateListeners[i];
                if (listener != null)
                {
                    listener();
                }
                else
                {
                    _updateListenersToRemove.Add(i);
                }
            }
            if (_updateListenersToRemove.Count > 0)
            {
                for (var i = _updateListenersToRemove.Count - 1; i >= 0; i--)
                {
                    _updateListeners.RemoveAt(_updateListenersToRemove[i]);
                }
                _updateListenersToRemove.Clear();
                NumListeners = _updateListeners.Count;
            }
        }

        public void AddUpdateListener(Action listener)
        {
            if (!_updateListeners.Contains(listener))
            {
                _updateListeners.Add(listener);
                NumListeners = _updateListeners.Count;
            }
        }

        public void RemoveUpdateListener(Action listener)
        {
            if (_updateListeners.Contains(listener))
            {
                _updateListeners[_updateListeners.IndexOf(listener)] = null;
            }
        }
    }
}
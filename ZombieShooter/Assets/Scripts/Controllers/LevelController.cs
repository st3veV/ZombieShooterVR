using System;
using UnityEngine;

namespace Controllers
{
    public class LevelController : Singleton<LevelController>
    {
        public event Action OnSceneLoaded;

        private enum LoadingState
        {
            Idle,
            Loading,
            Loaded
        }

        private LoadingState _state = LoadingState.Idle;

        private void OnUpdate()
        {
            if (_state == LoadingState.Loaded)
            {
                OnOnSceneLoaded();
                EventManager.Instance.RemoveUpdateListener(OnUpdate);
                _state = LoadingState.Idle;
            }
        }

        public void LoadScene(Scene scene)
        {
            _state = LoadingState.Loading;
            EventManager.Instance.AddUpdateListener(OnUpdate);
            Application.LoadLevel((int) scene);
        }

        private void OnLevelWasLoaded()
        {
            _state = LoadingState.Loaded;
        }

        #region Event invocators

        protected void OnOnSceneLoaded()
        {
            var handler = OnSceneLoaded;
            if (handler != null) handler();
        }

        #endregion
    }
}

public enum Scene
{
    Setup = 0,
    Tutorial = 1,
    Game = 2,
    GameOver = 3
}

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

        private void Update()
        {
            if (_state == LoadingState.Loaded)
            {
                OnOnSceneLoaded();
                _state = LoadingState.Idle;
            }
        }

        public void LoadScene(Scene scene)
        {
            _state = LoadingState.Loading;
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

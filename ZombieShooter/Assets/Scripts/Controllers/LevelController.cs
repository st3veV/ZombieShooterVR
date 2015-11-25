using System;
using UnityEngine;

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
    
    void Update()
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
        Application.LoadLevel((int)scene);
    }

    void OnLevelWasLoaded()
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

public enum Scene
{
    Setup = 0,
    Init = 1,//loaded automatically and continues to Tutorial
    Tutorial = 2,
    Game = 3,
    GameOver = 4
}

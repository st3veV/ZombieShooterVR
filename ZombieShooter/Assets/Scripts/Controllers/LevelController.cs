using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public event Action OnSceneLoaded;

    public Scene SceneToLoad { get; set; }

    private bool loaded = false;

    public void Start()
    {
        Scene scene = SceneToLoad;
        Application.LoadLevel((int) scene);
    }

    void Update()
    {
        if(loaded)
            OnOnSceneLoaded();
    }

    void OnLevelWasLoaded()
    {
        loaded = true;
    }

    protected void OnOnSceneLoaded()
    {
        var handler = OnSceneLoaded;
        if (handler != null) handler();
    }
}

public enum Scene
{
    Setup = 0,
    Init = 1,//loaded automatically and continues to Tutorial
    Tutorial = 2,
    Game = 3,
    GameOver = 4
}


public class TutorialSceneLoader : LevelController
{
    public TutorialSceneLoader()
    {
        SceneToLoad = Scene.Tutorial;
    }
}

public class GameSceneLoader : LevelController
{
    public GameSceneLoader()
    {
        SceneToLoad = Scene.Game;
    }
}

public class GameOverSceneLoader : LevelController
{
    public GameOverSceneLoader()
    {
        SceneToLoad = Scene.GameOver;
    }
}

public class InitSceneLoader : LevelController
{
    public InitSceneLoader()
    {
        SceneToLoad = Scene.Init;
    }
}
using System;
using System.Collections;
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
    Tutorial = 1,
    Game = 2,
    GameOver = 3
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
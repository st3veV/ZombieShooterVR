using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public event Action OnSceneLoaded;

    private AsyncOperation loadAsync;

    public void LoadScene(Scene scene)
    {
        //Application.LoadLevelAdditive((int) scene);
        loadAsync = Application.LoadLevelAdditiveAsync((int) scene);
    }

    public void UnloadScene()
    {
        GameObject container = GameObject.Find("Container");
        Destroy(container);
    }

    void Update()
    {
        if (loadAsync != null)
        {
            if(loadAsync.isDone)
            {
                Debug.Log("Scene loaded");
                OnOnSceneLoaded();
                loadAsync = null;
            }
            else
            {
                Debug.Log("Loading scene: " + loadAsync.progress);
            }
        }
    }

    protected virtual void OnOnSceneLoaded()
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
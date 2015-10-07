using System;
using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public event Action OnSceneLoaded;

    private AsyncOperation loadAsync;
    public Scene SceneToLoad { get; set; }

    public IEnumerator Start()
    {
        Scene scene = SceneToLoad;
        loadAsync = Application.LoadLevelAsync((int) scene);
        yield return loadAsync;

        OnOnSceneLoaded();
    }














    public void LoadScene(Scene scene)
    {
        //Application.LoadLevelAdditive((int) scene);
        //loadAsync = Application.LoadLevelAdditiveAsync((int) scene);
        loadAsync = Application.LoadLevelAsync((int) scene);
        

        Debug.Log("Level loaded");
        //Application.LoadLevel((int) scene);
        //loadAsync.allowSceneActivation = true;
    }

    public void UnloadScene()
    {
        GameObject container = GameObject.Find("Container");
        Destroy(container);
    }

    void Update()
    {
        /*
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
         * */
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
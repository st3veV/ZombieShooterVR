using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public LifetimeComponent UserLifetime;
    private TutorialController TutorialController;
    private GameController GameController;

    public List<GameObject> DontDestroyGameObjects;

    public GameObject loader;

	void Start ()
	{
	    SetDontDestroys();


        UserLifetime.OnDie += UserLifetime_OnDie;
        UserLifetime.OnDamage += UserLifetime_OnDamage;

	    StartTutorial();
	}

    private void SetDontDestroys()
    {
        foreach (GameObject dontDestroyGameObject in DontDestroyGameObjects)
        {
            DontDestroyOnLoad(dontDestroyGameObject);
        }
    }

    private void StartTutorial()
    {
        Debug.Log("Starting tutorial");
        loader = new GameObject("loader");
        DontDestroyOnLoad(loader);
        TutorialSceneLoader tutorialSceneLoader = loader.AddComponent<TutorialSceneLoader>();
        tutorialSceneLoader.OnSceneLoaded += OnTutorialSceneLoaded;
    }

    void OnTutorialSceneLoaded()
    {
        Debug.Log("Tutorial scene loaded");
        Destroy(loader);
        GameObject tutController = GameObject.Find("TutorialController");
        TutorialController = tutController.GetComponent<TutorialController>();
        TutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
    }

    void TutorialController_OnTutorialComplete()
    {
        Debug.Log("Tutorial complete, loading");
        loader = new GameObject("loader");
        DontDestroyOnLoad(loader);
        GameSceneLoader gameSceneLoader = loader.AddComponent<GameSceneLoader>();
        gameSceneLoader.OnSceneLoaded += gameSceneLoader_OnSceneLoaded;
    }

    void gameSceneLoader_OnSceneLoaded()
    {
        Destroy(loader);
        GameObject gController = GameObject.Find("GameController");
        GameController = gController.GetComponent<GameController>();
    }


    void UserLifetime_OnDamage(float damage)
    {
    }

    void UserLifetime_OnDie(LifetimeComponent lifetimeComponent)
    {
        //LevelController.LoadScene(Scene.GameOver);
    }
	
}

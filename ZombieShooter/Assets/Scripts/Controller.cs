using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public LifetimeComponent UserLifetime;
    private TutorialController TutorialController;

    public List<GameObject> DontDestroyGameObjects;

    private GameObject loader;

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
        loader = new GameObject("loader");
        TutorialSceneLoader tutorialSceneLoader = loader.AddComponent<TutorialSceneLoader>();
        tutorialSceneLoader.OnSceneLoaded += OnTutorialSceneLoaded;
    }

    void OnTutorialSceneLoaded()
    {
        Destroy(loader);
        GameObject tutController = GameObject.Find("TutorialController");
        TutorialController = tutController.GetComponent<TutorialController>();
        TutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
    }

    void TutorialController_OnTutorialComplete()
    {
        loader = new GameObject("loader");
        GameSceneLoader gameSceneLoader = loader.AddComponent<GameSceneLoader>();
    }


    void UserLifetime_OnDamage(float damage)
    {
    }

    void UserLifetime_OnDie(LifetimeComponent lifetimeComponent)
    {
        //LevelController.LoadScene(Scene.GameOver);
    }
	
}

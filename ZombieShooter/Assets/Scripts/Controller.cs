using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public LifetimeComponent UserLifetime;
    public LevelController LevelController;
    private TutorialController TutorialController;

	void Start () {
        UserLifetime.OnDie += UserLifetime_OnDie;
        UserLifetime.OnDamage += UserLifetime_OnDamage;

	    StartTutorial();
	}

    private void StartTutorial()
    {
        LevelController.OnSceneLoaded += OnTutorialSceneLoaded;
        LevelController.LoadScene(Scene.Tutorial);
    }

    void OnTutorialSceneLoaded()
    {
        LevelController.OnSceneLoaded -= OnTutorialSceneLoaded;
        GameObject tutController = GameObject.Find("TutorialController");
        TutorialController = tutController.GetComponent<TutorialController>();
        TutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
    }

    void TutorialController_OnTutorialComplete()
    {
        LevelController.UnloadScene();
        LevelController.LoadScene(Scene.Game);
    }


    void UserLifetime_OnDamage(float damage)
    {
    }

    void UserLifetime_OnDie(LifetimeComponent lifetimeComponent)
    {
        LevelController.LoadScene(Scene.GameOver);
    }
	
}

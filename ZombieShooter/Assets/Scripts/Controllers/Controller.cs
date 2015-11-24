using System.Collections.Generic;
using Thalmic.Myo;
using UnityEngine;

public class Controller : MonoBehaviour {

    private TutorialController TutorialController;
    private GameController GameController;
    private GameOverController GameOverController;

    public List<GameObject> DontDestroyGameObjects;

    public GameObject loader;

    public UserController UserController;

	void Start ()
	{
	    SetDontDestroys();
        
        StartTutorial();
        //StartGame();
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
        tutorialSceneLoader.OnSceneLoaded += tutorialSceneLoader_OnSceneLoaded;
    }

    void tutorialSceneLoader_OnSceneLoaded()
    {
        Debug.Log("Tutorial scene loaded");
        Destroy(loader);
        GameObject tutController = GameObject.Find("TutorialController");
        TutorialController = tutController.GetComponent<TutorialController>();
        TutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
        ResetPlayer();
    }

    void TutorialController_OnTutorialComplete()
    {
        Debug.Log("Tutorial complete, loading");
        TutorialController.OnTutorialComplete -= TutorialController_OnTutorialComplete;
        TutorialController = null;
        StartGame();
    }

    private void StartGame()
    {
        Debug.Log("Starting game");
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
        GameController.OnGameEnded += GameController_OnGameEnded;
        ResetPlayer();
    }

    void GameController_OnGameEnded()
    {
        GameController.OnGameEnded -= GameController_OnGameEnded;
        GameController = null;
        GameOver();
    }

    private void GameOver()
    {
        Debug.Log("Game over");
        loader = new GameObject("loader");
        DontDestroyOnLoad(loader);
        GameOverSceneLoader gameOverSceneLoader = loader.AddComponent<GameOverSceneLoader>();
        gameOverSceneLoader.OnSceneLoaded += gameOverSceneLoader_OnSceneLoaded;
    }

    void gameOverSceneLoader_OnSceneLoaded()
    {
        Destroy(loader);
        GameObject goController = GameObject.Find("GameOverController");
        GameOverController = goController.GetComponent<GameOverController>();
        GameOverController.OnPlayAgain += GameOverController_OnPlayAgain;
        GameOverController.OnGoToTutorial += GameOverController_OnGoToTutorial;
        ResetPlayer();
    }

    void GameOverController_OnGoToTutorial()
    {
        RemoveGameOverListeners();
        StartTutorial();
    }

    void GameOverController_OnPlayAgain()
    {
        RemoveGameOverListeners();
        StartGame();
    }

    private void ResetPlayer()
    {
        UserController.Reset();
    }

    void RemoveGameOverListeners()
    {
        GameOverController.OnPlayAgain -= GameOverController_OnPlayAgain;
        GameOverController.OnGoToTutorial -= GameOverController_OnGoToTutorial;
        GameOverController = null;
    }
}

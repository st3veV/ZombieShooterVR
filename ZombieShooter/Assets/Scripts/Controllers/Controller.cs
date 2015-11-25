using System.Collections.Generic;
using Thalmic.Myo;
using UnityEngine;

public class Controller : Singleton<Controller>
{
    private TutorialController TutorialController;
    private GameController GameController;
    private GameOverController GameOverController;

    public List<GameObject> DontDestroyGameObjects;
    
    private LevelController levelController;

    public UserController UserController;

    public Controller()
    {
        instance = this;
    }

    void Start ()
	{
	    SetDontDestroys();

        levelController = LevelController.Instance;

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
        levelController.OnSceneLoaded += tutorialSceneLoader_OnSceneLoaded;
        levelController.LoadScene(Scene.Tutorial);
    }

    void tutorialSceneLoader_OnSceneLoaded()
    {
        Debug.Log("Tutorial scene loaded");
        levelController.OnSceneLoaded -= tutorialSceneLoader_OnSceneLoaded;
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
        levelController.OnSceneLoaded += gameSceneLoader_OnSceneLoaded;
        levelController.LoadScene(Scene.Game);
    }

    void gameSceneLoader_OnSceneLoaded()
    {
        levelController.OnSceneLoaded -= gameSceneLoader_OnSceneLoaded;
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
        levelController.OnSceneLoaded += gameOverSceneLoader_OnSceneLoaded;
        levelController.LoadScene(Scene.GameOver);
    }

    void gameOverSceneLoader_OnSceneLoaded()
    {
        levelController.OnSceneLoaded -= gameOverSceneLoader_OnSceneLoaded;
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

using UnityEngine;

namespace Controllers
{
    public class Controller : Singleton<Controller>
    {
        private TutorialController _tutorialController;
        private GameController _gameController;
        private GameOverController _gameOverController;
        private LevelController _levelController;
        private PlayerController _playerController;

        private void Start()
        {
            _playerController = PlayerController.Instance;
            _levelController = LevelController.Instance;

            StartTutorial();
            //StartGame();
        }

        private void StartTutorial()
        {
            Debug.Log("Starting tutorial");
            _levelController.OnSceneLoaded += tutorialSceneLoader_OnSceneLoaded;
            _levelController.LoadScene(Scene.Tutorial);
        }

        private void tutorialSceneLoader_OnSceneLoaded()
        {
            Debug.Log("Tutorial scene loaded");
            _levelController.OnSceneLoaded -= tutorialSceneLoader_OnSceneLoaded;
            GameObject tutController = GameObject.Find("TutorialController");
            _tutorialController = tutController.GetComponent<TutorialController>();
            _tutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
            ResetPlayer();
        }

        private void TutorialController_OnTutorialComplete()
        {
            Debug.Log("Tutorial complete, loading");
            _tutorialController.OnTutorialComplete -= TutorialController_OnTutorialComplete;
            _tutorialController = null;
            StartGame();
        }

        private void StartGame()
        {
            Debug.Log("Starting game");
            _levelController.OnSceneLoaded += gameSceneLoader_OnSceneLoaded;
            _levelController.LoadScene(Scene.Game);
        }

        private void gameSceneLoader_OnSceneLoaded()
        {
            _levelController.OnSceneLoaded -= gameSceneLoader_OnSceneLoaded;
            GameObject gController = GameObject.Find("GameController");
            _gameController = gController.GetComponent<GameController>();
            _gameController.OnGameEnded += GameController_OnGameEnded;
            ResetPlayer();
        }

        private void GameController_OnGameEnded()
        {
            _gameController.OnGameEnded -= GameController_OnGameEnded;
            _gameController = null;
            GameOver();
        }

        private void GameOver()
        {
            Debug.Log("Game over");
            _levelController.OnSceneLoaded += gameOverSceneLoader_OnSceneLoaded;
            _levelController.LoadScene(Scene.GameOver);
        }

        private void gameOverSceneLoader_OnSceneLoaded()
        {
            _levelController.OnSceneLoaded -= gameOverSceneLoader_OnSceneLoaded;
            GameObject goController = GameObject.Find("GameOverController");
            _gameOverController = goController.GetComponent<GameOverController>();
            _gameOverController.OnPlayAgain += GameOverController_OnPlayAgain;
            _gameOverController.OnGoToTutorial += GameOverController_OnGoToTutorial;
            ResetPlayer();
        }

        private void GameOverController_OnGoToTutorial()
        {
            RemoveGameOverListeners();
            StartTutorial();
        }

        private void GameOverController_OnPlayAgain()
        {
            RemoveGameOverListeners();
            StartGame();
        }

        private void ResetPlayer()
        {
            _playerController.Reset();
        }

        private void RemoveGameOverListeners()
        {
            _gameOverController.OnPlayAgain -= GameOverController_OnPlayAgain;
            _gameOverController.OnGoToTutorial -= GameOverController_OnGoToTutorial;
            _gameOverController = null;
        }
    }
}
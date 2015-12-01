using UnityEngine;

namespace Controllers
{
    public class Controller : Singleton<Controller>
    {
        private TutorialController _tutorialController;
        private GameController _gameController;
        private GameOverController _gameOverController;
        private LevelController _levelController;

        private void Start()
        {
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
            ResetPlayer();
        }

        public void SetTutorialController(TutorialController tutorialController)
        {
            _tutorialController = tutorialController;
            _tutorialController.OnTutorialComplete += TutorialController_OnTutorialComplete;
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
            ResetPlayer();
        }

        public void SetGameController(GameController gameController)
        {
            _gameController = gameController;
            _gameController.OnGameEnded += GameController_OnGameEnded;
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
            ResetPlayer();
        }

        public void SetGameOverController(GameOverController gameOverController)
        {
            _gameOverController = gameOverController;
            _gameOverController.OnPlayAgain += GameOverController_OnPlayAgain;
            _gameOverController.OnGoToTutorial += GameOverController_OnGoToTutorial;
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
            PlayerController.Instance.Reset();
        }

        private void RemoveGameOverListeners()
        {
            _gameOverController.OnPlayAgain -= GameOverController_OnPlayAgain;
            _gameOverController.OnGoToTutorial -= GameOverController_OnGoToTutorial;
            _gameOverController = null;
        }
    }
}
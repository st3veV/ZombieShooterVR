using System;
using UnityEngine;
using System.Collections;

namespace Controllers
{
    public class GameOverController : MonoBehaviour
    {

        public GameObject PlayAgainTarget;
        public GameObject GoToTutorialTarget;

        public TextMesh ScoreOutput;

        private LifetimeComponent _playAgainLifetime;
        private LifetimeComponent _gotoTutorialLifetime;

        private Gun _userGun;
        private UserData _userData;

        public event Action OnPlayAgain;
        public event Action OnGoToTutorial;

        private void Start()
        {
            Controller.Instance.SetGameOverController(this);
            AssignReferences();
        }

        private void AssignReferences()
        {
            _userData = UserData.Instance;
            _userGun = PlayerController.Instance.Gun;

            Initialize();
        }

        public void Initialize()
        {
            _playAgainLifetime = PlayAgainTarget.GetComponent<LifetimeComponent>();
            _playAgainLifetime.OnDie += _playAgainLifetime_OnDie;

            _gotoTutorialLifetime = GoToTutorialTarget.GetComponent<LifetimeComponent>();
            _gotoTutorialLifetime.OnDie += _gotoTutorialLifetime_OnDie;

            ScoreOutput.text = string.Format("Score: {0}", _userData.Score);

            _userGun.SetFlashlightEnabled(false);
        }

        private void _gotoTutorialLifetime_OnDie(LifetimeComponent obj)
        {
            RemoveListeners();
            OnOnGoToTutorial();
        }

        private void _playAgainLifetime_OnDie(LifetimeComponent obj)
        {
            RemoveListeners();
            OnOnPlayAgain();
        }

        private void RemoveListeners()
        {
            _playAgainLifetime.OnDie -= _playAgainLifetime_OnDie;
            _gotoTutorialLifetime.OnDie += _gotoTutorialLifetime_OnDie;
        }

        #region Event invocators

        protected virtual void OnOnPlayAgain()
        {
            var handler = OnPlayAgain;
            if (handler != null) handler();
        }

        protected virtual void OnOnGoToTutorial()
        {
            var handler = OnGoToTutorial;
            if (handler != null) handler();
        }

        #endregion
    }
}
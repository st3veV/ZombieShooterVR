using System;
using UnityEngine;
using System.Collections;

namespace Controllers
{
    public class GameOverController : MonoBehaviour
    {

        public GameObject PlayAgainTarget;
        public GameObject GoToTutorialTarget;

        public UserData UserData;
        public TextMesh ScoreOutput;

        private LifetimeComponent _playAgainLifetime;
        private LifetimeComponent _gotoTutorialLifetime;

        private Gun UserGun;

        public event Action OnPlayAgain;
        public event Action OnGoToTutorial;

        private void Start()
        {
            AssignReferences();
        }

        private void AssignReferences()
        {
            UserData = UserData.Instance;

            UserGun = GameObject.Find("Gun").GetComponent<Gun>();

            Initialize();
        }

        public void Initialize()
        {
            _playAgainLifetime = PlayAgainTarget.GetComponent<LifetimeComponent>();
            _playAgainLifetime.OnDie += _playAgainLifetime_OnDie;

            _gotoTutorialLifetime = GoToTutorialTarget.GetComponent<LifetimeComponent>();
            _gotoTutorialLifetime.OnDie += _gotoTutorialLifetime_OnDie;

            ScoreOutput.text = string.Format("Score: {0}", UserData.Score);

            UserGun.SetFlashlightEnabled(false);
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
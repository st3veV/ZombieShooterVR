using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class SetupController:MonoBehaviour
    {
        private ThalmicMyo _myo;

        public event Action OnSetupCompleted;

        public Text HeadingText;
        public Text ConnectText;
        public Button ConnectButton;
        public Button StartButton;
        public Text InCardboardText;
        public Button InCardboardButton;
        public Text StartingText;
        public Button QuitButton;
        
        void Start()
        {
            Controller.Instance.SetSetupController(this);
            StartButton.enabled = false;
            InCardboardButton.gameObject.SetActive(false);
            StartingText.gameObject.SetActive(false);

            QuitButton.onClick.AddListener(OnQuit);

            _myo = ThalmicHub.instance.GetComponentInChildren<ThalmicMyo>();
            if (_myo.isPaired)
            {
                Synced();
            }
            else
            {
                ConnectButton.gameObject.SetActive(true);
                ConnectButton.onClick.AddListener(OnConnectClick);
            }
        }

        void WaitForMyoUpdate()
        {
            if (_myo.isPaired)
            {
                EventManager.Instance.RemoveUpdateListener(WaitForMyoUpdate);
                Synced();
            }
        }

        private void OnQuit()
        {
            Application.Quit();
        }

        private void OnConnectClick()
        {
            EventManager.Instance.AddUpdateListener(WaitForMyoUpdate);
            ThalmicHub.instance.Init();
        }

        private void Synced()
        {
            Debug.Log("synced");
            ConnectButton.onClick.RemoveAllListeners();
            Destroy(ConnectButton.gameObject);
            ConnectText.color = Color.green;
            AddToCardboard();
        }

        private void AddToCardboard()
        {
            InCardboardButton.gameObject.SetActive(true);
            InCardboardButton.onClick.AddListener(OnInCardboardClicked);
        }

        private void OnInCardboardClicked()
        {
            InCardboardText.color = Color.green;
            InCardboardButton.onClick.RemoveAllListeners();
            Destroy(InCardboardButton.gameObject);
            StartEnabled();
        }

        private void StartEnabled()
        {
            StartButton.enabled = true;
            StartButton.onClick.AddListener(OnStartClicked);
        }

        private void OnStartClicked()
        {
            Debug.Log("Start");
            DestroyAll();
            StartCoroutine(StartGame());
        }

        private void DestroyAll()
        {
            Destroy(ConnectText.gameObject);
            Destroy(StartButton.gameObject);
            Destroy(InCardboardText.gameObject);
            Destroy(HeadingText.gameObject);
        }

        private IEnumerator StartGame()
        {
            StartingText.gameObject.SetActive(true);
            yield return new WaitForSeconds(5);
            OnOnSetupCompleted();
        }

        #region Event invocators

        protected virtual void OnOnSetupCompleted()
        {
            if (OnSetupCompleted != null) OnSetupCompleted();
        }

        #endregion
    }
}
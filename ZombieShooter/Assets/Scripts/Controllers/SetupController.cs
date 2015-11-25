using System.Collections;
using Thalmic.Myo;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class SetupController:MonoBehaviour
    {
        private bool waitingForMyo = false;
        private ThalmicMyo myo;
        private GameObject loader;

        public Text HeadingText;
        public Text ConnectText;
        public Button ConnectButton;
        public Button StartButton;
        public Text InCardboardText;
        public Button InCardboardButton;
        public Text StartingText;
        

        void Start()
        {
            StartButton.enabled = false;
            InCardboardButton.gameObject.SetActive(false);
            StartingText.gameObject.SetActive(false);

            myo = ThalmicHub.instance.GetComponentInChildren<ThalmicMyo>();
            if (myo.isPaired)
            {
                Synced();
            }
            else
            {
                ConnectButton.gameObject.SetActive(true);
                ConnectButton.onClick.AddListener(OnConnectClick);
            }
        }

        void Update()
        {
            if (waitingForMyo)
            {
                if (myo.isPaired)
                {
                    waitingForMyo = false;
                    Synced();
                }
            }
        }

        private void OnConnectClick()
        {
            ThalmicHub.instance.Init();
            waitingForMyo = true;
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
            LevelController levelController = LevelController.Instance;
            levelController.LoadScene(Scene.Init);
        }
    }
}
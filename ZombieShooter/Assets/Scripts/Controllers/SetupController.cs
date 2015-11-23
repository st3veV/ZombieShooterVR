using Thalmic.Myo;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class SetupController:MonoBehaviour
    {
        private bool waitingForMyo = false;
        private ThalmicMyo myo;

        public Text ConnectText;

        void Start()
        {
            myo = ThalmicHub.instance.GetComponentInChildren<ThalmicMyo>();
            if (myo.isPaired)
            {
                Synced();
            }
            else
            {
                ThalmicHub.instance.Init();
                waitingForMyo = true;
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

        private void Synced()
        {
            Debug.Log("synced");
            ConnectText.color = Color.green;
        }
        
    }
}
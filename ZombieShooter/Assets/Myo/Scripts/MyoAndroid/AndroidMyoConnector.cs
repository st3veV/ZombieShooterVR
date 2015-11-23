using System;
using UnityEngine;

namespace Thalmic.Myo.MyoAndroid
{
    public class AndroidMyoConnector:MonoBehaviour
    {
        public event EventHandler<MyoEventArgs> MyoConnected;

        public void Init(string appId, bool debug)
        {
            Debug.Log("initializing");
            Handler.Call("init", gameObject.name, appId, debug);
        }

        public void ConnectDirectly()
        {
            Handler.Call("connectDirectly");
        }

        public void ShowScanActivity()
        {
            Handler.Call("showScanActivity");
        }

        public void MyoSelected(string obj)
        {
            OnMyoConnected(new MyoEventArgs(null, DateTime.Now));
        }

        private AndroidJavaObject _handlerJavaObject;
        private AndroidJavaObject Handler
        {
            get
            {
                if (_handlerJavaObject == null)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.thalmic.myo.unity.MyoNativeConnector");
                    _handlerJavaObject = jc.CallStatic<AndroidJavaObject>("instance");
                    Debug.Log("got instance");
                }
                return _handlerJavaObject;
            }
        }

        #region Event Invocators

        private void OnMyoConnected(MyoEventArgs e)
        {
            var handler = MyoConnected;
            if(handler != null)
                handler(this, e);
        }

        #endregion
    }
}
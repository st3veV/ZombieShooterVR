using System;
using UnityEngine;
using Quaternion = Thalmic.Myo.Quaternion;

namespace Thalmic.Myo.MyoAndroid
{
    public class AndroidMyo: MonoBehaviour,IMyo
    {
        #region Implemented Events
        public event EventHandler<MyoEventArgs> Connected;
        public event EventHandler<MyoEventArgs> Disconnected;
        public event EventHandler<ArmSyncedEventArgs> ArmSynced;
        public event EventHandler<MyoEventArgs> ArmUnsynced;
        public event EventHandler<PoseEventArgs> PoseChange;
        public event EventHandler<OrientationDataEventArgs> OrientationData;
        public event EventHandler<AccelerometerDataEventArgs> AccelerometerData;
        public event EventHandler<GyroscopeDataEventArgs> GyroscopeData;
        public event EventHandler<RssiEventArgs> Rssi;
        public event EventHandler<MyoEventArgs> Unlocked;
        public event EventHandler<MyoEventArgs> Locked;
        #endregion

        #region Implemented Methods
        public void Vibrate(VibrationType type)
        {
            Handler.Call("myoVibrate", type.ToString());
        }

        public void Unlock(UnlockType type)
        {
            Handler.Call("myoUnlock", type.ToString());
        }

        public void Lock()
        {
            Handler.Call("myoLock");
        }

        public void NotifyUserAction()
        {
            Handler.Call("myoNotifyUserAction");
        }
        #endregion


        public void Init(bool debug)
        {
            Debug.Log("initializing");
            Handler.Call("init", gameObject.name, debug);
        }


        #region Listener for android stuff

        public void MyoOrientation(string param)
        {
            var arr = param.Split(' ');
            var myoQuat = new Quaternion(
                float.Parse(arr[1]),
                float.Parse(arr[2]),
                float.Parse(arr[3]),
                float.Parse(arr[0])
                );
            OnOrientationData(new OrientationDataEventArgs(this, GetTimestamp(), myoQuat));
        }

        public void MyoPose(string param)
        {
            Debug.Log("pose: " + param);
            Pose pose;
            switch (param)
            {
                case "Rest":
                    pose = Pose.Rest;
                    break;
                case "Tap":
                    pose = Pose.DoubleTap;
                    break;
                case "Fist":
                    pose = Pose.Fist;
                    break;
                case "Wave_In":
                    pose = Pose.WaveIn;
                    break;
                case "Wave_Out":
                    pose = Pose.WaveOut;
                    break;
                case "Spread":
                    pose = Pose.FingersSpread;
                    break;
                default:
                    pose = Pose.Unknown;
                    break;
            }
            OnPoseChange(new PoseEventArgs(this, GetTimestamp(), pose));
        }

        public void MyoArm(string armName)
        {
            Debug.Log("received arm: " + armName);
            Arm arm;
            switch (armName)
            {
                case "LEFT":
                    arm = Arm.Left;
                    break;
                case "RIGHT":
                    arm = Arm.Right;
                    break;
                default:
                    arm = Arm.Unknown;
                    break;
            }
            _currentArm = arm;
            Sync(_currentArm, _xDirection);
        }

        public void MyoDirection(string directionName)
        {
            Debug.Log("received direction: " + directionName);
            XDirection direction;
            switch (directionName)
            {
                case "TOWARD_ELBOW":
                    direction = XDirection.TowardElbow;
                    break;
                case "TOWARD_WRIST":
                    direction = XDirection.TowardWrist;
                    break;
                default:
                    direction = XDirection.Unknown;
                    break;
            }
            _xDirection = direction;
            Sync(_currentArm, direction);
        }
        
        public void MyoDisconnected(string obj)
        {
            OnDisconnected(new MyoEventArgs(this, GetTimestamp()));
        }

        #endregion

        private Arm _currentArm = Arm.Unknown;
        private XDirection _xDirection = XDirection.Unknown;
        
        private void Sync(Arm arm, XDirection xDirection)
        {
            if (arm != Arm.Unknown && xDirection != XDirection.Unknown)
            {
                OnArmSynced(new ArmSyncedEventArgs(this, GetTimestamp(), arm, xDirection));
            }
        }

        private DateTime GetTimestamp()
        {
            return DateTime.Now;
        }

        private AndroidJavaObject _handlerJavaObject;
        private AndroidJavaObject Handler
        {
            get
            {
                if (_handlerJavaObject == null)
                {
                    AndroidJavaClass jc = new AndroidJavaClass("com.thalmic.myo.unity.MyoNativeHandler");
                    _handlerJavaObject = jc.CallStatic<AndroidJavaObject>("instance");
                    Debug.Log("got instance");
                }
                return _handlerJavaObject;
            }
        }

        #region Event invocators

        protected virtual void OnConnected(MyoEventArgs e)
        {
            var handler = Connected;
            if(handler != null) handler(this, e);
        }

        protected virtual void OnDisconnected(MyoEventArgs e)
        {
            var handler = Disconnected;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnArmSynced(ArmSyncedEventArgs e)
        {
            var handler = ArmSynced;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnArmUnsynced(MyoEventArgs e)
        {
            var handler = ArmUnsynced;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPoseChange(PoseEventArgs e)
        {
            var handler = PoseChange;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnOrientationData(OrientationDataEventArgs e)
        {
            var handler = OrientationData;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnAccelerometerData(AccelerometerDataEventArgs e)
        {
            var handler = AccelerometerData;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnGyroscopeData(GyroscopeDataEventArgs e)
        {
            var handler = GyroscopeData;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnRssi(RssiEventArgs e)
        {
            var handler = Rssi;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUnlocked(MyoEventArgs e)
        {
            var handler = Unlocked;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnLocked(MyoEventArgs e)
        {
            var handler = Locked;
            if (handler != null) handler(this, e);
        }

        #endregion

    }
}
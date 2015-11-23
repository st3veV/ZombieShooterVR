using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public class Myo:IMyo
    {
        private readonly Hub _hub;
        private IntPtr _handle;

        internal Myo(Hub hub, IntPtr handle)
        {
            Debug.Assert(handle != IntPtr.Zero, "Cannot construct Myo instance with null pointer.");

            _hub = hub;
            _handle = handle;
        }

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

        internal Hub Hub
        {
            get { return _hub; }
        }

        internal IntPtr Handle
        {
            get { return _handle; }
        }

        public void Vibrate(VibrationType type)
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.vibrate(_handle, (libmyo.VibrationType)type, IntPtr.Zero);
#endif
        }

        public void RequestRssi()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.request_rssi(_handle, IntPtr.Zero);
#endif
        }

        public void Unlock(UnlockType type)
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.myo_unlock(_handle, (libmyo.UnlockType)type, IntPtr.Zero);
#endif
        }

        public void Lock()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.myo_lock(_handle, IntPtr.Zero);
#endif
        }

        public void NotifyUserAction()
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            libmyo.myo_notify_user_action(_handle, libmyo.UserActionType.Single, IntPtr.Zero);
#endif
        }

        internal void HandleEvent(libmyo.EventType type, DateTime timestamp, IntPtr evt)
        {
            switch (type)
            {
                case libmyo.EventType.Connected:
                    if (Connected != null)
                    {
                        Connected(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.Disconnected:
                    if (Disconnected != null)
                    {
                        Disconnected(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.ArmSynced:
                    if (ArmSynced != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        Arm arm = (Arm)libmyo.event_get_arm(evt);
                        XDirection xDirection = (XDirection)libmyo.event_get_x_direction(evt);

                        ArmSynced(this, new ArmSyncedEventArgs(this, timestamp, arm, xDirection));
#endif
                    }
                    break;

                case libmyo.EventType.ArmUnsynced:
                    if (ArmUnsynced != null)
                    {
                        ArmUnsynced(this, new MyoEventArgs(this, timestamp));
                    }
                    break;

                case libmyo.EventType.Orientation:
                    if (AccelerometerData != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        float x = libmyo.event_get_accelerometer(evt, 0);
                        float y = libmyo.event_get_accelerometer(evt, 1);
                        float z = libmyo.event_get_accelerometer(evt, 2);

                        var accelerometer = new Vector3(x, y, z);
                        AccelerometerData(this, new AccelerometerDataEventArgs(this, timestamp, accelerometer));
#endif
                    }
                    if (GyroscopeData != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        float x = libmyo.event_get_gyroscope(evt, 0);
                        float y = libmyo.event_get_gyroscope(evt, 1);
                        float z = libmyo.event_get_gyroscope(evt, 2);

                        var gyroscope = new Vector3(x, y, z);
                        GyroscopeData(this, new GyroscopeDataEventArgs(this, timestamp, gyroscope));
#endif
                    }
                    if (OrientationData != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        float x = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.X);
                        float y = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.Y);
                        float z = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.Z);
                        float w = libmyo.event_get_orientation(evt, libmyo.OrientationIndex.W);

                        var orientation = new Quaternion(x, y, z, w);
                        OrientationData(this, new OrientationDataEventArgs(this, timestamp, orientation));
#endif
                    }
                    break;

                case libmyo.EventType.Pose:
                    if (PoseChange != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        var pose = (Pose)libmyo.event_get_pose(evt);
                        PoseChange(this, new PoseEventArgs(this, timestamp, pose));
#endif
                    }
                    break;

                case libmyo.EventType.Rssi:
                    if (Rssi != null)
                    {
#if !UNITY_ANDROID || UNITY_EDITOR
                        var rssi = libmyo.event_get_rssi(evt);
                        Rssi(this, new RssiEventArgs(this, timestamp, rssi));
#endif
                    }
                    break;
                case libmyo.EventType.Unlocked:
                    if (Unlocked != null)
                    {
                        Unlocked(this, new MyoEventArgs(this, timestamp));
                    }
                    break;
                case libmyo.EventType.Locked:
                    if (Locked != null)
                    {
                        Locked(this, new MyoEventArgs(this, timestamp));
                    }
                    break;
            }
        }
    }

    public enum Arm
    {
        Right,
        Left,
        Unknown
    }

    public enum XDirection
    {
        TowardWrist,
        TowardElbow,
        Unknown
    }

    public enum VibrationType
    {
        Short,
        Medium,
        Long
    }

    public enum UnlockType
    {
        Timed = 0,  ///< Unlock for a fixed period of time.
        Hold = 1    ///< Unlock until explicitly told to re-lock.
    }
}

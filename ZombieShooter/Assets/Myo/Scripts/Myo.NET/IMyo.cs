using System;

namespace Thalmic.Myo
{
    public interface IMyo
    {
        #region Events
        event EventHandler<MyoEventArgs> Connected;

        event EventHandler<MyoEventArgs> Disconnected;

        event EventHandler<ArmSyncedEventArgs> ArmSynced;

        event EventHandler<MyoEventArgs> ArmUnsynced;

        event EventHandler<PoseEventArgs> PoseChange;

        event EventHandler<OrientationDataEventArgs> OrientationData;

        event EventHandler<AccelerometerDataEventArgs> AccelerometerData;

        event EventHandler<GyroscopeDataEventArgs> GyroscopeData;

        event EventHandler<RssiEventArgs> Rssi;

        event EventHandler<MyoEventArgs> Unlocked;

        event EventHandler<MyoEventArgs> Locked;
        #endregion

        #region Methods

        void Vibrate(VibrationType type);

        void Unlock(UnlockType type);

        void Lock();

        void NotifyUserAction();
        
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thalmic.Myo
{
    public class MyoEventArgs : EventArgs
    {
        public MyoEventArgs(IMyo myo, DateTime timestamp)
        {
            this.Myo = myo;
            this.Timestamp = timestamp;
        }

        public IMyo Myo { get; private set; }

        public DateTime Timestamp { get; private set; }
    }

    public class ArmSyncedEventArgs : MyoEventArgs
    {
        public ArmSyncedEventArgs(IMyo myo, DateTime timestamp, Arm arm, XDirection xDirection)
            : base(myo, timestamp)
        {
            this.Arm = arm;
            this.XDirection = xDirection;
        }

        public Arm Arm { get; private set; }
        public XDirection XDirection { get; private set; }
    }

    public class AccelerometerDataEventArgs : MyoEventArgs
    {
        public AccelerometerDataEventArgs(IMyo myo, DateTime timestamp, Vector3 accelerometer)
            : base(myo, timestamp)
        {
            this.Accelerometer = accelerometer;
        }

        public Vector3 Accelerometer { get; private set; }
    }

    public class GyroscopeDataEventArgs : MyoEventArgs
    {
        public GyroscopeDataEventArgs(IMyo myo, DateTime timestamp, Vector3 gyroscope)
            : base(myo, timestamp)
        {
            this.Gyroscope = gyroscope;
        }

        public Vector3 Gyroscope { get; private set; }
    }

    public class OrientationDataEventArgs : MyoEventArgs
    {
        public OrientationDataEventArgs(IMyo myo, DateTime timestamp, Quaternion orientation)
            : base(myo, timestamp)
        {
            this.Orientation = orientation;
        }

        public Quaternion Orientation { get; private set; }
    }

    public class PoseEventArgs : MyoEventArgs
    {
        public PoseEventArgs(IMyo myo, DateTime timestamp, Pose pose)
            : base(myo, timestamp)
        {
            this.Pose = pose;
        }

        public Pose Pose { get; private set; }
    }

    public class RssiEventArgs : MyoEventArgs
    {
        public RssiEventArgs(IMyo myo, DateTime timestamp, sbyte rssi)
            : base(myo, timestamp)
        {
            this.Rssi = rssi;
        }

        public sbyte Rssi { get; private set; }
    }
}

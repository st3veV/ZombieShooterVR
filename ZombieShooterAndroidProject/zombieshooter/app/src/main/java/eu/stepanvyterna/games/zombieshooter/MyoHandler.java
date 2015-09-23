package eu.stepanvyterna.games.zombieshooter;

import android.content.Context;
import android.util.Log;
import android.widget.Toast;

import com.thalmic.myo.AbstractDeviceListener;
import com.thalmic.myo.DeviceListener;
import com.thalmic.myo.Hub;
import com.thalmic.myo.Myo;
import com.thalmic.myo.Pose;
import com.thalmic.myo.Quaternion;
import com.unity3d.player.UnityPlayer;

/**
 * Created by Steve on 23.9.2015.
 */
public class MyoHandler {

    private static String TAG = "MyoHandler";

    private static MyoHandler _instance;
    public Myo connectedMyo;

    public static MyoHandler instance()
    {
        if(_instance == null)
        {
            _instance = new MyoHandler();
            Log.d(TAG,"creating instance");
        }
        return _instance;
    }

    public void init()
    {
        Log.d(TAG, "init");
        Hub hub = Hub.getInstance();

        DeviceListener mListener = new AbstractDeviceListener() {

            // onOrientationData() is called whenever a Myo provides its current orientation,
// represented as a quaternion.
            @Override
            public void onOrientationData(Myo myo, long timestamp, Quaternion rotation) {
// Calculate Euler angles (roll, pitch, and yaw) from the quaternion.
                float roll = (float) Math.toDegrees(Quaternion.roll(rotation));
                float pitch = (float) Math.toDegrees(Quaternion.pitch(rotation));
                float yaw = (float) Math.toDegrees(Quaternion.yaw(rotation));
// Adjust roll and pitch for the orientation of the Myo on the arm.
//				System.out.println("Quaternion: " + rotation.toString());
                //UnityPlayer.UnitySendMessage("MessageManager", "MyoRotation",  String.format("%f %f %f %f", rotation.x(), rotation.y(), rotation.z(), rotation.w()));
// Next, we send a rotation to the unity using the roll, pitch, and yaw.

                //UnityPlayer.UnitySendMessage("MessageManager", "MyoRotation",  String.format("%f %f %f", roll, -pitch, yaw));
                UnityPlayer.UnitySendMessage("MessageManager", "MyoQuaternion", String.format("%f %f %f %f", rotation.w(), rotation.x(), rotation.y(), rotation.z()));
            }

            // onPose() is called whenever a Myo provides a new pose.
            @Override
            public void onPose(Myo myo, long timestamp, Pose pose) {
                String poseText = null;
                System.out.println("myo pose performed: " + pose.toString());
// Handle the cases of the Pose enumeration, and change the text of the text view
// based on the pose we receive.
                switch (pose) {
                    case UNKNOWN:
                        break;
                    case REST:
                        poseText = "Rest";
                        break;
                    case DOUBLE_TAP:
                        poseText = "Tap";
                        break;
                    case FIST:
                        poseText = "Fist";
                        break;
                    case WAVE_IN:
                        poseText = "Wave_In";
                        break;
                    case WAVE_OUT:
                        poseText = "Wave_Out";
                        break;
                    case FINGERS_SPREAD:
                        poseText = "Spread";
                        break;
                }
                /*
                if (pose != Pose.UNKNOWN && pose != Pose.REST) {
// Tell the Myo to stay unlocked until told otherwise. We do that here so you can
// hold the poses without the Myo becoming locked.
					myo.unlock(Myo.UnlockType.HOLD);
// Notify the Myo that the pose has resulted in an action, in this case changing
// the text on the screen. The Myo will vibrate.
					//myo.notifyUserAction();
				} else {
// Tell the Myo to stay unlocked only for a short period. This allows the Myo to
// stay unlocked while poses are being performed, but lock after inactivity.
					myo.unlock(Myo.UnlockType.TIMED);
				}
				*/
                UnityPlayer.UnitySendMessage("MessageManager", "MyoPose", poseText);
            }

            @Override
            public void onConnect(Myo myo, long timestamp) {
                Toast.makeText(getUnityContext(), "Myo Connected!", Toast.LENGTH_SHORT).show();

                Toast.makeText(getUnityContext(), "Myo settings: Arm=" + myo.getArm().name().toLowerCase() + ", Direction=" + myo.getXDirection().name().toLowerCase(), Toast.LENGTH_LONG).show();

                UnityPlayer.UnitySendMessage("MessageManager", "MyoArm", myo.getArm().name());
                UnityPlayer.UnitySendMessage("MessageManager", "MyoDirection", myo.getXDirection().name());

                MyoHandler.instance().connectedMyo = myo;
            }

            @Override
            public void onDisconnect(Myo myo, long timestamp) {
                Toast.makeText(getUnityContext(), "Myo Disconnected!", Toast.LENGTH_SHORT).show();
            }
        };

        hub.addListener(mListener);

        hub.setLockingPolicy(Hub.LockingPolicy.NONE);
    }

    private Context getUnityContext()
    {
        return UnityPlayer.currentActivity.getApplicationContext();
    }

    public int myoNotifyUserAction()
    {
        System.out.println("myo notify user action");

        if(connectedMyo != null)
        {
            connectedMyo.notifyUserAction();
            return 0;
        }
        return 1;
    }

    public int myoVibrate(String typeName)
    {
        System.out.println("vibrate myo: " + typeName);
        if(connectedMyo != null)
        {
            Myo.VibrationType type;
            switch (typeName)
            {
                case "Long":
                    type = Myo.VibrationType.LONG;
                    break;
                case "Medium":
                    type = Myo.VibrationType.MEDIUM;
                    break;
                case "Short":
                default:
                    type = Myo.VibrationType.SHORT;
            }
            connectedMyo.vibrate(type);
            return 0;
        }
        return 1;
    }

    public int myoLock()
    {
        System.out.println("myo lock");

        if(connectedMyo != null)
        {
            connectedMyo.lock();
            return 0;
        }
        return 1;
    }

    public int myoUnlock(String unlockTypeName)
    {
        System.out.println("myo unlock: " + unlockTypeName);

        if(connectedMyo != null)
        {
            Myo.UnlockType type;
            switch (unlockTypeName)
            {
                case "Hold":
                    type = Myo.UnlockType.HOLD;
                    break;
                case "Timed":
                default:
                    type = Myo.UnlockType.TIMED;
                    break;
            }
            connectedMyo.unlock(type);
            return 0;
        }
        return 1;
    }

}

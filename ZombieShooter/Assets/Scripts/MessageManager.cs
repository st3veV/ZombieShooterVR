using UnityEngine;
using System.Collections;

using Pose = Thalmic.Myo.Pose;
using MyoQaternion = Thalmic.Myo.Quaternion;
using Arm = Thalmic.Myo.Arm;
using XDirection = Thalmic.Myo.XDirection;

public class MessageManager : MonoBehaviour
{
    //Vector3 newEulerAngles = Vector3.zero;
    GameObject myo;
    ThalmicMyo thalmicMyo;
    string curPose;

    // Use this for initialization
    void Start()
    {
        //myo = GameObject.FindWithTag("Myo");
        myo = GameObject.Find("Myo");
        thalmicMyo = myo.GetComponent<ThalmicMyo>();
    }

    public void MyoRotation(string param)
    {
        string[] arr = param.Split(' ');
        //myo.transform.eulerAngles = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[0]));
        myo.transform.localEulerAngles = new Vector3(float.Parse(arr[1]), float.Parse(arr[2]), float.Parse(arr[0]));
    }

    public void MyoQuaternion(string param)
    {
        string[] arr = param.Split(' ');
        MyoQaternion myoQuat = new MyoQaternion(
            float.Parse(arr[1]),
            float.Parse(arr[2]),
            float.Parse(arr[3]),
            float.Parse(arr[0])
        );
        thalmicMyo.SetQuaternion(myoQuat);
    }

    public void MyoPose(string param)
    {
        Debug.Log("pose: " + param);
        curPose = param;
        if (param != null)
        {
            switch (param)
            {
                case "Rest":
                    thalmicMyo.pose = Pose.Rest;
                    break;
                case "Tap":
                    thalmicMyo.pose = Pose.DoubleTap;
                    break;
                case "Fist":
                    thalmicMyo.pose = Pose.Fist;
                    break;
                case "Wave_In":
                    thalmicMyo.pose = Pose.WaveIn;
                    break;
                case "Wave_Out":
                    thalmicMyo.pose = Pose.WaveOut;
                    break;
                case "Spread":
                    thalmicMyo.pose = Pose.FingersSpread;
                    break;
            }
        }
    }

    public void MyoArm(string armName)
    {
        Debug.Log("received arm: " + armName);
        switch (armName)
        {
            case "LEFT":
                thalmicMyo.SetArm(Arm.Left);
                break;
            case "RIGHT":
                thalmicMyo.SetArm(Arm.Right);
                break;
            default:
                thalmicMyo.SetArm(Arm.Unknown);
                break;
        }

    }

    public void MyoDirection(string directionName)
    {
        Debug.Log("received direction: " + directionName);
        switch (directionName)
        {
            case "TOWARD_ELBOW":
                thalmicMyo.SetDirection(XDirection.TowardElbow);
                break;
            case "TOWARD_WRIST":
                thalmicMyo.SetDirection(XDirection.TowardWrist);
                break;
            default:
                thalmicMyo.SetDirection(XDirection.Unknown);
                break;
        }
    }

    public string GetMyoPose() { return curPose; }

    public void Init()
    {
        Debug.Log("initializing");
        android.Call("init");
    }

    private AndroidJavaObject jo;
    private AndroidJavaObject android
    {
        get
        {
            if (jo == null)
            {
                AndroidJavaClass jc = new AndroidJavaClass("eu.stepanvyterna.games.zombieshooter.MyoHandler");
                jo = jc.CallStatic<AndroidJavaObject>("instance");
                Debug.Log("got instance");
            }
            return jo;
        }
    }

    internal void Unlock(Thalmic.Myo.UnlockType type)
    {
#if UNITY_ANDROID
        android.Call<int>("myoUnlock", type.ToString());
#endif
    }

    internal void Lock()
    {
#if UNITY_ANDROID
        android.Call<int>("myoLock");
#endif
    }

    internal void NotifyUserAction()
    {
#if UNITY_ANDROID
        android.Call<int>("myoNotifyUserAction");
#endif 
    }

    internal void Vibrate(Thalmic.Myo.VibrationType type)
    {
#if UNITY_ANDROID
        android.Call<int>("myoVibrate", type.ToString());
#endif
    }
}
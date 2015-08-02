using UnityEngine;
using System.Collections;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;

public class JointOrientation : MonoBehaviour
{
    public GameObject myo = null;
    public Cardboard cardboard;
    public GameObject gun;
    private Gun gunInternal;

    private Quaternion _antiYaw = Quaternion.identity;

    private float _referenceRoll = 0.0f;

    private Pose _lastPose = Pose.Unknown;

    void Start()
    {
        gunInternal = gun.GetComponent<Gun>();
    }

    // Update is called once per frame.
    void Update ()
    {
        ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();

        bool updateReference = false;
        if (thalmicMyo.pose != _lastPose) {
            _lastPose = thalmicMyo.pose;

            if (thalmicMyo.pose == Pose.FingersSpread) {
                updateReference = true;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
            }
        }
        if (Input.GetKeyDown ("r")) {
            updateReference = true;
        }

        if (updateReference) {
            _antiYaw = Quaternion.FromToRotation (
                new Vector3(myo.transform.forward.x, 0, myo.transform.forward.z),
                new Vector3(0, 0, 1)
            );
             //cardboard.transform.forward.z

            Vector3 referenceZeroRoll = computeZeroRollVector (myo.transform.forward);
            _referenceRoll = rollFromZero (referenceZeroRoll, myo.transform.forward, myo.transform.up);

            thalmicMyo.NotifyUserAction();
            thalmicMyo.NotifyUserAction();
        }

        Vector3 zeroRoll = computeZeroRollVector (myo.transform.forward);
        float roll = rollFromZero (zeroRoll, myo.transform.forward, myo.transform.up);

        float relativeRoll = normalizeAngle (roll - _referenceRoll);

        Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, myo.transform.forward);

        transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation (myo.transform.forward);

        if (thalmicMyo.xDirection == Thalmic.Myo.XDirection.TowardWrist) {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }

        if(thalmicMyo.pose == Pose.Fist)
        {
            if(isFiring == false)
            {
                thalmicMyo.NotifyUserAction();
            }
            isFiring = true;
        }
        else if(thalmicMyo.pose == Pose.Rest)
        {
            if(isFiring == true)
            {
                thalmicMyo.NotifyUserAction();
            }
            isFiring = false;
        }

        if(isFiring)
        {
            gunInternal.Fire();
        }
    }

    bool isFiring = false;

    private void toggleFire()
    {
        isFiring = !isFiring;
    }

    float rollFromZero (Vector3 zeroRoll, Vector3 forward, Vector3 up)
    {
        float cosine = Vector3.Dot (up, zeroRoll);
        Vector3 cp = Vector3.Cross (up, zeroRoll);
        float directionCosine = Vector3.Dot (forward, cp);
        float sign = directionCosine < 0.0f ? 1.0f : -1.0f;

        return sign * Mathf.Rad2Deg * Mathf.Acos (cosine);
    }

    Vector3 computeZeroRollVector (Vector3 forward)
    {
        Vector3 antigravity = Vector3.up;
        Vector3 m = Vector3.Cross (myo.transform.forward, antigravity);
        Vector3 roll = Vector3.Cross (m, myo.transform.forward);

        return roll.normalized;
    }

    float normalizeAngle (float angle)
    {
        if (angle > 180.0f) {
            return angle - 360.0f;
        }
        if (angle < -180.0f) {
            return angle + 360.0f;
        }
        return angle;
    }

    void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard) {
            myo.Unlock (UnlockType.Timed);
        }

        myo.NotifyUserAction ();
    }
}

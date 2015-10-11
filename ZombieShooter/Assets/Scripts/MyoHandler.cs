using System;
using System.Collections.Generic;
using Thalmic.Myo;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MyoHandler : MonoBehaviour
{
    public GameObject myo = null;
    private ThalmicMyo thalmicMyo;

    public GameObject gun;
    private Gun gunInternal;

    public List<GameObject> HandSideOffsetUpdate = new List<GameObject>();

    private Quaternion _antiYaw = Quaternion.identity;

    private float _referenceRoll = 0.0f;

    private Pose _lastPose = Pose.Unknown;
    private Arm _lastArm = Arm.Unknown;
    private bool _reloadLock = false;

    private float _handPositionOffset = 0.4f;

    public event Action OnMyoReset;

    private void Start()
    {
        gunInternal = gun.GetComponent<Gun>();
        gunInternal.OnWeaponKick += gunInternal_OnWeaponKick;
        thalmicMyo = myo.GetComponent<ThalmicMyo>();
    }

    void gunInternal_OnWeaponKick()
    {
        thalmicMyo.NotifyUserAction();
    }

    void Update ()
    {
        //move hand to the side to properly simulate hand position
        if (thalmicMyo.arm != _lastArm)
        {
            _lastArm = thalmicMyo.arm;
            UpdateHandOffsetPosition();
        }

        bool updateReference = false;
        if (thalmicMyo.pose != _lastPose) {
            _lastPose = thalmicMyo.pose;

            //if (thalmicMyo.pose == Pose.FingersSpread) {
            if (thalmicMyo.pose == Pose.DoubleTap) {
                updateReference = true;

                ExtendUnlockAndNotifyUserAction(thalmicMyo);
                MyoReset();
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

        if (thalmicMyo.xDirection == XDirection.TowardWrist) {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }


        if (thalmicMyo.pose == Pose.Rest)
        {
            gunInternal.StopShooting();
        }

        if (thalmicMyo.pose == Pose.Fist)
        {
            gunInternal.StartShooting();
        }

        if (gunInternal.ShellsInMagazine == 0 || thalmicMyo.pose == Pose.DoubleTap)
        {
            if (thalmicMyo.transform.forward.y >= 0.9)
            {
                if (!_reloadLock)
                {
                    gunInternal.Reload();
                    thalmicMyo.NotifyUserAction();
                    _reloadLock = true;
                }
            }
            else
            {
                if (_reloadLock)
                    _reloadLock = false;
            }
        }

    }

    private void UpdateHandOffsetPosition()
    {
        foreach (GameObject o in HandSideOffsetUpdate)
        {
            Vector3 localPosition = o.transform.position;
            if (thalmicMyo.arm == Arm.Right)
            {
                localPosition.Set(_handPositionOffset, localPosition.y, localPosition.z);
            }
            else if (thalmicMyo.arm == Arm.Left)
            {
                localPosition.Set(-_handPositionOffset, localPosition.y, localPosition.z);
            }
            o.transform.position = localPosition;
        }
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

    protected virtual void MyoReset()
    {
        var handler = OnMyoReset;
        if (handler != null) handler();
    }
}

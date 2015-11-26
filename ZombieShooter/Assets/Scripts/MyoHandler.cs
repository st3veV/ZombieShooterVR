using System;
using System.Collections.Generic;
using Thalmic.Myo;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MyoHandler : MonoBehaviour
{
    private GameObject _myo = null;
    private ThalmicMyo _thalmicMyo;
    
    public Gun Gun;

    public List<GameObject> HandSideOffsetUpdate = new List<GameObject>();

    private Quaternion _antiYaw = Quaternion.identity;

    private float _referenceRoll = 0.0f;

    private Pose _lastPose = Pose.Unknown;
    private Arm _lastArm = Arm.Unknown;
    private bool _reloadLock = false;

    private float _handPositionOffset = 0.3f;

    public event Action OnMyoReset;

    private void Start()
    {
        Gun.OnWeaponKick += GunOnWeaponKick;
        
        _thalmicMyo = ThalmicHub.instance.GetComponentInChildren<ThalmicMyo>();//myo.GetComponent<ThalmicMyo>();
        _myo = _thalmicMyo.gameObject;
    }

    void GunOnWeaponKick()
    {
        _thalmicMyo.NotifyUserAction();
    }

    void Update ()
    {
        //move hand to the side to properly simulate hand position
        if (_thalmicMyo.arm != _lastArm)
        {
            _lastArm = _thalmicMyo.arm;
            UpdateHandOffsetPosition();
        }

        bool updateReference = false;
        if (_thalmicMyo.pose != _lastPose) {
            _lastPose = _thalmicMyo.pose;

            if (_thalmicMyo.pose == Pose.FingersSpread) {
            //if (thalmicMyo.pose == Pose.DoubleTap) {
                updateReference = true;

                ExtendUnlockAndNotifyUserAction(_thalmicMyo);
                MyoReset();
            }
        }
        if (Input.GetKeyDown ("r")) {
            updateReference = true;
        }

        if (updateReference) {
            _antiYaw = Quaternion.FromToRotation (
                new Vector3(_myo.transform.forward.x, 0, _myo.transform.forward.z),
                new Vector3(0, 0, 1)
            );
             //cardboard.transform.forward.z

            Vector3 referenceZeroRoll = computeZeroRollVector (_myo.transform.forward);
            _referenceRoll = rollFromZero (referenceZeroRoll, _myo.transform.forward, _myo.transform.up);

            _thalmicMyo.NotifyUserAction();
            _thalmicMyo.NotifyUserAction();
        }

        Vector3 zeroRoll = computeZeroRollVector (_myo.transform.forward);
        float roll = rollFromZero (zeroRoll, _myo.transform.forward, _myo.transform.up);

        float relativeRoll = normalizeAngle (roll - _referenceRoll);

        Quaternion antiRoll = Quaternion.AngleAxis (relativeRoll, _myo.transform.forward);

        transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation (_myo.transform.forward);

        if (_thalmicMyo.xDirection == XDirection.TowardWrist) {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }


        if (_thalmicMyo.pose == Pose.Rest)
        {
            Gun.StopShooting();
        }

        if (_thalmicMyo.pose == Pose.Fist)
        {
            Gun.StartShooting();
        }

        if (Gun.ShellsInMagazine == 0 || _thalmicMyo.pose == Pose.DoubleTap)
        {
            if (_thalmicMyo.transform.forward.y >= 0.9)
            {
                if (!_reloadLock)
                {
                    Gun.Reload();
                    _thalmicMyo.NotifyUserAction();
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
            if (_thalmicMyo.arm == Arm.Right)
            {
                localPosition.Set(_handPositionOffset, localPosition.y, localPosition.z);
            }
            else if (_thalmicMyo.arm == Arm.Left)
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
        Vector3 m = Vector3.Cross (_myo.transform.forward, antigravity);
        Vector3 roll = Vector3.Cross (m, _myo.transform.forward);

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

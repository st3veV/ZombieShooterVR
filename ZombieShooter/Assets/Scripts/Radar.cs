using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{

    public GameObject LookAtGameObject;
    public GameObject ControlRotationGameObject;
    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;

    private Transform _helperTransform;
    private float _radarRadius = 15.0f;

    private GameObject[] _trackedObjects;
    private Dictionary<GameObject, GameObject> _trackedAvatars;
    private Dictionary<GameObject, Transform> _avatarParents;

	// Use this for initialization
	void Start () {
        GameObject helper = new GameObject("helper");
        helper.transform.SetParent(transform);
	    _helperTransform = helper.transform;

        if(ZombieSpawner != null)
        {
            ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;
        }

        if(WeaponSpawner != null)
        {
            WeaponSpawner.OnWeaponTargetSpawned += WeaponSpawner_OnWeaponTargetSpawned;
        }

        _trackedObjects = new GameObject[0];
        _trackedAvatars = new Dictionary<GameObject, GameObject>();
        _avatarParents = new Dictionary<GameObject, Transform>();
	}

    private void WeaponSpawner_OnWeaponTargetSpawned(GameObject weaponTarget)
    {
        LifetimeComponent weaponTargetLife = weaponTarget.GetComponent<LifetimeComponent>();
        weaponTargetLife.OnDie += weaponTargetLife_OnDie;

        if(!_trackedAvatars.ContainsKey(weaponTarget))
        {
            Transform avatarParent = weaponTarget.transform.FindChild("Container");
            _trackedAvatars.Add(weaponTarget, avatarParent.FindChild("TargetAvatar").gameObject);
            _avatarParents.Add(weaponTarget, avatarParent);
        }
        UpdateTrackedObjectsSet();
    }

    private void weaponTargetLife_OnDie(LifetimeComponent obj)
    {
        resetAvatar(obj);
        obj.OnDie -= weaponTargetLife_OnDie;
        UpdateTrackedObjectsSet();
    }

    private void ZombieSpawner_OnZombieSpawned(GameObject zombie)
    {
        LifetimeComponent zombieLife = zombie.GetComponent<LifetimeComponent>();
        zombieLife.OnDie += zombieLife_OnDie;
        if (!_trackedAvatars.ContainsKey(zombie))
        {
            _trackedAvatars.Add(zombie, zombie.transform.FindChild("EnemyAvatar").gameObject);
            _avatarParents.Add(zombie, zombie.transform);
        }
        UpdateTrackedObjectsSet();
    }

    private void zombieLife_OnDie(LifetimeComponent obj)
    {
        resetAvatar(obj);
        obj.OnDie -= zombieLife_OnDie;
        UpdateTrackedObjectsSet();
    }

	// Update is called once per frame
	void Update ()
	{
        //update camera transform
	    Vector3 eulerAngles = LookAtGameObject.transform.rotation.eulerAngles;
	    _helperTransform.rotation = Quaternion.Euler(90, eulerAngles.y, 0);
	    ControlRotationGameObject.transform.rotation = _helperTransform.rotation;

        if (_trackedObjects.Length > 0)
        {
            UpdateTrackedObjects();
        }

	}

    private void resetAvatar(LifetimeComponent obj)
    {
        resetAvatar(obj.gameObject);
    }

    private void resetAvatar(GameObject obj)
    {
        if (_trackedAvatars.ContainsKey(obj))
        {
            Transform avatarParent = _avatarParents[obj];
            GameObject avatar = _trackedAvatars[obj];
            if (avatar != null)
            {
                avatar.transform.SetParent(avatarParent);
            }
            _trackedAvatars.Remove(obj);
            _avatarParents.Remove(obj);
        }
    }

    private void UpdateTrackedObjectsSet()
    {
        List<GameObject> tObjs = new List<GameObject>();
        foreach(GameObject o in _trackedAvatars.Keys)
        {
            tObjs.Add(o);
        }
        _trackedObjects = tObjs.ToArray();
    }

    private void UpdateTrackedObjects()
    {
        //UpdateEnemySet();
        GameObject avatar;
        Vector3 position;
        bool needsAvatar = false;
        Transform avatarParent;
        List<GameObject> destroyedObjects = new List<GameObject>();
        foreach (GameObject o in _trackedObjects)
        {
            if (o == null)
            {
                destroyedObjects.Add(o);
                continue;
            }

            //Find proper position for avatar
            if (Vector3.Distance(o.transform.position, transform.position) > _radarRadius)
            {
                needsAvatar = true;
                _helperTransform.LookAt(o.transform);
                position = transform.position + _radarRadius*_helperTransform.forward;
            }
            else
            {
                needsAvatar = false;
                position = o.transform.position;
            }

            //get or create avatar and move it to position
            avatar = _trackedAvatars[o];
            avatarParent = _avatarParents[o];
            if (needsAvatar && avatar != null)
            {
                avatar.transform.SetParent(o.transform.parent);
                avatar.transform.position = position;
            }
            else
            {
                avatar.transform.SetParent(avatarParent);
            }
        }
        if (destroyedObjects.Count > 0)
        {
            foreach (GameObject destroyedObject in destroyedObjects)
            {
                resetAvatar(destroyedObject);
            }
            UpdateTrackedObjectsSet();
        }
    }

}

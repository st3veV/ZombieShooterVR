﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class Radar : MonoBehaviour
{

    public GameObject EnemyAvatarPrefab;
    public GameObject LookAtGameObject;
    public GameObject ControlRotationGameObject;
    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;

    private Transform helperTransform;
    private float radarRadius = 10.0f;

    private GameObject[] trackedObjects;
    private Dictionary<GameObject, GameObject> trackedAvatars;
    private Dictionary<GameObject, Transform> avatarParents;

	// Use this for initialization
	void Start () {
        GameObject Helper = new GameObject("helper");
        Helper.transform.SetParent(transform);
	    helperTransform = Helper.transform;

        if(ZombieSpawner != null)
        {
            ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;
        }

        if(WeaponSpawner != null)
        {
            WeaponSpawner.OnWeaponTargetSpawned += WeaponSpawner_OnWeaponTargetSpawned;
        }

        trackedObjects = new GameObject[0];
        trackedAvatars = new Dictionary<GameObject, GameObject>();
        avatarParents = new Dictionary<GameObject, Transform>();
	}

    private void WeaponSpawner_OnWeaponTargetSpawned(GameObject weaponTarget)
    {
        LifetimeComponent weaponTargetLife = weaponTarget.GetComponent<LifetimeComponent>();
        weaponTargetLife.OnDie += weaponTargetLife_OnDie;

        if(!trackedAvatars.ContainsKey(weaponTarget))
        {
            Transform avatarParent = weaponTarget.transform.FindChild("Container");
            trackedAvatars.Add(weaponTarget, avatarParent.FindChild("TargetAvatar").gameObject);
            avatarParents.Add(weaponTarget, avatarParent);
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
        if (!trackedAvatars.ContainsKey(zombie))
        {
            trackedAvatars.Add(zombie, zombie.transform.FindChild("EnemyAvatar").gameObject);
            avatarParents.Add(zombie, zombie.transform);
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
	    helperTransform.rotation = Quaternion.Euler(90, eulerAngles.y, 0);
	    ControlRotationGameObject.transform.rotation = helperTransform.rotation;

        if (trackedObjects.Length > 0)
        {
            UpdateTrackedObjects();
        }

	}

    private void resetAvatar(LifetimeComponent obj)
    {
        if (trackedAvatars.ContainsKey(obj.gameObject))
        {
            GameObject avatar = trackedAvatars[obj.gameObject];
            avatar.transform.SetParent(obj.gameObject.transform);
            trackedAvatars.Remove(obj.gameObject);
            avatarParents.Remove(obj.gameObject);
        }
    }

    private void UpdateTrackedObjectsSet()
    {
        List<GameObject> tObjs = new List<GameObject>();
        foreach(GameObject o in trackedAvatars.Keys)
        {
            tObjs.Add(o);
        }
        trackedObjects = tObjs.ToArray();
    }

    private void UpdateTrackedObjects()
    {
        //UpdateEnemySet();
        GameObject avatar;
        Vector3 position;
        bool needsAvatar = false;
        foreach (GameObject o in trackedObjects)
        {
            //Find proper position for avatar
            if (Vector3.Distance(o.transform.position, transform.position) > radarRadius)
            {
                needsAvatar = true;
                helperTransform.LookAt(o.transform);
                position = transform.position + radarRadius*helperTransform.forward;
            }
            else
            {
                needsAvatar = false;
                position = o.transform.position;
            }

            //get or create avatar and move it to position
            avatar = trackedAvatars[o];
            Transform avatarParent = avatarParents[o];
            if (needsAvatar && avatar != null)
            {
                avatar.transform.SetParent(avatarParent.parent);
                avatar.transform.position = position;
            }
            else
            {
                avatar.transform.SetParent(avatarParent);
            }
        }
    }
}

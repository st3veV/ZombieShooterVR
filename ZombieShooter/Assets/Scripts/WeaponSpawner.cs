using UnityEngine;
using System.Collections.Generic;
using System;

public class WeaponSpawner : MonoBehaviour {

    public ZombieSpawner ZombieSpawner;
    public GameObject TargetPrefab;
    public Transform PlayerLocation;

    public event Action<GameObject> OnWeaponTargetSpawned;

    private List<GameObject> targetPool;

	void Start () {
        ZombieSpawner.OnZombieSpawned += ZombieSpawner_OnZombieSpawned;

        targetPool = new List<GameObject>();
	}

    private void ZombieSpawner_OnZombieSpawned(GameObject zombie)
    {
        LifetimeComponent zombieLife = zombie.GetComponent<LifetimeComponent>();
        zombieLife.OnDie += zombieLife_OnDie;
    }

    private void zombieLife_OnDie(LifetimeComponent obj)
    {
        obj.OnDie -= zombieLife_OnDie;
        SpawnAmmo(obj.transform);
    }

    public void SpawnAmmo(Transform sourceTransform)
    {
        GameObject target;
        if(targetPool.Count > 0)
        {
            target = targetPool[0];
            targetPool.RemoveAt(0);
        }
        else
        {
            target = Instantiate(TargetPrefab);
        }
        Vector3 targetPosition = sourceTransform.position;
        targetPosition.y = 0;
        target.transform.position = targetPosition;
        target.transform.LookAt(PlayerLocation);
        

        LifetimeComponent targetLife = target.GetComponent<LifetimeComponent>();
        targetLife.OnDie += targetLife_OnDie;

        target.SetActive(true);
        spawned(target);
    }

    void targetLife_OnDie(LifetimeComponent obj)
    {
        PickAmmo(obj.gameObject);
    }

    public void PickAmmo(GameObject target)
    {
        Debug.Log("Ammo picked");
        target.SetActive(false);
        targetPool.Add(target);
    }
	
    private void spawned(GameObject target)
    {
        var handler = OnWeaponTargetSpawned;
        if (handler != null)
            handler(target);
    }

	void Update () {
	
	}
}

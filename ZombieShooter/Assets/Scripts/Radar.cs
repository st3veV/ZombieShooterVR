using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class Radar : MonoBehaviour
{

    public GameObject EnemyAvatarPrefab;
    public GameObject LookAtGameObject;
    public GameObject ControlRotationGameObject;
    public ZombieSpawner Spawner;

    private Transform helperTransform;
    private float radarRadius = 10.0f;

    private GameObject[] enemies;
    private Dictionary<GameObject, GameObject> enemyAvatars;

	// Use this for initialization
	void Start () {
        GameObject Helper = new GameObject("helper");
        Helper.transform.SetParent(transform);
	    helperTransform = Helper.transform;

        if(Spawner != null)
        {
            Spawner.OnZombieSpawned += OnZombieSpawned;
        }
        enemies = new GameObject[0];
        enemyAvatars = new Dictionary<GameObject, GameObject>();

	}

    private void OnZombieSpawned(GameObject zombie)
    {
        LifetimeComponent zombieLife = zombie.GetComponent<LifetimeComponent>();
        zombieLife.OnDie += zombieLife_OnDie;

        if (!enemyAvatars.ContainsKey(zombie))
        {
            enemyAvatars.Add(zombie, zombie.transform.FindChild("EnemyAvatar").gameObject);
        }
        UpdateEnemySet();
    }

    private void zombieLife_OnDie(LifetimeComponent obj)
    {
        if(enemyAvatars.ContainsKey(obj.gameObject))
        {
            enemyAvatars.Remove(obj.gameObject);
        }
        obj.OnDie -= zombieLife_OnDie;
        UpdateEnemySet();
    }

	// Update is called once per frame
	void Update ()
	{
        //update camera transform
	    Vector3 eulerAngles = LookAtGameObject.transform.rotation.eulerAngles;
	    helperTransform.rotation = Quaternion.Euler(90, eulerAngles.y, 0);
	    ControlRotationGameObject.transform.rotation = helperTransform.rotation;

        if (enemies.Length > 0)
        {
            UpdateTrackedObjects();
        }

	}

    private void UpdateEnemySet()
    {
        List<GameObject> en = new List<GameObject>();
        foreach(GameObject o in enemyAvatars.Keys)
        {
            en.Add(o);
        }
        enemies = en.ToArray();
    }

    private void UpdateTrackedObjects()
    {
        //UpdateEnemySet();
        GameObject avatar;
        Vector3 position;
        bool needsAvatar = false;
        foreach (GameObject o in enemies)
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
            avatar = enemyAvatars[o];
            if (needsAvatar && avatar != null)
            {
                avatar.transform.SetParent(o.transform.parent);
                avatar.transform.position = position;
            }
            else
            {
                avatar.transform.SetParent(o.transform);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Random = System.Random;

public class ZombieSpawner : MonoBehaviour {

    public AICharacterControl Zombie;
    public Transform ZombieTarget;
    public Transform SpawnPoint;
    public LifetimeComponent AttactTarget;
    public UserDataComponent UserData;
    
    public float Diameter = 0f;
    public float ZombieDamage = BalancingData.ZOMBIE_DAMAGE;

    public event Action<GameObject> OnZombieSpawned;

    private InternalTimer _timer;
    public float SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;

    public bool IsSpawning = true;

    private List<AICharacterControl> zombiePool;

	void Start () {
        if (SpawnPoint == null)
        {
            SpawnPoint = gameObject.transform;
        } 
        if (Diameter == 0)
        {
            float distance = Mathf.Sqrt(SpawnPoint.transform.position.x * SpawnPoint.transform.position.x + SpawnPoint.transform.position.z * SpawnPoint.transform.position.z);
            Diameter = distance;
        }

        AttactTarget.OnDie += AttactTarget_OnDie;
	    zombiePool = new List<AICharacterControl>();
        _timer = new InternalTimer();
	    _timer.Set(SpawnInterval*1000);
	}

    void AttactTarget_OnDie(LifetimeComponent lifetimeComponent)
    {
        AttactTarget.OnDie -= AttactTarget_OnDie;
        IsSpawning = false;
    }
	
	void Update () {
        if (IsSpawning)
        {
            if(_timer.Update())
            {
                _timer.Reset();
                SpawnZombie();
                _timer.Set(SpawnInterval*1000);
            }
        }
	}

    public void SpawnZombie()
    {
        AICharacterControl clone;

        if (zombiePool.Count > 0)
        {
            clone = zombiePool[zombiePool.Count - 1];
            zombiePool.Remove(clone);
            LifetimeComponent lifetimeComponent = clone.GetComponent<LifetimeComponent>();
            lifetimeComponent.Reset();
        }
        else
        {
            clone = Instantiate(Zombie) as AICharacterControl;
            LifetimeComponent lifetimeComponent = clone.GetComponent<LifetimeComponent>();
            lifetimeComponent.Autodestroy = false;
            lifetimeComponent.OnDie += Zombie_OnDie;
            clone.OnPositionReached += clone_OnPositionReached;
            clone.target = ZombieTarget;
        }
        clone.transform.position = SpawnPoint.position;
        clone.gameObject.SetActive(true);

        zombieSpawned(clone.gameObject);

        ChoseNextPosition();
    }

    private void zombieSpawned(GameObject zombie)
    {
        var handler = OnZombieSpawned;
        if (handler != null)
            handler(zombie);
    }

    private void Zombie_OnDie(LifetimeComponent lifetimeComponent)
    {
        //dispose zombie
        DisposeZombie(lifetimeComponent.gameObject);

        //deal with score
        UserData.IncreaseScore(BalancingData.SCORE_FOR_ZOMBIE);
        SpawnInterval -= BalancingData.ZOMBIE_SPAWN_INTERVAL_DECREASE;
        if (SpawnInterval < BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM)
        {
            SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM;
        }
    }

    private void clone_OnPositionReached(GameObject obj)
    {
        DisposeZombie(obj);
        AttactTarget.ReceiveDamage(ZombieDamage);
    }

    private void DisposeZombie(GameObject go)
    {
        go.SetActive(false);
        zombiePool.Add(go.GetComponent<AICharacterControl>());
    }

    private void ChoseNextPosition()
    {
        Random rand = new Random();
        float angle = rand.Next(0,360);
        float value = angle * Mathf.Deg2Rad;
        float xpos = Diameter * Mathf.Cos(value);
        float zpos = Diameter * Mathf.Sin(value);
        SpawnPoint.position = new Vector3(xpos, 0.5f, zpos);
    }

    public void Reset()
    {
        SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;
    }
}

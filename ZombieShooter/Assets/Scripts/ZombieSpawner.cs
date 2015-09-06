using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.ThirdPerson;

public class ZombieSpawner : MonoBehaviour {

    public AICharacterControl Zombie;
    public Transform ZombieTarget;
    public Transform SpawnPoint;
    public LifetimeComponent AttactTarget;
    public UserDataComponent UserData;
    
    public float Diameter = 0f;
    public float ZombieDamage = BalancingData.ZOMBIE_DAMAGE;

    private float timer = 0f;
    public float SpawnInterval = 5f;

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
	}

    void AttactTarget_OnDie(LifetimeComponent lifetimeComponent)
    {
        AttactTarget.OnDie -= AttactTarget_OnDie;
        IsSpawning = false;
    }
	
	void Update () {
        if (IsSpawning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SpawnZombie();
                timer = SpawnInterval;
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
            clone.gameObject.SetActive(true);
        }
        else
        {
            clone = Instantiate(Zombie) as AICharacterControl;
        }
        clone.transform.position = SpawnPoint.position;
        clone.target = ZombieTarget;
        clone.OnPositionReached += clone_OnPositionReached;
        LifetimeComponent lifetimeComponent = clone.GetComponent<LifetimeComponent>();
        if (lifetimeComponent != null)
        {
            lifetimeComponent.OnDie += Zombie_OnDie;
        }
        ChoseNextPosition();
    }

    void Zombie_OnDie(LifetimeComponent lifetimeComponent)
    {
        lifetimeComponent.OnDie -= Zombie_OnDie;
        UserData.IncreaseScore(BalancingData.SCORE_FOR_ZOMBIE);
        SpawnInterval -= BalancingData.ZOMBIE_SPAWN_INTERVAL_DECREASE;
        if (SpawnInterval < BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM)
        {
            SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM;
        }
    }

    void clone_OnPositionReached(GameObject obj)
    {
        LifetimeComponent lifetimeComponent = obj.GetComponent<LifetimeComponent>();
        if (lifetimeComponent != null)
        {
            lifetimeComponent.OnDie -= Zombie_OnDie;
        }
        //Destroy(obj);
        obj.SetActive(false);
        zombiePool.Add(obj.GetComponent<AICharacterControl>());
        AttactTarget.ReceiveDamage(ZombieDamage);
    }

    private void ChoseNextPosition()
    {
        System.Random rand = new System.Random();
        float angle = rand.Next(0,360);
        float value = angle * (Mathf.PI / 180f);
        float xpos = Diameter * Mathf.Cos(value);
        float zpos = Diameter * Mathf.Sin(value);
        //SpawnPoint.Rotate(270f, angle, 0f);
        SpawnPoint.position = new Vector3(xpos,0.5f,zpos);
    }
}

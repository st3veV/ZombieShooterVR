using System;
using Controllers;
using Radar;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Utils;
using Random = System.Random;

public class ZombieSpawner : AutoObject<ZombieSpawner>
{

    public GameObject Zombie;
    public Transform SpawnPoint;
    
    public float Diameter = 0f;
    public float ZombieDamage = BalancingData.ZOMBIE_DAMAGE;

    public event Action<GameObject> OnZombieSpawned;

    private UserData _userData;
    private InternalTimer _timer;
    public float SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;

    public bool IsSpawning = true;

    private GameObjectPool _zombiePool;
    private LifetimeComponent _attactTarget;
    private Transform _zombieTarget;

    void Start () {
        Debug.Log("Start");
	    _userData = UserData.Instance;
        if (SpawnPoint == null)
        {
            SpawnPoint = gameObject.transform;
        } 
        if (Diameter == 0)
        {
            float distance = Mathf.Sqrt(SpawnPoint.transform.position.x * SpawnPoint.transform.position.x + SpawnPoint.transform.position.z * SpawnPoint.transform.position.z);
            Diameter = distance;
        }

        PlayerController playerController = PlayerController.Instance;
        _attactTarget = playerController.Lifetime;
        _attactTarget.OnDie += AttactTarget_OnDie;

        _zombieTarget = playerController.PlayerTransform;

        _zombiePool = new GameObjectPool(Zombie, gameObject);
        _timer = new InternalTimer();
	    _timer.Set(SpawnInterval*1000);
	}

    void AttactTarget_OnDie(LifetimeComponent lifetimeComponent)
    {
        _attactTarget.OnDie -= AttactTarget_OnDie;
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
        bool isNew;
        GameObject clone = _zombiePool.Get(out isNew);

        AICharacterControl characterControl = clone.GetComponent<AICharacterControl>();
        LifetimeComponent lifetimeComponent = clone.GetComponent<LifetimeComponent>();
        if (isNew)
        {
            lifetimeComponent.Autodestroy = false;
        }
        else
        {
            lifetimeComponent.Reset();
        }
        lifetimeComponent.OnDie += Zombie_OnDie;
        characterControl.OnPositionReached += Zombie_OnPositionReached;

        characterControl.SetTarget(_zombieTarget);
        clone.transform.position = SpawnPoint.position;
        clone.gameObject.SetActive(true);
        clone.GetComponent<ZombieAudioController>().Spawn();

        var radarTrackable = clone.GetComponent<RadarTrackable>();
        RadarController.Instance.AddTrackedObject(radarTrackable);

        zombieSpawned(clone.gameObject);

        ChoseNextPosition();
    }
    
    private void Zombie_OnDie(LifetimeComponent lifetimeComponent)
    {
        var radarTrackable = lifetimeComponent.GetComponent<RadarTrackable>();
        RadarController.Instance.RemoveTrackedObject(radarTrackable);

        //dispose zombie
        lifetimeComponent.gameObject.GetComponent<ThirdPersonCharacter>().Die(DisposeZombie);

        //deal with score
        _userData.IncreaseScore(BalancingData.SCORE_FOR_ZOMBIE);
        SpawnInterval -= BalancingData.ZOMBIE_SPAWN_INTERVAL_DECREASE;
        if (SpawnInterval < BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM)
        {
            SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM;
        }
    }

    private void Zombie_OnPositionReached(GameObject obj)
    {
        var radarTrackable = obj.GetComponent<RadarTrackable>();
        RadarController.Instance.RemoveTrackedObject(radarTrackable);

        obj.GetComponent<ThirdPersonCharacter>().Attack(AttackAndDispose);
    }

    private void AttackAndDispose(GameObject go)
    {
        _attactTarget.ReceiveDamage(ZombieDamage);
        DisposeZombie(go);
    }

    private void DisposeZombie(GameObject go)
    {
        AICharacterControl characterControl = go.GetComponent<AICharacterControl>();
        LifetimeComponent lifetimeComponent = go.GetComponent<LifetimeComponent>();
        characterControl.OnPositionReached -= Zombie_OnPositionReached;
        lifetimeComponent.OnDie -= Zombie_OnDie;

        go.SetActive(false);
        _zombiePool.Add(go);
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
        Debug.Log("Reset");
        _zombiePool.Clear();
        SpawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;
        IsSpawning = true;
        _timer.Set(SpawnInterval * 1000);
    }

    #region Event invocators

    private void zombieSpawned(GameObject zombie)
    {
        var handler = OnZombieSpawned;
        if (handler != null)
            handler(zombie);
    }

    #endregion
}

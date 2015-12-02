using System;
using Controllers;
using Radar;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Utils;
using Random = System.Random;

public class ZombieSpawner : AutoObject<ZombieSpawner>
{
    private Transform _spawnPoint;
    
    private float _diameter = BalancingData.ZOMBIE_SPAWN_DIAMETER;
    private float _zombieDamage = BalancingData.ZOMBIE_DAMAGE;
    private float _spawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;

    public event Action<Zombie> OnZombieSpawned;

    private GameObject _zombie;
    private UserData _userData;
    private InternalTimer _timer;

    public bool IsSpawning = true;

    private AutoObjectWrapperPool<Zombie> _zombiePool;
    private LifetimeComponent _attactTarget;
    private Transform _zombieTarget;

    void Start () {
        Debug.Log("Start");
	    _userData = UserData.Instance;
        if (_spawnPoint == null)
        {
            _spawnPoint = new GameObject("SpawnPoint").transform;
            _spawnPoint.SetParent(gameObject.transform);
        }

        PlayerController playerController = PlayerController.Instance;
        _attactTarget = playerController.Lifetime;
        _attactTarget.OnDie += AttactTarget_OnDie;

        _zombieTarget = playerController.PlayerTransform;

        _zombie = Resources.Load("Prefabs/ZombieEnemy") as GameObject;
        _zombiePool = new AutoObjectWrapperPool<Zombie>(_zombie, gameObject);
        
        _timer = new InternalTimer();
        _timer.Set(_spawnInterval*1000);
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
                ChoseNextPosition();
                SpawnZombie();
                _timer.Set(_spawnInterval*1000);
            }
        }
	}

    public void SpawnZombieAt(Vector3 position)
    {
        _spawnPoint.position = position;
        SpawnZombie();
    }

    public void SpawnZombie()
    {
        bool isNew;
        Zombie clone = _zombiePool.Get(out isNew);
        
        if (isNew)
        {
            clone.Lifetime.Autodestroy = false;
        }
        else
        {
            clone.Lifetime.Reset();
        }
        clone.AddDieListener(Zombie_OnDie);
        clone.AddPositionReachedListener(Zombie_OnPositionReached);

        clone.AiCharacterControl.SetTarget(_zombieTarget);
        clone.transform.position = _spawnPoint.position;
        clone.gameObject.SetActive(true);
        clone.AudioController.Spawn();
        
        RadarController.Instance.AddTrackedObject(clone.RadarTrackable);

        zombieSpawned(clone);
    }
    
    private void Zombie_OnDie(Zombie zombie)
    {
        RemoveZombieListeners(zombie);
        RadarController.Instance.RemoveTrackedObject(zombie.RadarTrackable);

        //dispose zombie
        zombie.ThirdPersonCharacter.Die(DisposeZombie);

        //deal with score
        _userData.IncreaseScore(BalancingData.SCORE_FOR_ZOMBIE);
        _spawnInterval -= BalancingData.ZOMBIE_SPAWN_INTERVAL_DECREASE;
        if (_spawnInterval < BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM)
        {
            _spawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_MINIMUM;
        }
    }

    private void Zombie_OnPositionReached(Zombie zombie)
    {
        RemoveZombieListeners(zombie);
        RadarController.Instance.RemoveTrackedObject(zombie.RadarTrackable);

        zombie.ThirdPersonCharacter.Attack(AttackAndDispose);
    }

    private void AttackAndDispose(GameObject go)
    {
        _attactTarget.ReceiveDamage(_zombieDamage);
        DisposeZombie(go);
    }

    private void DisposeZombie(GameObject go)
    {
        Zombie zombie = go.GetComponent<Zombie>();
        RemoveZombieListeners(zombie);

        go.SetActive(false);
        _zombiePool.Add(zombie);
    }

    private void RemoveZombieListeners(Zombie zombie)
    {
        zombie.RemovePositionReachedListener(Zombie_OnPositionReached);
        zombie.RemoveDieListener(Zombie_OnDie);
    }

    private void ChoseNextPosition()
    {
        Random rand = new Random();
        float angle = rand.Next(0, 360);
        float value = angle * Mathf.Deg2Rad;
        float xpos = _diameter * Mathf.Cos(value);
        float zpos = _diameter * Mathf.Sin(value);
        _spawnPoint.position = new Vector3(xpos, 0.5f, zpos);
    }

    public void Reset()
    {
        Debug.Log("Reset");
        if (_zombiePool != null && _zombiePool.HasItems())
        {
            _zombiePool.Clear();
        }
        _spawnInterval = BalancingData.ZOMBIE_SPAWN_INTERVAL_INITIAL;
        IsSpawning = true;
        _timer.Set(_spawnInterval*1000);
    }

    #region Event invocators

    private void zombieSpawned(Zombie zombie)
    {
        var handler = OnZombieSpawned;
        if (handler != null)
            handler(zombie);
    }

    #endregion
}

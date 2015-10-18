using System;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;
    public Gun UserGun;
    public LifetimeComponent UserLifetime;

    public event Action OnGameEnded;

	void Start ()
	{
	    AssignReferences();
	}

    private void AssignReferences()
    {
        ZombieSpawner = GameObject.Find("ZombieSpawner").GetComponent<ZombieSpawner>();
        WeaponSpawner = GameObject.Find("WeaponSpawner").GetComponent<WeaponSpawner>();
        UserGun = GameObject.Find("Gun").GetComponent<Gun>();
        UserLifetime = GameObject.Find("UserData").GetComponent<LifetimeComponent>();

        Initialize();
    }

    public void Initialize()
    {
        ZombieSpawner.Reset();
        ZombieSpawner.IsSpawning = true;

        WeaponSpawner.Reset();
        UserLifetime.OnDie += UserLifetime_OnDie;
    }

    void UserLifetime_OnDie(LifetimeComponent obj)
    {
        UserLifetime.OnDie -= UserLifetime_OnDie;
        ZombieSpawner.IsSpawning = false;
        ZombieSpawner.Reset();
        OnOnGameEnded();
    }

    protected virtual void OnOnGameEnded()
    {
        var handler = OnGameEnded;
        if (handler != null) handler();
    }
}

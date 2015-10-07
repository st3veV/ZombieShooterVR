using UnityEngine;

public class GameController : MonoBehaviour
{

    public ZombieSpawner ZombieSpawner;
    public WeaponSpawner WeaponSpawner;
    public Gun UserGun;

	void Start ()
	{
	    AssignReferences();
	}

    private void AssignReferences()
    {
        ZombieSpawner = GameObject.Find("ZombieSpawner").GetComponent<ZombieSpawner>();
        WeaponSpawner = GameObject.Find("WeaponSpawner").GetComponent<WeaponSpawner>();
        UserGun = GameObject.Find("Gun").GetComponent<Gun>();

        Initialize();
    }

    public void Initialize()
    {
        ZombieSpawner.Reset();
        ZombieSpawner.IsSpawning = true;

        WeaponSpawner.Reset();

        UserGun.Reset();
    }

    void Update () {
	
	}
}

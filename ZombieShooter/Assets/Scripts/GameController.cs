using UnityEngine;

public class GameController : MonoBehaviour
{

    public ZombieSpawner ZombieSpawner;

	void Start ()
	{
	    AssignReferences();
	}

    private void AssignReferences()
    {
        ZombieSpawner = GameObject.Find("ZombieSpawner").GetComponent<ZombieSpawner>();

        Initialize();
    }

    public void Initialize()
    {
        ZombieSpawner.Reset();
        ZombieSpawner.IsSpawning = true;
    }

    void Update () {
	
	}
}

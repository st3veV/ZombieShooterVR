using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;

public class ZombieSpawner : MonoBehaviour {

    public AICharacterControl Zombie;
    public Transform ZombieTarget;
    public Transform SpawnPoint;
    public LifetimeComponent AttactTarget;
    public float Diameter = 0f;
    public float ZombieDamage = 100.0f;

    private float timer = 0f;
    public float SpawnInterval = 5f;

    public bool IsSpawning = true;

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
	}

    void AttactTarget_OnDie()
    {
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
        Debug.Log("Spawn");
        AICharacterControl clone = Instantiate(Zombie) as AICharacterControl;
        clone.transform.position = SpawnPoint.position;
        clone.target = ZombieTarget;
        clone.OnPositionReached += clone_OnPositionReached;
        ChoseNextPosition();
    }

    void clone_OnPositionReached(GameObject obj)
    {
        Destroy(obj);
        AttactTarget.ReceiveDamage(ZombieDamage);
    }

    private void ChoseNextPosition()
    {
        System.Random rand = new System.Random();
        float angle = rand.Next(0,360);
        Debug.Log("angle: " + angle);
        float value = angle * (Mathf.PI / 180f);
        float xpos = Diameter * Mathf.Cos(value);
        float zpos = Diameter * Mathf.Sin(value);
        SpawnPoint.Rotate(270f, angle, 0f);
        SpawnPoint.position = new Vector3(xpos,0.5f,zpos);
        Debug.Log("next position: " + SpawnPoint.position);
    }
}

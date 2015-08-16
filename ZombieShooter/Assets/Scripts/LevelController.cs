using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {

    public LifetimeComponent UserLifetime;

	// Use this for initialization
	void Start () {
        UserLifetime.OnDie += UserLifetime_OnDie;
        UserLifetime.OnDamage += UserLifetime_OnDamage;
	}

    void UserLifetime_OnDamage(float damage)
    {
        Debug.Log("Boom, you just got hit!! (this much: "+damage+")");
    }

    void UserLifetime_OnDie(LifetimeComponent lifetimeComponent)
    {
        Debug.Log("You just died :(");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}

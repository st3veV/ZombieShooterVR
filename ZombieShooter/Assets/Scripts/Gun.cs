using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Rigidbody bullet;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Fire()
    {
        //Debug.Log("Fire!");
        Rigidbody clone = Instantiate(bullet, transform.position, transform.rotation) as Rigidbody;
        clone.AddForce(clone.transform.forward * 500);
    }
}

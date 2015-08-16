using System;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Rigidbody Bullet;
    public int Cadency = 1000;

    public event Action OnWeaponKick;

    private bool _isFiring = false;
    private float timer = 0;

    // Use this for initialization
    void Start ()
    {
        timer = Cadency;
    }
    
    // Update is called once per frame
    void Update () {
        if (_isFiring)
        {
            timer -= (Time.deltaTime * 1000f);
            if (timer <= 0)
            {
                Fire();
                timer = Cadency;
            }
        }
    }

    public void Fire()
    {
        Kick();
        //Debug.Log("Fire!");
        Rigidbody clone = Instantiate(Bullet, transform.position, transform.rotation) as Rigidbody;
        clone.AddForce(clone.transform.forward * 1500);
    }

    public void StartShooting()
    {
        if (_isFiring == false)
        {
            _isFiring = true;
            timer = 0;
            //Kick();
        }
    }

    public void StopShooting()
    {
        if (_isFiring)
        {
            //timer = 0;
            _isFiring = false;
            Kick();
        }
    }

    protected virtual void Kick()
    {
        var handler = OnWeaponKick;
        if (handler != null) handler();
    }
}

using UnityEngine;
using System;
using System.Collections;

public class LifetimeComponent : MonoBehaviour {

    public float LifetimeDamage = 100f;
    private float OriginalHealth;
    public event Action<LifetimeComponent> OnDie;
    public event Action<float> OnDamage;
    public bool Autodestroy = true;

    public float CurrentHealthPercentage
    {
        get { return LifetimeDamage/OriginalHealth; }
    }

    // Use this for initialization
    void Start ()
    {
        OriginalHealth = LifetimeDamage;
    }
    
    // Update is called once per frame
    void Update () {
        if(LifetimeDamage <= 0)
        {
            if (OnDie != null)
            {
                OnDie(this);
            }
            if (Autodestroy)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        LifetimeDamage -= damage;
        if(OnDamage != null)
        {
            OnDamage(damage);
        }
    }

}

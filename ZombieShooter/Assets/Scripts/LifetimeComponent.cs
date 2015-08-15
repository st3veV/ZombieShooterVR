using UnityEngine;
using System;
using System.Collections;

public class LifetimeComponent : MonoBehaviour {

    public float LifetimeDamage = 100f;
    public event Action OnDie;
    public event Action<float> OnDamage;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        if(LifetimeDamage <= 0)
        {
            if (OnDie != null)
            {
                OnDie();
            }
            Destroy(gameObject);
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

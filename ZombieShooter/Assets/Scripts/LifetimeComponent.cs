using System;
using UnityEngine;

public class LifetimeComponent : MonoBehaviour {

    public float LifetimeDamage = 100f;
    public event Action<LifetimeComponent> OnDie;
    public event Action<float> OnDamage;
    public bool Autodestroy = true;

    private float _originalHealth;

    public float CurrentHealthPercentage
    {
        get { return LifetimeDamage/_originalHealth; }
    }

    // Use this for initialization
    void Start ()
    {
        _originalHealth = LifetimeDamage;
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

    public void Reset()
    {
        LifetimeDamage = _originalHealth;
    }
}

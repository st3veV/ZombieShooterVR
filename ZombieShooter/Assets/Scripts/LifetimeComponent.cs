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
    
    void Start ()
    {
        _originalHealth = LifetimeDamage;
    }
    
    public void ReceiveDamage(float damage)
    {
        LifetimeDamage -= damage;
        if(OnDamage != null)
        {
            OnDamage(damage);
        }

        if (LifetimeDamage <= 0)
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

    public void Reset()
    {
        LifetimeDamage = _originalHealth;
    }
}

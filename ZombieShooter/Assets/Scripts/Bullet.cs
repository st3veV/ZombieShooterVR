using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    private float startTime; 
    public float SecondsUntilDestroy = 10;
    private float _currentDamage;

    public event Action<Bullet> OnBulletDie;
    private InternalTimer _timer;

    void Start () {
        _timer = new InternalTimer();
        Reset();
    }
    
    void Update () {
        if (_timer.Update())
        {
            OnOnBulletDie(this);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        LifetimeComponent lifetime = collision.gameObject.GetComponent<LifetimeComponent>();
        if (lifetime != null)
        {
            lifetime.ReceiveDamage(_currentDamage);
            if (OnBulletDie != null)
            {
                OnOnBulletDie(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void SetDamage(float damage)
    {
        _currentDamage = damage;
    }

    public void Reset()
    {
        _timer.Set(SecondsUntilDestroy*1000);
    }

    protected virtual void OnOnBulletDie(Bullet obj)
    {
        var handler = OnBulletDie;
        if (handler != null) handler(obj);
    }
}

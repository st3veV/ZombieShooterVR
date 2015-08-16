using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    
    private float startTime; 
    public float SecondsUntilDestroy = 10;
    // Use this for initialization
    void Start () {
        startTime = Time.time;
    }
    
    // Update is called once per frame
    void Update () {
        if(Time.time-startTime >= SecondsUntilDestroy)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        LifetimeComponent lifetime = collision.gameObject.GetComponent<LifetimeComponent>();
        if (lifetime != null)
        {
            lifetime.ReceiveDamage(BalancingData.BULLET_DAMAGE);
            Destroy(gameObject);
        }
    }
}

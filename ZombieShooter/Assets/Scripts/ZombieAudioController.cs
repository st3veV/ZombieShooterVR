using Controllers;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(AudioSource))]
public class ZombieAudioController : MonoBehaviour {

    private AudioSource _audioSource;
    private LifetimeComponent _zombieLifetime;
    private InternalTimer _timer;
    private Random _random;

    public AudioClip SpawnSound;
    public AudioClip HitSound;
    public AudioClip WalkSound;

    void Awake()
    {
        _random = new Random();
        _timer = new InternalTimer();

        _audioSource = gameObject.GetComponent<AudioSource>();
        _zombieLifetime = gameObject.GetComponent<LifetimeComponent>();
        _zombieLifetime.OnDamage += _zombieLifetime_OnDamage;
    }
    
    private void _zombieLifetime_OnDamage(float obj)
    {
        _audioSource.PlayOneShot(HitSound);
    }

    private void OnUpdate ()
    {
        if (_timer.Update())
        {
            _timer.Set(_random.Next(5, 10) * 1000);
            _audioSource.PlayOneShot(WalkSound);
        }
    }

    public void Spawn()
    {
        _audioSource.PlayOneShot(SpawnSound);
        EventManager.Instance.AddUpdateListener(OnUpdate);
        _timer.Set(_random.Next(5, 10)*1000);
    }

}

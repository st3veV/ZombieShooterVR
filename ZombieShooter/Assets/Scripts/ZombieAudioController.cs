using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZombieAudioController : MonoBehaviour {

    private AudioSource _audioSource;
    private LifetimeComponent _zombieLifetime;
    private bool _moving = true;

    public AudioClip SpawnSound;
    public AudioClip HitSound;
    public AudioClip WalkSound;

    void Awake()
    {
        _audioSource = gameObject.GetComponent<AudioSource>();
        _zombieLifetime = gameObject.GetComponent<LifetimeComponent>();
        _zombieLifetime.OnDamage += _zombieLifetime_OnDamage;
        _zombieLifetime.OnDie += _zombieLifetime_OnDie;
    }

    private void _zombieLifetime_OnDie(LifetimeComponent obj)
    {
        _moving = false;
    }

    private void _zombieLifetime_OnDamage(float obj)
    {
        _audioSource.PlayOneShot(HitSound);
    }
    
    private IEnumerator SoundCoroutine()
    {
        while (_moving)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            if (!_moving) continue;
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(WalkSound);
            }
            else
            {
                _moving = false;
            }
        }
        RemoveListeners();
    }

    private void RemoveListeners()
    {
        _zombieLifetime.OnDamage -= _zombieLifetime_OnDamage;
        _zombieLifetime.OnDie -= _zombieLifetime_OnDie;
    }

    public void Spawn()
    {
        _audioSource.PlayOneShot(SpawnSound);
        StartCoroutine(SoundCoroutine());
    }

}

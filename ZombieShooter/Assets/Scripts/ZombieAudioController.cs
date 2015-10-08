using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(AudioSource))]
public class ZombieAudioController : MonoBehaviour {

    private AudioSource _audioSource;
    private InternalTimer _timer;
    private Random _random;

	void Start ()
	{
	    _audioSource = gameObject.GetComponent<AudioSource>();
	    
	}

    void Update ()
    {
        
    }

    public void Spawn()
    {
    }

}

enum ZombieAudio
{
    Spawn,
    Hit,
    Attack,
    Move
}

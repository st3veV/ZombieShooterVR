using System;
using System.Collections.Generic;
using Radar;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Utils;

public class Zombie : AutoObjectWrapper<Zombie>
{
    public LifetimeComponent Lifetime;
    public AICharacterControl AiCharacterControl;
    public RadarTrackable RadarTrackable;
    public ThirdPersonCharacter ThirdPersonCharacter;
    public ZombieAudioController AudioController;

    private readonly List<Action<Zombie>> _dieListeners = new List<Action<Zombie>>();
    private readonly List<Action<Zombie>> _positionReachedListeners = new List<Action<Zombie>>();
    

    protected override void SetPrefab(GameObject originalPrefab)
    {
        base.SetPrefab(originalPrefab);

        Lifetime = OriginalPrefab.GetComponent<LifetimeComponent>();
        Lifetime.OnDie += Lifetime_OnDie;

        AiCharacterControl = OriginalPrefab.GetComponent<AICharacterControl>();
        AiCharacterControl.OnPositionReached += AiCharacterControl_OnPositionReached;

        RadarTrackable = OriginalPrefab.GetComponent<RadarTrackable>();
        ThirdPersonCharacter = OriginalPrefab.GetComponent<ThirdPersonCharacter>();
        AudioController = OriginalPrefab.GetComponent<ZombieAudioController>();
    }

    private void AiCharacterControl_OnPositionReached(GameObject obj)
    {
        ExecuteListeners(_positionReachedListeners);
    }

    private void Lifetime_OnDie(LifetimeComponent obj)
    {
        ExecuteListeners(_dieListeners);
    }

    public void AddDieListener(Action<Zombie> listener)
    {
        _dieListeners.Add(listener);
    }

    public void RemoveDieListener(Action<Zombie> listener)
    {
        if (_dieListeners.Contains(listener))
        {
            _dieListeners.Remove(listener);
        }
    }
    
    public void AddPositionReachedListener(Action<Zombie> listener)
    {
        _positionReachedListeners.Add(listener);
    }

    public void RemovePositionReachedListener(Action<Zombie> listener)
    {
        if (_positionReachedListeners.Contains(listener))
        {
            _positionReachedListeners.Remove(listener);
        }
    }
    

    private void ExecuteListeners(List<Action<Zombie>> listeners)
    {
        if (listeners.Count > 0)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                Action<Zombie> listener = listeners[i];
                listener(this);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Radar;
using UnityEngine;
using Utils;

public class AmmoTarget : AutoObjectWrapper<AmmoTarget>
{
    public LifetimeComponent Lifetime;
    public RadarTrackable RadarTrackable;
    public PickupHolder PickupHolder;

    private readonly List<Action<AmmoTarget>> _dieListeners = new List<Action<AmmoTarget>>();

    protected override void SetPrefab(GameObject originalPrefab)
    {
        base.SetPrefab(originalPrefab);

        Lifetime = OriginalPrefab.GetComponent<LifetimeComponent>();
        Lifetime.OnDie += Lifetime_OnDie;

        RadarTrackable = OriginalPrefab.GetComponent<RadarTrackable>();
        PickupHolder = OriginalPrefab.GetComponent<PickupHolder>();
    }
    
    private void Lifetime_OnDie(LifetimeComponent obj)
    {
        ExecuteListeners(_dieListeners);
    }

    public void AddDieListener(Action<AmmoTarget> listener)
    {
        _dieListeners.Add(listener);
    }

    public void RemoveDieListener(Action<AmmoTarget> listener)
    {
        if (_dieListeners.Contains(listener))
        {
            _dieListeners.Remove(listener);
        }
    }

}

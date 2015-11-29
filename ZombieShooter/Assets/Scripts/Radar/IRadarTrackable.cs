using UnityEngine;

namespace Radar
{
    public interface IRadarTrackable
    {
        GameObject Avatar { get; }
        GameObject Trackable { get; }
    }
}
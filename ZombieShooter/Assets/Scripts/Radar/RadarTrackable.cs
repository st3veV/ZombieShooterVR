using Radar;
using UnityEngine;

namespace Radar
{
    public class RadarTrackable : MonoBehaviour, IRadarTrackable
    {
        public GameObject RadarAvatar;

        public GameObject Avatar
        {
            get { return RadarAvatar; }
        }

        public GameObject Trackable
        {
            get { return gameObject; }
        }
        
    }
}
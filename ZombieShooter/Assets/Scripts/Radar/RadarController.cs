using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
    public class RadarController : Singleton<RadarController>
    {
        private Transform _helperTransform;
        private const float RadarRadius = 15.0f;

        private GameObject _centerGameObject;
        private GameObject _sightWedgeDisplay;

        private void Awake()
        {
            Debug.Log("Radar controller awake");
            GameObject radar = Instantiate(Resources.Load("Prefabs/Radar") as GameObject);
            radar.name = "Radar";
            radar.transform.SetParent(gameObject.transform);
            _sightWedgeDisplay = radar.transform.FindChild("RadarSightWedgeQuad").gameObject;
        }

        private void Start()
        {
            GameObject helper = new GameObject("helper");
            helper.transform.SetParent(transform);
            _helperTransform = helper.transform;
        }

        public void SetCenterObject(GameObject centerObject)
        {
            _centerGameObject = centerObject;
        }

        private readonly List<IRadarTrackable> _tracked = new List<IRadarTrackable>();

        public void AddTrackedObject(IRadarTrackable trackedObject)
        {
            if (!_tracked.Contains(trackedObject))
            {
                _tracked.Add(trackedObject);
            }
        }

        public void RemoveTrackedObject(IRadarTrackable trackedObject)
        {
            if (_tracked.Contains(trackedObject))
            {
                _tracked.Remove(trackedObject);
            }
        }
        
        private void Update()
        {
            //Update radar view
            Vector3 eulerAngles = _centerGameObject.transform.rotation.eulerAngles;
            _helperTransform.rotation = Quaternion.Euler(90, eulerAngles.y, 0);
            _sightWedgeDisplay.transform.rotation = _helperTransform.rotation;
            var centerPosition = _centerGameObject.transform.position;
            transform.position = new Vector3(centerPosition.x, 0, centerPosition.z);
            
            //Update objects on radar
            for (int i = 0; i < _tracked.Count; i++)
            {
                var radarTrackable = _tracked[i];
                var o = radarTrackable.Trackable;
                Vector3 position;
                //Find proper position for avatar
                if (Vector3.Distance(o.transform.position, transform.position) > RadarRadius)
                {
                    _helperTransform.LookAt(o.transform);
                    position = transform.position + RadarRadius*_helperTransform.forward;
                }
                else
                {
                    position = o.transform.position;
                }
                radarTrackable.Avatar.transform.position = position;
            }
        }
    }
}
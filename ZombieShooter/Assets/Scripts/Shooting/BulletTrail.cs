using System;
using UnityEngine;

namespace Shooting
{
    [RequireComponent(typeof (LineRenderer))]
    public class BulletTrail : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        private Vector3 _destination;
        private float _distance;
        private float _currentStep = 0;

        public Action<GameObject> OnDone;
        private bool _animate;
        private Vector3 _origin;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetTarget(Vector3 origin, Vector3 target)
        {
            _origin = origin;
            _destination = target;
            _distance = Vector3.Distance(origin, target);
            _currentStep = 0;
            _animate = true;
        }

        private void Update()
        {
            if (_animate)
            {
                if (_currentStep < _distance)
                {
                    float x0 = Mathf.Lerp(0, _distance, _currentStep);
                    _currentStep += _distance/3;
                    float x1 = Mathf.Lerp(0, _distance, _currentStep);

                    Vector3 origin = _origin;
                    Vector3 destination = _destination;

                    Vector3 pos0 = x0*Vector3.Normalize(destination - origin) + origin;
                    Vector3 pos1 = x1*Vector3.Normalize(destination - origin) + origin;

                    _lineRenderer.SetPosition(0, pos0);
                    _lineRenderer.SetPosition(1, pos1);
                }
                else
                {
                    _animate = false;
                    OnDone(gameObject);
                }
            }
        }
    }
}
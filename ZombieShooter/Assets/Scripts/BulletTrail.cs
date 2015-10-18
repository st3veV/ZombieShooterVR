using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class BulletTrail : MonoBehaviour
{
    LineRenderer _lineRenderer;

    private Vector3 _destination;
    private float _distance;
    private const float AnimLength = .1f;
    private float _currentStep = 0;

    public Action<GameObject> OnDone;
    private bool _animate;
    private Vector3 _origin;

    void Start ()
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

    void Update ()
    {
        if (_animate)
        {
            if (_currentStep < AnimLength)
            {
                float x0 = Mathf.Lerp(0, _distance, _currentStep);
                _currentStep += AnimLength/3;
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

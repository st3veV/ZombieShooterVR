using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{

    public float LaserWidth = 0.010f;
    public float MaxLength = 50.0f;
    public Color Color = Color.red;


    private LineRenderer _lineRenderer;
    private float _length;

    private Transform _myTransform;


    // Use this for initialization
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.SetWidth(LaserWidth, LaserWidth);
        _myTransform = transform;
        
        _lineRenderer.SetVertexCount(2);
    }

    void Update()
    {
        RenderLaser();
    }

    private void RenderLaser()
    {
        UpdateLength();
        
        _lineRenderer.SetPosition(1,new Vector3(0,0,_length));
        _lineRenderer.SetColors(Color, Color);
    }

    private void UpdateLength()
    {
        RaycastHit[] hit;
        hit = Physics.RaycastAll(_myTransform.position, _myTransform.forward, MaxLength);

        int i = 0;
        while (i < hit.Length)
        {
            if (!hit[i].collider.isTrigger)
            {
                _length = hit[i].distance + 2.0f;
                return;
            }
            i++;
        }
        _length = MaxLength;
    }
}
